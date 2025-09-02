using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Player;

namespace Assets.Scripts.Shop
{
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
                else if (weaponInfo.currentLevel >= 10)
                {
                    // Max level - check for ultimate ability
                    if (weaponInfo.hasUltimateAbility)
                    {
                        descriptionText.text = "ULTIMATE UNLOCKED";
                    }
                    else
                    {
                        descriptionText.text = "Fully upgraded";
                    }
                }
                else if (!string.IsNullOrEmpty(weaponInfo.nextUpgradeDescription))
                {
                    // Use simplified upgrade description if possible, otherwise fall back to formatted version
                    string shortUpgrade = GetSimplifiedUpgradeDescription() ?? FormatUpgradeDescription(weaponInfo.nextUpgradeDescription);
                    descriptionText.text = shortUpgrade;
                }
                else
                {
                    // Fallback - should not happen if upgrade system is working properly
                    descriptionText.text = "Upgrade available";
                }
            }
        }
        
        private string FormatUpgradeDescription(string fullDescription)
        {
            if (string.IsNullOrEmpty(fullDescription))
                return "Upgrade available";

            // Convert long descriptions to short format
            // E.g., "+15% Damage & +5% Fire Rate" -> "+15% DMG & +5% FR"
            string formatted = fullDescription
                .Replace("Damage", "DMG")
                .Replace("Fire Rate", "FR") 
                .Replace("Magazine Capacity", "MAG")
                .Replace("Magazine", "MAG")
                .Replace("Accuracy", "ACC")
                .Replace("Recoil", "REC")
                .Replace("Range", "RNG")
                .Replace("Reload Speed", "RLD")
                .Replace("Reload", "RLD")
                .Replace("Bullet Speed", "BS")
                .Replace("Extra Magazines", "XM")
                .Replace("percent", "%")
                .Replace("Percent", "%");

            // If description is still too long, truncate but keep essential info
            if (formatted.Length > 25)
            {
                // Try to keep the most important part (first upgrade mentioned)
                string[] parts = formatted.Split('&', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 0)
                {
                    formatted = parts[0].Trim();
                    if (parts.Length > 1)
                    {
                        formatted += " & more";
                    }
                }
            }

            return formatted;
        }
        
        /// <summary>
        /// Gets simplified upgrade description using the UpgradeDescriptionCalculator
        /// </summary>
        /// <returns>Simplified description or null if not available</returns>
        private string GetSimplifiedUpgradeDescription()
        {
            if (weaponInfo.isOwned && weaponInfo.currentLevel < 10)
            {
                // Try to get the simplified description from the upgrade service
                var upgradeService = ShopManager.Instance?.GetUpgradeService();
                if (upgradeService != null)
                {
                    return upgradeService.GetSimplifiedUpgradeDescription(weaponInfo.weaponType, weaponInfo.currentLevel + 1);
                }
            }
            
            return null;
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
}