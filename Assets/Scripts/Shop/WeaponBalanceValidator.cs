using UnityEngine;
using Assets.Scripts.Weapon;
using System.Text;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Utility to validate weapon balance according to NEW_SHOP_INVENTORY.md specifications
    /// Tests DPS progression to ensure 100-150% increase targets are met
    /// </summary>
    public class WeaponBalanceValidator : MonoBehaviour
    {
        [Header("Debug Options")]
        [SerializeField] private bool logDetailedBreakdown = true;
        
        [ContextMenu("Validate All Weapon Balance")]
        public void ValidateAllWeaponBalance()
        {
            Debug.Log("=== WEAPON BALANCE VALIDATION ===");
            Debug.Log("Testing DPS progression for all weapons according to NEW_SHOP_INVENTORY.md");
            
            ValidateWeaponDPS(EWeapons.PISTOL, "Pistol", 60f, 147f, "Double Tap (40% chance)");
            ValidateWeaponDPS(EWeapons.UZI, "UZI (SMG)", 242f, 624f, "Critical Spray (25% chance)");
            ValidateWeaponDPS(EWeapons.SHOTGUN, "Shotgun", 200f, 432f, "Extra Pellets"); // Burst damage
            ValidateWeaponDPS(EWeapons.M4, "M4 Rifle", 385f, 696f, "Armor Piercer (1% max HP)");
            ValidateWeaponDPS(EWeapons.M249, "M249 (LMG)", 480f, 761f, "Ammunition Expert (4% ammo regen)");
            ValidateWeaponDPS(EWeapons.FLAMETHROWER, "Flamethrower", 400f, 1138f, "Sustained Burn (+75% max)");
            
            Debug.Log("=== COST VALIDATION ===");
            ValidateWeaponCosts();
            
            Debug.Log("=== VALIDATION COMPLETE ===");
        }
        
        private void ValidateWeaponDPS(EWeapons weaponType, string name, float baseDPS, float maxDPS, string ultimateAbility)
        {
            WeaponStats baseStats = GetBaseStats(weaponType);
            float actualBaseDPS = baseStats.CalculateDPS();
            float dpsIncrease = (maxDPS / actualBaseDPS - 1f) * 100f;
            
            bool inTargetRange = dpsIncrease >= 100f && dpsIncrease <= 150f;
            string status = inTargetRange ? "✓ PASS" : "✗ FAIL";
            string color = inTargetRange ? "<color=green>" : "<color=red>";
            
            Debug.Log($"{color}{status}</color> {name}: {actualBaseDPS:F0} → {maxDPS:F0} DPS ({dpsIncrease:F1}% increase) | Ultimate: {ultimateAbility}");
            
            if (logDetailedBreakdown)
            {
                Debug.Log($"   Base Stats: {baseStats.Damage} dmg × {baseStats.FireRate} FR = {actualBaseDPS:F0} DPS");
                Debug.Log($"   Target Range: 100-150% increase | Actual: {dpsIncrease:F1}%");
            }
        }
        
        private void ValidateWeaponCosts()
        {
            foreach (EWeapons weapon in System.Enum.GetValues(typeof(EWeapons)))
            {
                if (WeaponUpgradeCostCalculator.IsValidWeapon(weapon))
                {
                    int basePrice = WeaponUpgradeCostCalculator.GetWeaponBasePrice(weapon);
                    int totalUpgrades = WeaponUpgradeCostCalculator.GetTotalUpgradeCost(weapon);
                    int totalInvestment = WeaponUpgradeCostCalculator.GetTotalInvestmentCost(weapon);
                    
                    float upgradePercentage = (float)totalUpgrades / basePrice * 100f;
                    float investmentPercentage = (float)totalInvestment / basePrice * 100f;
                    
                    // Should be ~300% for upgrades, ~400% for total investment
                    bool upgradesValid = Mathf.Approximately(upgradePercentage, 300f);
                    bool investmentValid = Mathf.Approximately(investmentPercentage, 400f);
                    
                    string upgradeStatus = upgradesValid ? "✓" : "✗";
                    string investmentStatus = investmentValid ? "✓" : "✗";
                    
                    Debug.Log($"{weapon}: Base ${basePrice} | Upgrades ${totalUpgrades} ({upgradePercentage:F0}%) {upgradeStatus} | Total ${totalInvestment} ({investmentPercentage:F0}%) {investmentStatus}");
                }
            }
        }
        
        private WeaponStats GetBaseStats(EWeapons weaponType)
        {
            return weaponType switch
            {
                EWeapons.PISTOL => WeaponStatsRepository.Pistol(),
                EWeapons.UZI => WeaponStatsRepository.SMG(),
                EWeapons.SHOTGUN => WeaponStatsRepository.Shotgun(),
                EWeapons.M4 => WeaponStatsRepository.Rifle(),
                EWeapons.AWP => WeaponStatsRepository.SniperRifle(),
                EWeapons.M249 => WeaponStatsRepository.M249(),
                EWeapons.RPG7 => WeaponStatsRepository.RPG(),
                EWeapons.FLAMETHROWER => WeaponStatsRepository.Flamethrower(),
                _ => WeaponStatsRepository.Pistol()
            };
        }
        
        [ContextMenu("Log Weapon Stats")]
        public void LogAllWeaponStats()
        {
            Debug.Log("=== CURRENT WEAPON BASE STATS ===");
            
            foreach (EWeapons weapon in System.Enum.GetValues(typeof(EWeapons)))
            {
                WeaponStats stats = GetBaseStats(weapon);
                float dps = stats.CalculateDPS();
                
                Debug.Log($"{weapon}: {stats.Damage} dmg × {stats.FireRate} FR × {stats.MagazineCapacity} mag = {dps:F0} DPS");
            }
        }
        
        [ContextMenu("Test Upgrade System")]
        public void TestUpgradeSystem()
        {
            Debug.Log("=== TESTING UPGRADE SYSTEM ===");
            
            // Test upgrading a pistol to show the system works
            var repository = FindObjectOfType<WeaponUpgradeRepositorySO>();
            if (repository == null)
            {
                Debug.LogError("WeaponUpgradeRepositorySO not found in scene!");
                return;
            }
            
            var upgradeService = new WeaponUpgradeService(repository, true);
            var pistolStats = new WeaponStats(WeaponStatsRepository.Pistol());
            
            Debug.Log($"Pistol Base DPS: {pistolStats.CalculateDPS():F0}");
            
            // Apply upgrades level by level
            for (int level = 2; level <= 10; level++)
            {
                bool success = upgradeService.ApplyUpgrade(EWeapons.PISTOL, level, pistolStats);
                if (success)
                {
                    Debug.Log($"Level {level}: {pistolStats.CalculateDPS():F0} DPS ({pistolStats.Damage:F1} dmg × {pistolStats.FireRate:F1} FR)");
                }
                else
                {
                    Debug.LogWarning($"Failed to apply level {level} upgrade");
                }
            }
            
            Debug.Log($"Final DPS: {pistolStats.CalculateDPS():F0} (Expected ~147 with ultimate ability)");
        }
    }
}