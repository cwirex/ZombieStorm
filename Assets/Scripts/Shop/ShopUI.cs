using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Player;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Basic shop UI system that works with the existing UIController
    /// Provides a simple but functional interface for the shop with weapons and consumables on one panel
    /// </summary>
    public class ShopUI : MonoBehaviour
    {
        [Header("Shop UI Panels")]
        [SerializeField] private GameObject shopPanel;
        
        [Header("Navigation")]
        [SerializeField] private Button closeButton;
        
        [Header("Weapons Section")]
        [SerializeField] private Transform weaponListContainer;
        [SerializeField] private GameObject weaponItemPrefab;
        [SerializeField] private TMP_Text weaponsSectionTitle;
        
        [Header("Consumables Section")]
        [SerializeField] private Transform consumableListContainer;
        [SerializeField] private GameObject consumableItemPrefab;
        [SerializeField] private TMP_Text consumablesSectionTitle;
        
        [Header("Header Info")]
        [SerializeField] private TMP_Text cashDisplay;
        [SerializeField] private TMP_Text shopTitle;
        
        [Header("Feedback")]
        [SerializeField] private GameObject insufficientFundsPanel;
        [SerializeField] private float feedbackDuration = 2f;
        
        // Current state
        private List<WeaponShopItem> weaponItems = new();
        private List<ConsumableShopItem> consumableItems = new();
        
        // Static instance for easy access
        public static ShopUI Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeUI();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Subscribe to shop events
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OnShopOpened += ShowShop;
                ShopManager.Instance.OnShopClosed += HideShop;
                ShopManager.Instance.OnWeaponPurchased += OnWeaponPurchased;
                ShopManager.Instance.OnWeaponUpgraded += OnWeaponUpgraded;
                ShopManager.Instance.OnConsumablePurchased += OnConsumablePurchased;
                ShopManager.Instance.OnInsufficientFunds += OnInsufficientFunds;
            }
            
            // Subscribe to currency events
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCashChanged += UpdateCashDisplay;
            }
        }
        
        private void InitializeUI()
        {
            // Set up button listeners
            if (closeButton != null)
                closeButton.onClick.AddListener(CloseShop);
            
            // Initialize panels
            if (shopPanel != null)
                shopPanel.SetActive(false);
            
            if (insufficientFundsPanel != null)
                insufficientFundsPanel.SetActive(false);
            
            // Set section titles
            if (shopTitle != null)
                shopTitle.text = "WEAPON SHOP";
            
            if (weaponsSectionTitle != null)
                weaponsSectionTitle.text = "WEAPONS";
            
            if (consumablesSectionTitle != null)
                consumablesSectionTitle.text = "CONSUMABLES";
        }
        
        #region Shop Display
        
        public void ShowShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(true);
                RefreshShopDisplay();
                UpdateCashDisplay(CurrencyManager.Instance?.CurrentCash ?? 0);
            }
        }
        
        public void HideShop()
        {
            if (shopPanel != null)
            {
                shopPanel.SetActive(false);
            }
        }
        
        public void CloseShop()
        {
            ShopManager.Instance?.CloseShop();
        }
        
        private void RefreshShopDisplay()
        {
            PopulateWeaponsDisplay();
            PopulateConsumablesDisplay();
        }
        
        #endregion
        
        #region Weapons Display
        
        private void PopulateWeaponsDisplay()
        {
            if (weaponListContainer == null || ShopManager.Instance == null)
                return;
            
            // Clear existing items
            ClearWeaponItems();
            
            // Create weapon items for each weapon type
            foreach (EWeapons weaponType in System.Enum.GetValues(typeof(EWeapons)))
            {
                CreateWeaponItem(weaponType);
            }
        }
        
        private void CreateWeaponItem(EWeapons weaponType)
        {
            if (weaponItemPrefab == null || weaponListContainer == null)
                return;
            
            var itemGO = Instantiate(weaponItemPrefab, weaponListContainer);
            var weaponItem = itemGO.GetComponent<WeaponShopItem>();
            
            if (weaponItem != null)
            {
                var weaponInfo = ShopManager.Instance.GetWeaponInfo(weaponType);
                weaponItem.Initialize(weaponInfo, this);
                weaponItems.Add(weaponItem);
            }
        }
        
        private void ClearWeaponItems()
        {
            foreach (var item in weaponItems)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }
            weaponItems.Clear();
        }
        
        #endregion
        
        #region Consumables Display
        
        private void PopulateConsumablesDisplay()
        {
            if (consumableListContainer == null || ShopManager.Instance == null)
                return;
            
            // Clear existing items
            ClearConsumableItems();
            
            // Create items for each consumable type
            foreach (ConsumableType itemType in System.Enum.GetValues(typeof(ConsumableType)))
            {
                CreateConsumableItem(itemType);
            }
        }
        
        private void CreateConsumableItem(ConsumableType itemType)
        {
            if (consumableItemPrefab == null || consumableListContainer == null)
                return;
            
            var itemGO = Instantiate(consumableItemPrefab, consumableListContainer);
            var consumableItem = itemGO.GetComponent<ConsumableShopItem>();
            
            if (consumableItem != null)
            {
                var itemInfo = ShopManager.Instance.GetConsumableInfo(itemType);
                consumableItem.Initialize(itemInfo, this);
                consumableItems.Add(consumableItem);
            }
        }
        
        private void ClearConsumableItems()
        {
            foreach (var item in consumableItems)
            {
                if (item != null && item.gameObject != null)
                {
                    Destroy(item.gameObject);
                }
            }
            consumableItems.Clear();
        }
        
        #endregion
        
        #region Purchase Actions
        
        public void OnWeaponPurchaseClicked(EWeapons weaponType)
        {
            ShopManager.Instance?.TryPurchaseWeapon(weaponType);
        }
        
        public void OnWeaponUpgradeClicked(EWeapons weaponType)
        {
            ShopManager.Instance?.TryUpgradeWeapon(weaponType);
        }
        
        public void OnConsumablePurchaseClicked(ConsumableType itemType, int quantity = 1)
        {
            ShopManager.Instance?.TryPurchaseConsumable(itemType, quantity);
        }
        
        #endregion
        
        #region Event Handlers
        
        private void OnWeaponPurchased(EWeapons weaponType, int level, int cost)
        {
            // Refresh weapons display to show new status
            PopulateWeaponsDisplay();
        }
        
        private void OnWeaponUpgraded(EWeapons weaponType, int newLevel, int cost)
        {
            // Refresh weapons display to show new level
            PopulateWeaponsDisplay();
        }
        
        private void OnConsumablePurchased(ConsumableType itemType, int quantity, int cost)
        {
            // Refresh consumables display to show new quantities/prices
            PopulateConsumablesDisplay();
        }
        
        private void OnInsufficientFunds()
        {
            ShowInsufficientFundsMessage();
        }
        
        private void UpdateCashDisplay(int currentCash)
        {
            if (cashDisplay != null)
            {
                cashDisplay.text = $"${currentCash}";
            }
        }
        
        #endregion
        
        #region Feedback
        
        private void ShowInsufficientFundsMessage()
        {
            if (insufficientFundsPanel != null)
            {
                insufficientFundsPanel.SetActive(true);
                CancelInvoke(nameof(HideInsufficientFundsMessage));
                Invoke(nameof(HideInsufficientFundsMessage), feedbackDuration);
            }
        }
        
        private void HideInsufficientFundsMessage()
        {
            if (insufficientFundsPanel != null)
            {
                insufficientFundsPanel.SetActive(false);
            }
        }
        
        #endregion
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.OnShopOpened -= ShowShop;
                ShopManager.Instance.OnShopClosed -= HideShop;
                ShopManager.Instance.OnWeaponPurchased -= OnWeaponPurchased;
                ShopManager.Instance.OnWeaponUpgraded -= OnWeaponUpgraded;
                ShopManager.Instance.OnConsumablePurchased -= OnConsumablePurchased;
                ShopManager.Instance.OnInsufficientFunds -= OnInsufficientFunds;
            }
            
            if (CurrencyManager.Instance != null)
            {
                CurrencyManager.Instance.OnCashChanged -= UpdateCashDisplay;
            }
            
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}