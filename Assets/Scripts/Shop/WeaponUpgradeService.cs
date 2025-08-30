using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Service that handles applying weapon upgrades.
    /// Single Responsibility: Apply upgrades to weapon stats.
    /// Uses Dependency Injection for the repository.
    /// </summary>
    public class WeaponUpgradeService
    {
        private readonly IWeaponUpgradeRepository repository;
        private readonly bool debugMode;
        
        public WeaponUpgradeService(IWeaponUpgradeRepository repository, bool debugMode = false)
        {
            this.repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
            this.debugMode = debugMode;
        }
        
        /// <summary>
        /// Applies a specific level upgrade to weapon stats
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <param name="level">Level to apply (1-10)</param>
        /// <param name="stats">Stats to modify</param>
        /// <returns>True if upgrade was applied successfully</returns>
        public bool ApplyUpgrade(EWeapons weaponType, int level, Assets.Scripts.Weapon.IWeaponStats stats)
        {
            if (stats == null)
            {
                if (debugMode)
                    Debug.LogError("Cannot apply upgrade: stats is null");
                return false;
            }
            
            if (level < 1 || level > 10)
            {
                if (debugMode)
                    Debug.LogError($"Invalid upgrade level: {level}. Must be 1-10.");
                return false;
            }
            
            var upgradePath = repository.GetUpgradePath(weaponType);
            if (upgradePath == null)
            {
                if (debugMode)
                    Debug.LogError($"No upgrade path found for weapon: {weaponType}");
                return false;
            }
            
            var upgrade = upgradePath.GetUpgradeForLevel(level);
            if (upgrade == null)
            {
                if (debugMode)
                    Debug.LogError($"No upgrade found for {weaponType} level {level}");
                return false;
            }
            
            try
            {
                upgrade.ApplyTo(stats);
                
                if (debugMode)
                {
                    Debug.Log($"Applied {weaponType} level {level} upgrade: {upgrade.GetDescription()}");
                }
                
                return true;
            }
            catch (System.Exception ex)
            {
                if (debugMode)
                    Debug.LogError($"Error applying upgrade: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Applies all upgrades from level 1 to target level
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <param name="targetLevel">Target level (1-10)</param>
        /// <param name="stats">Stats to modify</param>
        /// <returns>True if all upgrades were applied successfully</returns>
        public bool ApplyAllUpgrades(EWeapons weaponType, int targetLevel, Assets.Scripts.Weapon.IWeaponStats stats)
        {
            if (targetLevel < 1 || targetLevel > 10)
            {
                if (debugMode)
                    Debug.LogError($"Invalid target level: {targetLevel}. Must be 1-10.");
                return false;
            }
            
            bool success = true;
            
            // Apply upgrades from level 2 to target level (level 1 is just purchasing the weapon)
            for (int level = 2; level <= targetLevel; level++)
            {
                if (!ApplyUpgrade(weaponType, level, stats))
                {
                    success = false;
                    break;
                }
            }
            
            if (debugMode && success)
            {
                Debug.Log($"Applied all upgrades for {weaponType} up to level {targetLevel}");
            }
            
            return success;
        }
        
        /// <summary>
        /// Gets the description for a specific upgrade level
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <param name="level">Level to get description for</param>
        /// <returns>Upgrade description or empty string if not found</returns>
        public string GetUpgradeDescription(EWeapons weaponType, int level)
        {
            var upgradePath = repository.GetUpgradePath(weaponType);
            if (upgradePath == null)
                return string.Empty;
            
            var upgrade = upgradePath.GetUpgradeForLevel(level);
            return upgrade?.GetDescription() ?? string.Empty;
        }
        
        /// <summary>
        /// Gets all upgrade descriptions for a weapon
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>Dictionary of level -> description</returns>
        public Dictionary<int, string> GetAllUpgradeDescriptions(EWeapons weaponType)
        {
            var descriptions = new Dictionary<int, string>();
            
            var upgradePath = repository.GetUpgradePath(weaponType);
            if (upgradePath == null)
                return descriptions;
            
            for (int level = 1; level <= upgradePath.GetMaxLevel(); level++)
            {
                var upgrade = upgradePath.GetUpgradeForLevel(level);
                if (upgrade != null)
                {
                    descriptions[level] = upgrade.GetDescription();
                }
            }
            
            return descriptions;
        }
        
        /// <summary>
        /// Checks if a weapon has an ultimate ability (level 10)
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>True if weapon has ultimate ability</returns>
        public bool HasUltimateAbility(EWeapons weaponType)
        {
            var upgradePath = repository.GetUpgradePath(weaponType);
            return upgradePath?.HasUltimateAbility() ?? false;
        }
        
        /// <summary>
        /// Gets the ultimate ability for a weapon
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <returns>Ultimate ability or null if not found</returns>
        public IUltimateAbility GetUltimateAbility(EWeapons weaponType)
        {
            var upgradePath = repository.GetUpgradePath(weaponType);
            return upgradePath?.GetUltimateAbility();
        }
        
        /// <summary>
        /// Activates ultimate ability for a weapon (if at level 10)
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <param name="weapon">Weapon instance to activate ability on</param>
        /// <param name="currentLevel">Current weapon level</param>
        /// <returns>True if ability was activated</returns>
        public bool ActivateUltimateAbility(EWeapons weaponType, IWeapon weapon, int currentLevel)
        {
            if (currentLevel < 10)
            {
                if (debugMode)
                    Debug.Log($"Cannot activate ultimate ability: {weaponType} is only level {currentLevel}");
                return false;
            }
            
            var ultimateAbility = GetUltimateAbility(weaponType);
            if (ultimateAbility == null)
            {
                if (debugMode)
                    Debug.LogError($"No ultimate ability found for {weaponType}");
                return false;
            }
            
            if (weapon == null)
            {
                if (debugMode)
                    Debug.LogError("Cannot activate ultimate ability: weapon is null");
                return false;
            }
            
            try
            {
                ultimateAbility.Activate(weapon);
                
                if (debugMode)
                {
                    Debug.Log($"Activated ultimate ability for {weaponType}: {ultimateAbility.Name}");
                }
                
                return true;
            }
            catch (System.Exception ex)
            {
                if (debugMode)
                    Debug.LogError($"Error activating ultimate ability: {ex.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Deactivates ultimate ability for a weapon
        /// </summary>
        /// <param name="weaponType">Type of weapon</param>
        /// <param name="weapon">Weapon instance to deactivate ability on</param>
        /// <returns>True if ability was deactivated</returns>
        public bool DeactivateUltimateAbility(EWeapons weaponType, IWeapon weapon)
        {
            var ultimateAbility = GetUltimateAbility(weaponType);
            if (ultimateAbility == null)
                return false;
            
            try
            {
                ultimateAbility.Deactivate(weapon);
                
                if (debugMode)
                {
                    Debug.Log($"Deactivated ultimate ability for {weaponType}: {ultimateAbility.Name}");
                }
                
                return true;
            }
            catch (System.Exception ex)
            {
                if (debugMode)
                    Debug.LogError($"Error deactivating ultimate ability: {ex.Message}");
                return false;
            }
        }
    }
}