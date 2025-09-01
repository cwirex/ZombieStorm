using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Player;

namespace Assets.Scripts.Shop
{
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
            
            // Quantity owned with max limit display
            if (quantityText != null)
            {
                int maxQuantity = ConsumablePricingService.Instance.GetMaxQuantity(itemInfo.itemType);
                if (maxQuantity == int.MaxValue)
                {
                    quantityText.text = $"Owned: {itemInfo.currentQuantity}";
                }
                else
                {
                    quantityText.text = $"Owned: {itemInfo.currentQuantity}/{maxQuantity}";
                }
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
                
                // Check both money and inventory capacity
                int maxQuantity = ConsumablePricingService.Instance.GetMaxQuantity(itemInfo.itemType);
                bool hasEnoughMoney = CurrencyManager.Instance.CurrentCash >= itemInfo.nextPrice;
                bool hasInventorySpace = itemInfo.currentQuantity < maxQuantity;
                
                buyButton.interactable = hasEnoughMoney && hasInventorySpace;
                
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
                    
                    // Check both money and inventory capacity for bulk purchase
                    int maxQuantity = ConsumablePricingService.Instance.GetMaxQuantity(itemInfo.itemType);
                    bool hasEnoughMoney = CurrencyManager.Instance.CurrentCash >= itemInfo.bulkPrice;
                    bool hasInventorySpace = itemInfo.currentQuantity + itemInfo.bulkQuantity <= maxQuantity;
                    
                    buyBulkButton.interactable = hasEnoughMoney && hasInventorySpace;
                    
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