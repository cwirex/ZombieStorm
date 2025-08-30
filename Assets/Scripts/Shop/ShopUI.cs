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
    
    /// <summary>
    /// Individual weapon item in the shop display
    /// </summary>
    public class WeaponShopItem : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text lvlText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Button actionButton;
        
        [Header("Weapon Icons - Assign weapon sprites here")]
        [SerializeField] private Sprite pistolIcon;
        [SerializeField] private Sprite uziIcon;
        [SerializeField] private Sprite shotgunIcon;
        [SerializeField] private Sprite flamethrowerIcon;
        [SerializeField] private Sprite m4Icon;
        [SerializeField] private Sprite awpIcon;
        [SerializeField] private Sprite m249Icon;
        [SerializeField] private Sprite rpgIcon;
        
        private WeaponShopInfo weaponInfo;
        private ShopUI shopUI;
        
        public void Initialize(WeaponShopInfo info, ShopUI parentShopUI)
        {
            weaponInfo = info;
            shopUI = parentShopUI;
            
            UpdateDisplay();
            SetupButton();
        }
        
        private void UpdateDisplay()
        {
            // Set weapon icon based on type
            if (icon != null)
            {
                icon.sprite = weaponInfo.weaponType switch
                {
                    EWeapons.PISTOL => pistolIcon,
                    EWeapons.UZI => uziIcon,
                    EWeapons.SHOTGUN => shotgunIcon,
                    EWeapons.FLAMETHROWER => flamethrowerIcon,
                    EWeapons.M4 => m4Icon,
                    EWeapons.AWP => awpIcon,
                    EWeapons.M249 => m249Icon,
                    EWeapons.RPG7 => rpgIcon,
                    _ => null
                };
            }
            
            // Weapon name (clean format)
            if (nameText != null)
            {
                string cleanName = weaponInfo.weaponType switch
                {
                    EWeapons.UZI => "UZI",
                    EWeapons.M4 => "M4 Rifle",
                    EWeapons.AWP => "AWP Sniper",
                    EWeapons.M249 => "M249 LMG",
                    EWeapons.RPG7 => "RPG-7",
                    _ => weaponInfo.weaponType.ToString().Replace("_", " ")
                };
                nameText.text = cleanName;
            }
            
            // Level display (e.g., "Lvl 10")
            if (lvlText != null)
            {
                if (weaponInfo.isOwned)
                {
                    lvlText.text = $"Lvl {weaponInfo.currentLevel}";
                    if (weaponInfo.currentLevel >= 10)
                    {
                        lvlText.text = "Lvl 10 (MAX)";
                    }
                }
                else
                {
                    lvlText.text = "Not Owned";
                }
            }
            
            // Price display
            if (priceText != null)
            {
                if (!weaponInfo.isOwned)
                {
                    priceText.text = weaponInfo.purchasePrice == 0 ? "FREE" : $"${weaponInfo.purchasePrice}";
                }
                else if (weaponInfo.nextLevelCost > 0)
                {
                    priceText.text = $"${weaponInfo.nextLevelCost}";
                }
                else
                {
                    priceText.text = "MAXED";
                }
            }
            
            // Short description for current upgrade or weapon info
            if (descriptionText != null)
            {
                if (!weaponInfo.isOwned)
                {
                    // Very short description before unlocking
                    string shortDesc = weaponInfo.weaponType switch
                    {
                        EWeapons.PISTOL => "Default sidearm",
                        EWeapons.UZI => "High fire rate SMG",
                        EWeapons.SHOTGUN => "Close-range powerhouse", 
                        EWeapons.FLAMETHROWER => "Area denial weapon",
                        EWeapons.M4 => "Versatile assault rifle",
                        EWeapons.AWP => "One-shot sniper rifle",
                        EWeapons.M249 => "Heavy machine gun",
                        EWeapons.RPG7 => "Explosive launcher",
                        _ => "Powerful weapon"
                    };
                    descriptionText.text = shortDesc;
                }
                else if (!string.IsNullOrEmpty(weaponInfo.nextUpgradeDescription))
                {
                    // Format upgrade description to be shorter (e.g., "+10% DMG")
                    string shortUpgrade = FormatUpgradeDescription(weaponInfo.nextUpgradeDescription);
                    descriptionText.text = shortUpgrade;
                }
                else if (weaponInfo.hasUltimateAbility)
                {
                    descriptionText.text = "ULTIMATE UNLOCKED";
                }
                else
                {
                    descriptionText.text = "Fully upgraded";
                }
            }
        }
        
        private string FormatUpgradeDescription(string fullDescription)
        {
            // Convert long descriptions to short format
            // E.g., "+15% Damage & +5% Fire Rate" -> "+15% DMG & +5% FR"
            return fullDescription
                .Replace("Damage", "DMG")
                .Replace("Fire Rate", "FR")
                .Replace("Magazine Capacity", "MAG")
                .Replace("Accuracy", "ACC")
                .Replace("Recoil", "REC")
                .Replace("Range", "RNG")
                .Replace("Reload Speed", "RLD");
        }
        
        private void SetupButton()
        {
            if (actionButton == null)
                return;
            
            // Remove previous listeners
            actionButton.onClick.RemoveAllListeners();
            
            // Get button text component
            var buttonText = actionButton.GetComponentInChildren<TMP_Text>();
            
            if (!weaponInfo.isOwned)
            {
                // Purchase button
                actionButton.onClick.AddListener(() => shopUI.OnWeaponPurchaseClicked(weaponInfo.weaponType));
                actionButton.interactable = CurrencyManager.Instance.CurrentCash >= weaponInfo.purchasePrice;
                
                if (buttonText != null)
                {
                    buttonText.text = "BUY";
                }
            }
            else if (weaponInfo.nextLevelCost > 0)
            {
                // Upgrade button
                actionButton.onClick.AddListener(() => shopUI.OnWeaponUpgradeClicked(weaponInfo.weaponType));
                actionButton.interactable = CurrencyManager.Instance.CurrentCash >= weaponInfo.nextLevelCost;
                
                if (buttonText != null)
                {
                    buttonText.text = "UPGRADE";
                }
            }
            else
            {
                // Maxed out
                actionButton.interactable = false;
                
                if (buttonText != null)
                {
                    buttonText.text = "MAX";
                }
            }
        }
    }
    
    /// <summary>
    /// Individual consumable item in the shop display
    /// </summary>
    public class ConsumableShopItem : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text quantityText;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private TMP_Text bulkPriceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button buyBulkButton;
        
        [Header("Item Icons")]
        [SerializeField] private Sprite medkitIcon;
        [SerializeField] private Sprite tntIcon;
        
        private ConsumableShopInfo itemInfo;
        private ShopUI shopUI;
        
        public void Initialize(ConsumableShopInfo info, ShopUI parentShopUI)
        {
            itemInfo = info;
            shopUI = parentShopUI;
            
            UpdateDisplay();
            SetupButtons();
        }
        
        private void UpdateDisplay()
        {
            // Set icon based on item type
            if (icon != null)
            {
                icon.sprite = itemInfo.itemType switch
                {
                    ConsumableType.Medkit => medkitIcon,
                    ConsumableType.TNT => tntIcon,
                    _ => null
                };
            }
            
            // Item name
            if (nameText != null)
            {
                nameText.text = itemInfo.itemType.ToString();
            }
            
            // Quantity owned
            if (quantityText != null)
            {
                quantityText.text = $"Owned: {itemInfo.currentQuantity}";
            }
            
            // Single purchase price
            if (priceText != null)
            {
                priceText.text = $"${itemInfo.nextPrice}";
            }
            
            // Bulk purchase option
            if (bulkPriceText != null && itemInfo.bulkQuantity > 0)
            {
                bulkPriceText.text = $"{itemInfo.bulkQuantity}x for ${itemInfo.bulkPrice}";
                if (itemInfo.bulkSavings > 0)
                {
                    bulkPriceText.text += $" (Save ${itemInfo.bulkSavings})";
                }
            }
            else if (bulkPriceText != null)
            {
                bulkPriceText.text = "";
            }
        }
        
        private void SetupButtons()
        {
            // Single purchase button
            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() => shopUI.OnConsumablePurchaseClicked(itemInfo.itemType, 1));
                buyButton.interactable = CurrencyManager.Instance.CurrentCash >= itemInfo.nextPrice;
                
                // Set button text to simply "BUY"
                var buttonText = buyButton.GetComponentInChildren<TMP_Text>();
                if (buttonText != null)
                {
                    buttonText.text = "BUY";
                }
            }
            
            // Bulk purchase button
            if (buyBulkButton != null)
            {
                if (itemInfo.bulkQuantity > 0)
                {
                    buyBulkButton.gameObject.SetActive(true);
                    buyBulkButton.onClick.RemoveAllListeners();
                    buyBulkButton.onClick.AddListener(() => shopUI.OnConsumablePurchaseClicked(itemInfo.itemType, itemInfo.bulkQuantity));
                    buyBulkButton.interactable = CurrencyManager.Instance.CurrentCash >= itemInfo.bulkPrice;
                    
                    // Set bulk button text (e.g., "x10")
                    var buttonText = buyBulkButton.GetComponentInChildren<TMP_Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = $"x{itemInfo.bulkQuantity}";
                    }
                }
                else
                {
                    buyBulkButton.gameObject.SetActive(false);
                }
            }
        }
    }
}