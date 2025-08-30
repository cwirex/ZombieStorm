#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Assets.Scripts.Shop;
using System.Collections.Generic;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Editor utility to create weapon upgrade assets based on NEW_SHOP_INVENTORY.md
    /// Run this from the Unity Editor to generate all required ScriptableObject assets
    /// </summary>
    public class CreateShopAssets : MonoBehaviour
    {
        private static readonly string AssetPath = "Assets/ScriptableObjects/Shop/";
        
        [MenuItem("Tools/Shop/Create All Weapon Upgrade Assets")]
        public static void CreateAllWeaponUpgradeAssets()
        {
            // Ensure directory exists
            EnsureDirectoryExists(AssetPath);
            EnsureDirectoryExists(AssetPath + "Weapons/");
            EnsureDirectoryExists(AssetPath + "Upgrades/");
            EnsureDirectoryExists(AssetPath + "Ultimates/");
            
            CreatePistolUpgrades();
            CreateUziUpgrades();
            CreateShotgunUpgrades();
            CreateM4Upgrades();
            CreateAWPUpgrades();
            CreateLMGUpgrades();
            CreateFlamethrowerUpgrades();
            CreateRPGUpgrades();
            
            AssetDatabase.Refresh();
            Debug.Log("All weapon upgrade assets created successfully!");
        }
        
        private static void EnsureDirectoryExists(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
        
        #region Pistol Upgrades
        private static void CreatePistolUpgrades()
        {
            var upgradePath = CreateWeaponUpgradePath(EWeapons.PISTOL, "Pistol", "The reliable backup you never throw away. Its upgrades make it a surprisingly viable sidearm.");
            
            // Level 2: +10% Damage (33)
            var level2 = CreateWeaponUpgrade(2, 5, "+10% Damage (33)", new StatModifier[]
            {
                new StatModifier 
                { 
                    statType = StatType.Damage, 
                    modifierType = ModifierType.Percentage, 
                    value = 10f, 
                    description = "+10% Damage" 
                }
            });
            
            // Level 3: +2 Magazine Capacity (12)
            var level3 = CreateWeaponUpgrade(3, 8, "+2 Magazine Capacity (12)", new StatModifier[]
            {
                new StatModifier 
                { 
                    statType = StatType.MagazineCapacity, 
                    modifierType = ModifierType.Add, 
                    value = 2f, 
                    description = "+2 Magazine Capacity" 
                }
            });
            
            // Level 4: +10% Fire Rate (2.2)
            var level4 = CreateWeaponUpgrade(4, 12, "+10% Fire Rate (2.2)", new StatModifier[]
            {
                new StatModifier 
                { 
                    statType = StatType.FireRate, 
                    modifierType = ModifierType.Percentage, 
                    value = 10f, 
                    description = "+10% Fire Rate" 
                }
            });
            
            // Level 5: +15% Damage (38), +2 Magazine (14)
            var level5 = CreateWeaponUpgrade(5, 30, "+15% Damage (38), +2 Magazine (14)", new StatModifier[]
            {
                new StatModifier { statType = StatType.Damage, modifierType = ModifierType.Percentage, value = 15f, description = "+15% Damage" },
                new StatModifier { statType = StatType.MagazineCapacity, modifierType = ModifierType.Add, value = 2f, description = "+2 Magazine Capacity" }
            });
            
            // Level 6: +10% Fire Rate (2.42), +1 Reload Speed
            var level6 = CreateWeaponUpgrade(6, 35, "+10% Fire Rate (2.42), +1 Reload Speed", new StatModifier[]
            {
                new StatModifier { statType = StatType.FireRate, modifierType = ModifierType.Percentage, value = 10f, description = "+10% Fire Rate" },
                new StatModifier { statType = StatType.ReloadSpeed, modifierType = ModifierType.Add, value = 1f, description = "+1 Reload Speed" }
            });
            
            // Level 7: +10% Damage (42), +2 Magazine (16)
            var level7 = CreateWeaponUpgrade(7, 40, "+10% Damage (42), +2 Magazine (16)", new StatModifier[]
            {
                new StatModifier { statType = StatType.Damage, modifierType = ModifierType.Percentage, value = 10f, description = "+10% Damage" },
                new StatModifier { statType = StatType.MagazineCapacity, modifierType = ModifierType.Add, value = 2f, description = "+2 Magazine Capacity" }
            });
            
            // Level 8: +10% Fire Rate (2.66), +1 Reload Speed
            var level8 = CreateWeaponUpgrade(8, 45, "+10% Fire Rate (2.66), +1 Reload Speed", new StatModifier[]
            {
                new StatModifier { statType = StatType.FireRate, modifierType = ModifierType.Percentage, value = 10f, description = "+10% Fire Rate" },
                new StatModifier { statType = StatType.ReloadSpeed, modifierType = ModifierType.Add, value = 1f, description = "+1 Reload Speed" }
            });
            
            // Level 9: +15% Damage (48), +4 Magazine (20)
            var level9 = CreateWeaponUpgrade(9, 50, "+15% Damage (48), +4 Magazine (20)", new StatModifier[]
            {
                new StatModifier { statType = StatType.Damage, modifierType = ModifierType.Percentage, value = 15f, description = "+15% Damage" },
                new StatModifier { statType = StatType.MagazineCapacity, modifierType = ModifierType.Add, value = 4f, description = "+4 Magazine Capacity" }
            });
            
            // Level 10: Ultimate - Double Tap: 40% chance each shot fires twice
            var level10 = CreateWeaponUpgrade(10, 75, "Double Tap: 40% chance each shot fires twice (no ammo cost).", new StatModifier[0]);
            var ultimate = CreateUltimateAbility("Double Tap", "40% chance each shot fires twice (no ammo cost).", UltimateAbilityType.DoubleTap, 0.4f);
            
            // Save all assets
            SaveUpgradeAssets("Pistol", new[] { level2, level3, level4, level5, level6, level7, level8, level9, level10 }, ultimate, upgradePath);
        }
        #endregion
        
        #region UZI Upgrades
        private static void CreateUziUpgrades()
        {
            var upgradePath = CreateWeaponUpgradePath(EWeapons.UZI, "UZI (SMG)", "Overwhelm single targets up close and thin out small groups with a torrent of bullets.");
            
            var level2 = CreateWeaponUpgrade(2, 5, "+10% Damage (24.2)", new StatModifier[]
            {
                new StatModifier { statType = StatType.Damage, modifierType = ModifierType.Percentage, value = 10f, description = "+10% Damage" }
            });
            
            var level3 = CreateWeaponUpgrade(3, 8, "+4 Magazine Capacity (28)", new StatModifier[]
            {
                new StatModifier { statType = StatType.MagazineCapacity, modifierType = ModifierType.Add, value = 4f, description = "+4 Magazine Capacity" }
            });
            
            var level4 = CreateWeaponUpgrade(4, 12, "-10% Recoil", new StatModifier[]
            {
                new StatModifier { statType = StatType.Recoil, modifierType = ModifierType.Percentage, value = -10f, description = "-10% Recoil" }
            });
            
            var level5 = CreateWeaponUpgrade(5, 30, "+18% Fire Rate (13.0)", new StatModifier[]
            {
                new StatModifier { statType = StatType.FireRate, modifierType = ModifierType.Percentage, value = 18f, description = "+18% Fire Rate" }
            });
            
            var level6 = CreateWeaponUpgrade(6, 35, "+12% Damage (27.1), +4 Magazine (32)", new StatModifier[]
            {
                new StatModifier { statType = StatType.Damage, modifierType = ModifierType.Percentage, value = 12f, description = "+12% Damage" },
                new StatModifier { statType = StatType.MagazineCapacity, modifierType = ModifierType.Add, value = 4f, description = "+4 Magazine Capacity" }
            });
            
            var level7 = CreateWeaponUpgrade(7, 40, "+12% Fire Rate (14.6), -15% Recoil", new StatModifier[]
            {
                new StatModifier { statType = StatType.FireRate, modifierType = ModifierType.Percentage, value = 12f, description = "+12% Fire Rate" },
                new StatModifier { statType = StatType.Recoil, modifierType = ModifierType.Percentage, value = -15f, description = "-15% Recoil" }
            });
            
            var level8 = CreateWeaponUpgrade(8, 45, "+15% Damage (31.2), +8 Magazine (40)", new StatModifier[]
            {
                new StatModifier { statType = StatType.Damage, modifierType = ModifierType.Percentage, value = 15f, description = "+15% Damage" },
                new StatModifier { statType = StatType.MagazineCapacity, modifierType = ModifierType.Add, value = 8f, description = "+8 Magazine Capacity" }
            });
            
            var level9 = CreateWeaponUpgrade(9, 50, "+10% Fire Rate (16.0), +1 Reload Speed", new StatModifier[]
            {
                new StatModifier { statType = StatType.FireRate, modifierType = ModifierType.Percentage, value = 10f, description = "+10% Fire Rate" },
                new StatModifier { statType = StatType.ReloadSpeed, modifierType = ModifierType.Add, value = 1f, description = "+1 Reload Speed" }
            });
            
            var level10 = CreateWeaponUpgrade(10, 75, "Critical Spray: 25% chance per shot for +100% damage critical hit.", new StatModifier[0]);
            var ultimate = CreateUltimateAbility("Critical Spray", "25% chance per shot for +100% damage critical hit.", UltimateAbilityType.CriticalSpray, 0.25f);
            
            SaveUpgradeAssets("UZI", new[] { level2, level3, level4, level5, level6, level7, level8, level9, level10 }, ultimate, upgradePath);
        }
        #endregion
        
        // Placeholder methods for remaining weapons
        private static void CreateShotgunUpgrades() { /* TODO: Implement complete Shotgun upgrade path */ }
        private static void CreateM4Upgrades() { /* TODO: Implement complete M4 upgrade path */ }
        private static void CreateAWPUpgrades() { /* TODO: Implement complete AWP upgrade path */ }
        private static void CreateLMGUpgrades() { /* TODO: Implement complete LMG upgrade path */ }
        private static void CreateFlamethrowerUpgrades() { /* TODO: Implement complete Flamethrower upgrade path */ }
        private static void CreateRPGUpgrades() { /* TODO: Implement complete RPG upgrade path */ }
        
        #region Helper Methods
        private static WeaponUpgradePathSO CreateWeaponUpgradePath(EWeapons weaponType, string weaponName, string description)
        {
            var path = ScriptableObject.CreateInstance<WeaponUpgradePathSO>();
            path.weaponType = weaponType;
            path.weaponName = weaponName;
            path.weaponDescription = description;
            return path;
        }
        
        private static WeaponUpgradeSO CreateWeaponUpgrade(int level, int costPercentage, string description, StatModifier[] modifiers)
        {
            var upgrade = ScriptableObject.CreateInstance<WeaponUpgradeSO>();
            upgrade.level = level;
            upgrade.costPercentage = costPercentage;
            upgrade.description = description;
            upgrade.statModifiers = modifiers;
            return upgrade;
        }
        
        private static UltimateAbilitySO CreateUltimateAbility(string name, string description, UltimateAbilityType type, float triggerChance)
        {
            var ability = ScriptableObject.CreateInstance<UltimateAbilitySO>();
            ability.abilityName = name;
            ability.description = description;
            ability.abilityType = type;
            ability.triggerChance = triggerChance;
            return ability;
        }
        
        private static void SaveUpgradeAssets(string weaponName, WeaponUpgradeSO[] upgrades, UltimateAbilitySO ultimate, WeaponUpgradePathSO path)
        {
            string weaponPath = AssetPath + "Weapons/" + weaponName + "/";
            EnsureDirectoryExists(weaponPath);
            
            // Save upgrades
            for (int i = 0; i < upgrades.Length; i++)
            {
                AssetDatabase.CreateAsset(upgrades[i], weaponPath + $"{weaponName}_Level{i + 2}.asset");
            }
            
            // Save ultimate
            AssetDatabase.CreateAsset(ultimate, weaponPath + $"{weaponName}_Ultimate.asset");
            
            // Save path
            AssetDatabase.CreateAsset(path, weaponPath + $"{weaponName}_Path.asset");
        }
        #endregion
    }
}
#endif