using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Calculates weapon upgrade costs based on the universal cost formula.
    /// Single Responsibility: Only calculates upgrade costs.
    /// </summary>
    public class WeaponUpgradeCostCalculator
    {
        // Base weapon prices according to our new pricing structure
        private static readonly Dictionary<EWeapons, int> BaseWeaponPrices = new()
        {
            { EWeapons.PISTOL, 200 },      // Special case: free weapon, but upgrades cost as if $200
            { EWeapons.UZI, 300 },
            { EWeapons.SHOTGUN, 400 },
            { EWeapons.FLAMETHROWER, 700 },
            { EWeapons.M4, 800 },
            { EWeapons.AWP, 1500 },
            { EWeapons.M249, 1800 },
            { EWeapons.RPG7, 2000 }
        };
        
        // Universal cost percentages for each level
        private static readonly Dictionary<int, float> LevelCostPercentages = new()
        {
            { 1, 1.00f },   // 100% - weapon purchase (except pistol which is free)
            { 2, 0.05f },   // 5% of base price
            { 3, 0.08f },   // 8% of base price
            { 4, 0.12f },   // 12% of base price
            { 5, 0.30f },   // 30% of base price - Power Spike
            { 6, 0.35f },   // 35% of base price
            { 7, 0.40f },   // 40% of base price
            { 8, 0.45f },   // 45% of base price
            { 9, 0.50f },   // 50% of base price
            { 10, 0.75f }   // 75% of base price - Ultimate Ability
        };
        
        /// <summary>
        /// Gets the base purchase price for a weapon
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>Base purchase price (0 for pistol, since it's free)</returns>
        public static int GetWeaponPurchasePrice(EWeapons weaponType)
        {
            if (weaponType == EWeapons.PISTOL)
                return 0; // Pistol is free
            
            return BaseWeaponPrices.GetValueOrDefault(weaponType, 100);
        }
        
        /// <summary>
        /// Gets the base price used for upgrade calculations (pistol uses $200 for calculations)
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>Base price for upgrade calculations</returns>
        public static int GetWeaponBasePrice(EWeapons weaponType)
        {
            return BaseWeaponPrices.GetValueOrDefault(weaponType, 100);
        }
        
        /// <summary>
        /// Calculates the cost to upgrade to a specific level
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <param name="targetLevel">Level to upgrade to (1-10)</param>
        /// <returns>Cost to reach that level, or -1 if invalid</returns>
        public static int CalculateLevelCost(EWeapons weaponType, int targetLevel)
        {
            if (targetLevel < 1 || targetLevel > 10)
            {
                Debug.LogError($"Invalid target level: {targetLevel}. Must be 1-10.");
                return -1;
            }
            
            // Level 1 is weapon purchase
            if (targetLevel == 1)
            {
                return GetWeaponPurchasePrice(weaponType);
            }
            
            // Get base price for calculations
            int basePrice = GetWeaponBasePrice(weaponType);
            
            // Get percentage for this level
            float percentage = LevelCostPercentages.GetValueOrDefault(targetLevel, 0f);
            
            // Calculate and return cost
            return Mathf.RoundToInt(basePrice * percentage);
        }
        
        /// <summary>
        /// Calculates the cumulative cost to reach a specific level from level 1
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <param name="targetLevel">Level to reach (1-10)</param>
        /// <returns>Total cost from level 1 to target level</returns>
        public static int CalculateCumulativeCost(EWeapons weaponType, int targetLevel)
        {
            if (targetLevel < 1 || targetLevel > 10)
            {
                Debug.LogError($"Invalid target level: {targetLevel}. Must be 1-10.");
                return -1;
            }
            
            int totalCost = 0;
            
            // Add costs for all levels from 1 to target level
            for (int level = 1; level <= targetLevel; level++)
            {
                totalCost += CalculateLevelCost(weaponType, level);
            }
            
            return totalCost;
        }
        
        /// <summary>
        /// Calculates the cost to upgrade from current level to next level
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <param name="currentLevel">Current weapon level (0-9)</param>
        /// <returns>Cost to upgrade to next level, or -1 if invalid/maxed</returns>
        public static int CalculateNextLevelCost(EWeapons weaponType, int currentLevel)
        {
            if (currentLevel < 0 || currentLevel >= 10)
            {
                Debug.LogWarning($"Cannot upgrade {weaponType}: current level {currentLevel} is invalid or maxed out.");
                return -1;
            }
            
            int nextLevel = currentLevel + 1;
            return CalculateLevelCost(weaponType, nextLevel);
        }
        
        /// <summary>
        /// Gets a breakdown of all upgrade costs for a weapon
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>Dictionary of level -> cost</returns>
        public static Dictionary<int, int> GetUpgradeCostBreakdown(EWeapons weaponType)
        {
            var breakdown = new Dictionary<int, int>();
            
            for (int level = 1; level <= 10; level++)
            {
                breakdown[level] = CalculateLevelCost(weaponType, level);
            }
            
            return breakdown;
        }
        
        /// <summary>
        /// Gets a breakdown of cumulative costs for a weapon
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>Dictionary of level -> cumulative cost</returns>
        public static Dictionary<int, int> GetCumulativeCostBreakdown(EWeapons weaponType)
        {
            var breakdown = new Dictionary<int, int>();
            int cumulativeCost = 0;
            
            for (int level = 1; level <= 10; level++)
            {
                cumulativeCost += CalculateLevelCost(weaponType, level);
                breakdown[level] = cumulativeCost;
            }
            
            return breakdown;
        }
        
        /// <summary>
        /// Validates if a weapon type has valid pricing data
        /// </summary>
        /// <param name="weaponType">Type of weapon to validate</param>
        /// <returns>True if weapon has valid pricing</returns>
        public static bool IsValidWeapon(EWeapons weaponType)
        {
            return BaseWeaponPrices.ContainsKey(weaponType);
        }
        
        /// <summary>
        /// Gets the total investment cost for a weapon (purchase + all upgrades)
        /// This should equal 400% of base price for all weapons
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>Total cost to fully upgrade weapon</returns>
        public static int GetTotalInvestmentCost(EWeapons weaponType)
        {
            return CalculateCumulativeCost(weaponType, 10);
        }
        
        /// <summary>
        /// Gets the cost just for upgrades (excluding weapon purchase)
        /// This should equal 300% of base price for all weapons
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>Total upgrade cost (levels 2-10)</returns>
        public static int GetTotalUpgradeCost(EWeapons weaponType)
        {
            int purchaseCost = CalculateLevelCost(weaponType, 1);
            int totalCost = GetTotalInvestmentCost(weaponType);
            return totalCost - purchaseCost;
        }
        
        /// <summary>
        /// For debugging - logs cost breakdown for a weapon
        /// </summary>
        /// <param name="weaponType">Weapon to analyze</param>
        public static void LogCostBreakdown(EWeapons weaponType)
        {
            if (!IsValidWeapon(weaponType))
            {
                Debug.LogError($"Invalid weapon type: {weaponType}");
                return;
            }
            
            Debug.Log($"=== {weaponType} Cost Breakdown ===");
            Debug.Log($"Base Price: ${GetWeaponBasePrice(weaponType)}");
            Debug.Log($"Purchase Price: ${GetWeaponPurchasePrice(weaponType)}");
            
            var breakdown = GetUpgradeCostBreakdown(weaponType);
            var cumulative = GetCumulativeCostBreakdown(weaponType);
            
            foreach (var level in breakdown.Keys)
            {
                string levelType = level switch
                {
                    1 => "Purchase",
                    5 => "Power Spike",
                    10 => "Ultimate",
                    _ => "Upgrade"
                };
                
                Debug.Log($"Level {level} ({levelType}): ${breakdown[level]} (Cumulative: ${cumulative[level]})");
            }
            
            Debug.Log($"Total Investment: ${GetTotalInvestmentCost(weaponType)}");
            Debug.Log($"Total Upgrades: ${GetTotalUpgradeCost(weaponType)}");
        }
    }
}