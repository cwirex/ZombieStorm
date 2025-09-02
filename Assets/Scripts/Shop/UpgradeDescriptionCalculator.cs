using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Calculates simplified upgrade descriptions by converting technical stats into Power/Ammo categories
    /// </summary>
    public static class UpgradeDescriptionCalculator
    {
        /// <summary>
        /// Contains simplified upgrade information
        /// </summary>
        public class SimplifiedUpgradeInfo
        {
            public float PowerValue { get; set; }
            public int AmmoValue { get; set; }
            public List<string> SpecialUpgrades { get; set; }
            
            public SimplifiedUpgradeInfo()
            {
                SpecialUpgrades = new List<string>();
            }
            
            public bool HasPower => PowerValue > 0;
            public bool HasAmmo => AmmoValue > 0;
            public bool HasSpecial => SpecialUpgrades.Count > 0;
            public int TotalUpgradeCount => (HasPower ? 1 : 0) + (HasAmmo ? 1 : 0) + SpecialUpgrades.Count;
        }
        
        /// <summary>
        /// Base magazine sizes for ammo calculations
        /// </summary>
        private static readonly Dictionary<EWeapons, int> BaseMagazineSizes = new Dictionary<EWeapons, int>
        {
            { EWeapons.PISTOL, 10 },
            { EWeapons.UZI, 24 },
            { EWeapons.SHOTGUN, 7 },
            { EWeapons.M4, 30 },
            { EWeapons.AWP, 5 },
            { EWeapons.M249, 100 },
            { EWeapons.FLAMETHROWER, 100 },
            { EWeapons.RPG7, 4 }
        };
        
        /// <summary>
        /// Converts StatModifier array into simplified Power/Ammo description
        /// </summary>
        public static string GenerateSimplifiedDescription(StatModifier[] modifiers, EWeapons weaponType)
        {
            if (modifiers == null || modifiers.Length == 0)
                return "Upgrade available";
                
            var info = CalculateSimplifiedInfo(modifiers, weaponType);
            return FormatDescription(info);
        }
        
        /// <summary>
        /// Calculates simplified upgrade info from stat modifiers
        /// </summary>
        private static SimplifiedUpgradeInfo CalculateSimplifiedInfo(StatModifier[] modifiers, EWeapons weaponType)
        {
            var info = new SimplifiedUpgradeInfo();
            
            foreach (var modifier in modifiers)
            {
                switch (modifier.statType)
                {
                    // Power-related stats (combat effectiveness)
                    case StatType.Damage:
                        info.PowerValue += modifier.value * 1.0f; // Damage has full weight
                        break;
                    case StatType.FireRate:
                        info.PowerValue += modifier.value * 0.8f; // Fire rate slightly less impact
                        break;
                    case StatType.ReloadSpeed:
                        info.PowerValue += modifier.value * 10f; // Flat reload speed bonus (convert to percentage equivalent)
                        break;
                    case StatType.Accuracy:
                        info.PowerValue += modifier.value * 0.5f; // Accuracy improvements
                        break;
                    case StatType.Recoil:
                        info.PowerValue += Mathf.Abs(modifier.value) * 0.5f; // Recoil reduction (use absolute value)
                        break;
                    
                    // Ammo-related stats
                    case StatType.MagazineCapacity:
                        info.AmmoValue += (int)modifier.value;
                        break;
                    case StatType.ExtraMagazines:
                        // Convert extra magazines to total ammo
                        int baseMagSize = GetBaseMagazineSize(weaponType);
                        info.AmmoValue += (int)(modifier.value * baseMagSize);
                        break;
                    
                    // Special cases that remain as-is
                    case StatType.PelletCount:
                        info.SpecialUpgrades.Add($"+{(int)modifier.value} Pellet per Shot");
                        break;
                    case StatType.FlameWidth:
                        info.SpecialUpgrades.Add($"+{modifier.value}% Flame Width");
                        break;
                    case StatType.BlastRadius:
                        info.SpecialUpgrades.Add($"+{modifier.value}% Blast Radius");
                        break;
                    case StatType.Range:
                        info.SpecialUpgrades.Add($"+{modifier.value}% Range");
                        break;
                    case StatType.ProjectileSpeed:
                        info.SpecialUpgrades.Add($"+{modifier.value}% Projectile Speed");
                        break;
                    case StatType.Piercing:
                        info.SpecialUpgrades.Add("Bullet Piercing");
                        break;
                    case StatType.BulletSpeed:
                        info.SpecialUpgrades.Add($"+{modifier.value}% Bullet Speed");
                        break;
                }
            }
            
            // Round power value to nearest integer for cleaner display
            info.PowerValue = Mathf.Round(info.PowerValue);
            
            return info;
        }
        
        /// <summary>
        /// Formats the simplified upgrade info into display string
        /// </summary>
        private static string FormatDescription(SimplifiedUpgradeInfo info)
        {
            var parts = new List<string>();
            
            // Add Power if present
            if (info.HasPower)
            {
                parts.Add($"+{info.PowerValue} Power");
            }
            
            // Add Ammo if present
            if (info.HasAmmo)
            {
                parts.Add($"+{info.AmmoValue} Ammo");
            }
            
            // Add special upgrades
            parts.AddRange(info.SpecialUpgrades);
            
            if (parts.Count == 0)
                return "Upgrade available";
            
            // If 2+ upgrades, add newline after first one for multi-row display
            if (parts.Count >= 2)
            {
                return parts[0] + "\n" + string.Join(", ", parts.Skip(1));
            }
            
            return string.Join(", ", parts);
        }
        
        /// <summary>
        /// Gets base magazine size for ammo calculations
        /// </summary>
        private static int GetBaseMagazineSize(EWeapons weaponType)
        {
            return BaseMagazineSizes.TryGetValue(weaponType, out int size) ? size : 30; // Default to 30
        }
        
        /// <summary>
        /// For debugging - shows both original and simplified descriptions
        /// </summary>
        public static string GetDebugComparison(StatModifier[] modifiers, EWeapons weaponType, string originalDescription)
        {
            string simplified = GenerateSimplifiedDescription(modifiers, weaponType);
            return $"Original: {originalDescription}\nSimplified: {simplified}";
        }
    }
}