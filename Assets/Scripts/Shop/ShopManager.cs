using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Player;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Main controller for the shop system. Orchestrates all shop-related functionality.
    /// Single Responsibility: Coordinate shop operations between different systems.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [Header("Shop Configuration")]
        [SerializeField] private WeaponUpgradeRepositorySO weaponRepository;
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool autoRefillAmmo = true; // New design: no ammo purchases
        
        [Header("Dependencies")]
        [SerializeField] private WeaponManager weaponManager;
        [SerializeField] private PlayerInventory playerInventory;
        
        // Core services
        private WeaponUpgradeService upgradeService;
        private Dictionary<EWeapons, WeaponStatsAdapter> weaponAdapters;
        
        // Static instance for easy access
        public static ShopManager Instance { get; private set; }
        
        // Events for UI updates
        public event Action<EWeapons, int, int> OnWeaponPurchased; // weapon, level, cost
        public event Action<EWeapons, int, int> OnWeaponUpgraded;  // weapon, newLevel, cost
        public event Action<ConsumableType, int, int> OnConsumablePurchased; // type, quantity, cost
        public event Action OnShopOpened;
        public event Action OnShopClosed;
        public event Action OnInsufficientFunds;
        
        // Shop state tracking
        public bool IsShopOpen { get; private set; }
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeShop();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Find dependencies if not set in inspector
            if (weaponManager == null)
                weaponManager = FindObjectOfType<WeaponManager>();
            
            if (playerInventory == null)
                playerInventory = FindObjectOfType<PlayerInventory>();
            
            // Subscribe to wave events for auto-ammo refill
            if (autoRefillAmmo && WaveManager.Instance != null)
            {
                WaveManager.Instance.OnWaveStarted += OnWaveStarted;
            }
            
            ValidateDependencies();
        }
        
        private void InitializeShop()
        {
            // Initialize weapon adapters dictionary
            weaponAdapters = new Dictionary<EWeapons, WeaponStatsAdapter>();
            
            // Create upgrade service
            if (weaponRepository != null)
            {
                upgradeService = new WeaponUpgradeService(weaponRepository, debugMode);
            }
            else
            {
                Debug.LogError("ShopManager: WeaponUpgradeRepository not assigned!");
            }
            
            if (debugMode)
            {
                Debug.Log("ShopManager initialized successfully");
            }
        }
        
        private void ValidateDependencies()
        {
            bool hasErrors = false;
            
            if (weaponRepository == null)
            {
                Debug.LogError("ShopManager: WeaponUpgradeRepository not assigned!");
                hasErrors = true;
            }
            
            if (WeaponLevelTracker.Instance == null)
            {
                Debug.LogError("ShopManager: WeaponLevelTracker not found in scene!");
                hasErrors = true;
            }
            
            if (ConsumablePricingService.Instance == null)
            {
                Debug.LogError("ShopManager: ConsumablePricingService not found in scene!");
                hasErrors = true;
            }
            
            if (CurrencyManager.Instance == null)
            {
                Debug.LogError("ShopManager: CurrencyManager not found in scene!");
                hasErrors = true;
            }
            
            if (hasErrors)
            {
                Debug.LogError("ShopManager: Cannot function properly due to missing dependencies!");
            }
        }
        
        #region Weapon Purchase and Upgrade
        
        /// <summary>
        /// Attempts to purchase a weapon
        /// </summary>
        /// <param name="weaponType">Type of weapon to purchase</param>
        /// <returns>True if purchase was successful</returns>
        public bool TryPurchaseWeapon(EWeapons weaponType)
        {
            if (!ValidateShopOperation())
                return false;
            
            // Check if weapon can be purchased
            if (!WeaponLevelTracker.Instance.CanPurchase(weaponType))
            {
                if (debugMode)
                    Debug.Log($"Cannot purchase {weaponType}: already owned");
                return false;
            }
            
            // Get purchase cost
            int cost = WeaponUpgradeCostCalculator.GetWeaponPurchasePrice(weaponType);
            
            // Check if player has enough money
            if (!CurrencyManager.Instance.SpendCash(cost))
            {
                OnInsufficientFunds?.Invoke();
                if (debugMode)
                    Debug.Log($"Cannot purchase {weaponType}: insufficient funds (need ${cost})");
                return false;
            }
            
            // Purchase weapon (set to level 1)
            WeaponLevelTracker.Instance.PurchaseWeapon(weaponType);
            
            // Initialize weapon adapter with base stats
            InitializeWeaponAdapter(weaponType);
            
            // Trigger events
            OnWeaponPurchased?.Invoke(weaponType, 1, cost);
            
            if (debugMode)
            {
                Debug.Log($"Purchased {weaponType} for ${cost}");
            }
            
            return true;
        }
        
        /// <summary>
        /// Attempts to upgrade a weapon to the next level
        /// </summary>
        /// <param name="weaponType">Type of weapon to upgrade</param>
        /// <returns>True if upgrade was successful</returns>
        public bool TryUpgradeWeapon(EWeapons weaponType)
        {
            if (!ValidateShopOperation())
                return false;
            
            // Check if weapon can be upgraded
            if (!WeaponLevelTracker.Instance.CanLevelUp(weaponType))
            {
                if (debugMode)
                    Debug.Log($"Cannot upgrade {weaponType}: not owned or max level");
                return false;
            }
            
            int currentLevel = WeaponLevelTracker.Instance.GetWeaponLevel(weaponType);
            int nextLevel = currentLevel + 1;
            
            // Get upgrade cost
            int cost = WeaponUpgradeCostCalculator.CalculateNextLevelCost(weaponType, currentLevel);
            if (cost < 0)
            {
                Debug.LogError($"Invalid upgrade cost for {weaponType} level {nextLevel}");
                return false;
            }
            
            // Check if player has enough money
            if (!CurrencyManager.Instance.SpendCash(cost))
            {
                OnInsufficientFunds?.Invoke();
                if (debugMode)
                    Debug.Log($"Cannot upgrade {weaponType}: insufficient funds (need ${cost})");
                return false;
            }
            
            // Upgrade weapon
            WeaponLevelTracker.Instance.UpgradeWeapon(weaponType);
            
            // Apply upgrade to weapon stats
            ApplyUpgradeToWeapon(weaponType, nextLevel);
            
            // Check for ultimate ability activation
            if (nextLevel >= 10)
            {
                ActivateUltimateAbility(weaponType);
            }
            
            // Trigger events
            OnWeaponUpgraded?.Invoke(weaponType, nextLevel, cost);
            
            if (debugMode)
            {
                Debug.Log($"Upgraded {weaponType} to level {nextLevel} for ${cost}");
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets information about a weapon for shop display
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>Weapon shop info</returns>
        public WeaponShopInfo GetWeaponInfo(EWeapons weaponType)
        {
            int currentLevel = WeaponLevelTracker.Instance.GetWeaponLevel(weaponType);
            bool isOwned = WeaponLevelTracker.Instance.OwnsWeapon(weaponType);
            
            int purchasePrice = WeaponUpgradeCostCalculator.GetWeaponPurchasePrice(weaponType);
            int nextLevelCost = isOwned && currentLevel < 10 
                ? WeaponUpgradeCostCalculator.CalculateNextLevelCost(weaponType, currentLevel)
                : -1;
            
            int totalInvestment = WeaponUpgradeCostCalculator.CalculateCumulativeCost(weaponType, currentLevel);
            
            string nextUpgradeDescription = "";
            if (isOwned && currentLevel < 10 && upgradeService != null)
            {
                nextUpgradeDescription = upgradeService.GetUpgradeDescription(weaponType, currentLevel + 1);
            }
            
            bool hasUltimate = WeaponLevelTracker.Instance.HasUltimateAbility(weaponType);
            
            return new WeaponShopInfo
            {
                weaponType = weaponType,
                currentLevel = currentLevel,
                isOwned = isOwned,
                purchasePrice = purchasePrice,
                nextLevelCost = nextLevelCost,
                totalInvestment = totalInvestment,
                nextUpgradeDescription = nextUpgradeDescription,
                hasUltimateAbility = hasUltimate
            };
        }
        
        #endregion
        
        #region Consumable Purchase
        
        /// <summary>
        /// Attempts to purchase consumable items
        /// </summary>
        /// <param name="itemType">Type of consumable</param>
        /// <param name="quantity">Quantity to purchase</param>
        /// <returns>True if purchase was successful</returns>
        public bool TryPurchaseConsumable(ConsumableType itemType, int quantity = 1)
        {
            if (!ValidateShopOperation())
                return false;
            
            if (playerInventory == null)
            {
                Debug.LogError("Cannot purchase consumables: PlayerInventory not found");
                return false;
            }
            
            // Get current quantity owned
            int currentQuantity = GetConsumableQuantity(itemType);
            
            // Calculate cost
            int totalCost = quantity == 1
                ? ConsumablePricingService.Instance.GetPrice(itemType, currentQuantity)
                : ConsumablePricingService.Instance.GetBulkPrice(itemType, currentQuantity, quantity);
            
            // Check if player has enough money
            if (!CurrencyManager.Instance.SpendCash(totalCost))
            {
                OnInsufficientFunds?.Invoke();
                if (debugMode)
                    Debug.Log($"Cannot purchase {quantity}x {itemType}: insufficient funds (need ${totalCost})");
                return false;
            }
            
            // Add items to inventory
            AddConsumableToInventory(itemType, quantity);
            
            // Trigger events
            OnConsumablePurchased?.Invoke(itemType, quantity, totalCost);
            
            if (debugMode)
            {
                Debug.Log($"Purchased {quantity}x {itemType} for ${totalCost}");
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets current quantity of a consumable type
        /// </summary>
        /// <param name="itemType">Type of consumable</param>
        /// <returns>Current quantity owned</returns>
        public int GetConsumableQuantity(ConsumableType itemType)
        {
            if (playerInventory == null)
                return 0;
            
            // This would need to be implemented based on how PlayerInventory stores items
            // For now, we'll return a placeholder
            // TODO: Implement actual inventory quantity checking
            return 0;
        }
        
        /// <summary>
        /// Gets information about a consumable for shop display
        /// </summary>
        /// <param name="itemType">Type of consumable</param>
        /// <returns>Consumable shop info</returns>
        public ConsumableShopInfo GetConsumableInfo(ConsumableType itemType)
        {
            int currentQuantity = GetConsumableQuantity(itemType);
            int nextPrice = ConsumablePricingService.Instance.GetPrice(itemType, currentQuantity);
            
            var bulkOption = itemType == ConsumableType.TNT 
                ? ConsumablePricingService.Instance.GetTNTBulkOption()
                : (quantity: 0, totalPrice: 0, savings: 0);
            
            return new ConsumableShopInfo
            {
                itemType = itemType,
                currentQuantity = currentQuantity,
                nextPrice = nextPrice,
                bulkQuantity = bulkOption.quantity,
                bulkPrice = bulkOption.totalPrice,
                bulkSavings = bulkOption.savings
            };
        }
        
        #endregion
        
        #region Shop State Management
        
        /// <summary>
        /// Opens the shop interface
        /// </summary>
        public void OpenShop()
        {
            IsShopOpen = true;
            
            // Pause the game when shop opens
            Time.timeScale = 0f;
            
            OnShopOpened?.Invoke();
            
            if (debugMode)
            {
                Debug.Log("Shop opened");
            }
        }
        
        /// <summary>
        /// Closes the shop interface
        /// </summary>
        public void CloseShop()
        {
            IsShopOpen = false;
            
            // Resume the game when shop closes
            Time.timeScale = 1f;
            
            OnShopClosed?.Invoke();
            
            if (debugMode)
            {
                Debug.Log("Shop closed");
            }
        }
        
        /// <summary>
        /// Toggles shop open/close state
        /// </summary>
        public void ToggleShop()
        {
            if (IsShopOpen)
                CloseShop();
            else
                OpenShop();
        }
        
        /// <summary>
        /// Checks if shop operations are valid (dependencies exist)
        /// </summary>
        /// <returns>True if shop can operate</returns>
        private bool ValidateShopOperation()
        {
            return WeaponLevelTracker.Instance != null && 
                   ConsumablePricingService.Instance != null && 
                   CurrencyManager.Instance != null;
        }
        
        #endregion
        
        #region Private Helper Methods
        
        private void InitializeWeaponAdapter(EWeapons weaponType)
        {
            // This would create or update the weapon adapter with base stats
            // Implementation would depend on integration with existing weapon system
            if (debugMode)
            {
                Debug.Log($"Initialized weapon adapter for {weaponType}");
            }
        }
        
        private void ApplyUpgradeToWeapon(EWeapons weaponType, int level)
        {
            if (upgradeService == null)
                return;
            
            // Get weapon adapter or create one
            if (!weaponAdapters.TryGetValue(weaponType, out var adapter))
            {
                // This would need actual weapon stats from the weapon system
                // For now, it's a placeholder
                if (debugMode)
                {
                    Debug.Log($"Would apply level {level} upgrade to {weaponType}");
                }
                return;
            }
            
            // Apply upgrade
            upgradeService.ApplyUpgrade(weaponType, level, adapter);
            
            // Update weapon ammo to reflect new magazine capacity and extra magazines
            UpdateWeaponAmmo(weaponType);
        }
        
        /// <summary>
        /// Updates weapon ammo based on current stats (magazine capacity and extra magazines)
        /// </summary>
        private void UpdateWeaponAmmo(EWeapons weaponType)
        {
            if (weaponManager == null) return;
            
            // Get current weapon instance
            var weapon = weaponManager.GetWeapon(weaponType);
            if (weapon == null) return;
            
            // Update magazine capacity
            weapon.Ammo.MagazineCapacity = weapon.Stats.MagazineCapacity;
            
            // Calculate total reserve ammo based on extra magazines
            int totalReserveAmmo = weapon.Stats.ExtraMagazines * weapon.Stats.MagazineCapacity;
            
            // Set max ammo capacity for regeneration calculations (LMG/Flamethrower)
            weapon.Ammo.SetMaxAmmoCapacity(totalReserveAmmo);
            
            // Add any new ammo from magazine upgrades (don't reduce existing ammo)
            int currentTotalAmmo = weapon.Ammo.AmmoLeft + weapon.Ammo.CurrentAmmoInMagazine;
            int expectedTotalAmmo = weapon.Stats.MagazineCapacity + totalReserveAmmo; // Current mag + reserves
            
            if (expectedTotalAmmo > currentTotalAmmo)
            {
                int ammoToAdd = expectedTotalAmmo - currentTotalAmmo;
                weapon.Ammo.AddAmmo(ammoToAdd);
            }
            
            // Update UI
            weapon.Ammo.UpdateUI();
            
            if (debugMode)
            {
                Debug.Log($"Updated {weaponType} ammo: {weapon.Stats.MagazineCapacity} mag capacity, {weapon.Stats.ExtraMagazines} extra magazines, {totalReserveAmmo} reserve ammo");
            }
        }
        
        private void ActivateUltimateAbility(EWeapons weaponType)
        {
            if (upgradeService == null)
                return;
            
            var ultimateAbility = upgradeService.GetUltimateAbility(weaponType);
            if (ultimateAbility != null)
            {
                // This would need actual weapon instance
                // For now, it's a placeholder
                if (debugMode)
                {
                    Debug.Log($"Would activate ultimate ability for {weaponType}: {ultimateAbility.Name}");
                }
            }
        }
        
        private void AddConsumableToInventory(ConsumableType itemType, int quantity)
        {
            if (playerInventory == null)
                return;
            
            // This would add items to the actual inventory
            // Implementation depends on existing inventory system
            switch (itemType)
            {
                case ConsumableType.Medkit:
                    var medkit = new Medkit(50f, quantity); // 50% healing
                    playerInventory.AddItem(medkit);
                    break;
                    
                case ConsumableType.TNT:
                    var tnt = new TNT(quantity);
                    playerInventory.AddItem(tnt);
                    break;
            }
        }
        
        private void OnWaveStarted(int waveNumber)
        {
            if (autoRefillAmmo)
            {
                RefillAllAmmo();
            }
        }
        
        private void RefillAllAmmo()
        {
            // This would refill ammo for all owned weapons
            // Implementation depends on existing weapon/ammo system
            if (debugMode)
            {
                Debug.Log("Auto-refilled all weapon ammo");
            }
        }
        
        #endregion
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
            
            // Unsubscribe from events
            if (WaveManager.Instance != null)
            {
                WaveManager.Instance.OnWaveStarted -= OnWaveStarted;
            }
        }
        
        #region Debug Methods
        
        [ContextMenu("Log All Weapon Prices")]
        public void LogAllWeaponPrices()
        {
            foreach (EWeapons weapon in Enum.GetValues(typeof(EWeapons)))
            {
                WeaponUpgradeCostCalculator.LogCostBreakdown(weapon);
            }
        }
        
        [ContextMenu("Log Consumable Prices")]
        public void LogConsumablePrices()
        {
            if (ConsumablePricingService.Instance != null)
            {
                ConsumablePricingService.Instance.LogPriceBreakdown(ConsumableType.Medkit, 0);
                ConsumablePricingService.Instance.LogPriceBreakdown(ConsumableType.TNT, 0);
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// Data structure containing weapon information for shop display
    /// </summary>
    [System.Serializable]
    public struct WeaponShopInfo
    {
        public EWeapons weaponType;
        public int currentLevel;
        public bool isOwned;
        public int purchasePrice;
        public int nextLevelCost;
        public int totalInvestment;
        public string nextUpgradeDescription;
        public bool hasUltimateAbility;
    }
    
    /// <summary>
    /// Data structure containing consumable information for shop display
    /// </summary>
    [System.Serializable]
    public struct ConsumableShopInfo
    {
        public ConsumableType itemType;
        public int currentQuantity;
        public int nextPrice;
        public int bulkQuantity;
        public int bulkPrice;
        public int bulkSavings;
    }
}