#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Assets.Scripts.Shop;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Fully automated weapon upgrade generation system
    /// Generates all weapon paths from data definitions based on NEW_SHOP_INVENTORY.md
    /// </summary>
    public class AutomatedWeaponUpgradeGenerator : MonoBehaviour
    {
        private static readonly string AssetPath = "Assets/ScriptableObjects/Shop/";
        
        [MenuItem("Tools/Shop/Generate All Weapon Upgrades (Automated)")]
        public static void GenerateAllWeaponUpgrades()
        {
            Debug.Log("🤖 Starting automated weapon upgrade generation...");
            
            EnsureDirectoriesExist();
            var weaponDefinitions = GetAllWeaponDefinitions();
            
            // Validate definitions first
            ValidateWeaponDefinitions(weaponDefinitions);
            
            var createdPaths = new List<WeaponUpgradePathSO>();
            
            foreach (var weaponDef in weaponDefinitions)
            {
                try
                {
                    Debug.Log($"⚡ Generating {weaponDef.Name} upgrade path...");
                    var path = GenerateWeaponPath(weaponDef);
                    createdPaths.Add(path);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"❌ Failed to generate {weaponDef.Name}: {ex.Message}");
                }
            }
            
            CreateWeaponRepository(createdPaths);
            AssetDatabase.Refresh();
            
            Debug.Log($"✅ Successfully generated {createdPaths.Count} complete weapon upgrade paths!");
        }
        
        [MenuItem("Tools/Shop/Validate Weapon Definitions")]
        public static void ValidateWeaponDefinitionsMenu()
        {
            var weaponDefinitions = GetAllWeaponDefinitions();
            ValidateWeaponDefinitions(weaponDefinitions);
        }
        
        private static void ValidateWeaponDefinitions(WeaponDefinition[] definitions)
        {
            Debug.Log("🔍 Validating weapon definitions...");
            
            foreach (var weaponDef in definitions)
            {
                // Check basic structure
                if (string.IsNullOrEmpty(weaponDef.Name))
                {
                    Debug.LogError($"❌ {weaponDef.WeaponType}: Missing weapon name");
                    continue;
                }
                
                if (weaponDef.Upgrades == null || weaponDef.Upgrades.Length != 9)
                {
                    Debug.LogError($"❌ {weaponDef.Name}: Expected 9 upgrades (levels 2-10), got {weaponDef.Upgrades?.Length ?? 0}");
                    continue;
                }
                
                if (weaponDef.Ultimate == null)
                {
                    Debug.LogError($"❌ {weaponDef.Name}: Missing ultimate ability definition");
                    continue;
                }
                
                // Validate upgrade levels
                for (int i = 0; i < weaponDef.Upgrades.Length; i++)
                {
                    var upgrade = weaponDef.Upgrades[i];
                    int expectedLevel = i + 2; // Levels 2-10
                    
                    if (upgrade.Level != expectedLevel)
                    {
                        Debug.LogWarning($"⚠️ {weaponDef.Name}: Upgrade at index {i} has level {upgrade.Level}, expected {expectedLevel}");
                    }
                    
                    if (string.IsNullOrEmpty(upgrade.Description))
                    {
                        Debug.LogWarning($"⚠️ {weaponDef.Name} Level {upgrade.Level}: Missing description");
                    }
                }
                
                Debug.Log($"✅ {weaponDef.Name}: Definition validated successfully");
            }
            
            Debug.Log($"🔍 Validation complete for {definitions.Length} weapon definitions");
        }
        
        private static void EnsureDirectoriesExist()
        {
            var directories = new[] { 
                AssetPath, 
                AssetPath + "Weapons/", 
                AssetPath + "Upgrades/", 
                AssetPath + "Ultimates/" 
            };
            
            foreach (var dir in directories)
            {
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);
            }
        }
        
        #region Weapon Data Definitions
        
        /// <summary>
        /// Complete weapon definition from NEW_SHOP_INVENTORY.md
        /// </summary>
        private class WeaponDefinition
        {
            public EWeapons WeaponType { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int BasePrice { get; set; }
            public UpgradeLevel[] Upgrades { get; set; }
            public UltimateDefinition Ultimate { get; set; }
        }
        
        private class UpgradeLevel
        {
            public int Level { get; set; }
            public string Description { get; set; }
            public StatChange[] StatChanges { get; set; }
        }
        
        private class StatChange
        {
            public StatType StatType { get; set; }
            public ModifierType ModifierType { get; set; }
            public float Value { get; set; }
            public string Description { get; set; }
        }
        
        private class UltimateDefinition
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public UltimateAbilityType Type { get; set; }
            public float TriggerChance { get; set; }
        }
        
        /// <summary>
        /// All weapon definitions from NEW_SHOP_INVENTORY.md
        /// </summary>
        private static WeaponDefinition[] GetAllWeaponDefinitions()
        {
            return new WeaponDefinition[]
            {
                // PISTOL - 60 → 147 DPS (145% increase)
                new WeaponDefinition
                {
                    WeaponType = EWeapons.PISTOL,
                    Name = "Pistol",
                    Description = "The reliable backup you never throw away. Its upgrades make it a surprisingly viable sidearm.",
                    BasePrice = 0,
                    Upgrades = new UpgradeLevel[]
                    {
                        new UpgradeLevel { Level = 2, Description = "+10% Damage (33)", StatChanges = new[] { Stat(StatType.Damage, ModifierType.Percentage, 10f, "+10% Damage") } },
                        new UpgradeLevel { Level = 3, Description = "+2 Magazine Capacity (12), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 2f, "+2 Magazine"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 4, Description = "+10% Fire Rate (2.2)", StatChanges = new[] { Stat(StatType.FireRate, ModifierType.Percentage, 10f, "+10% Fire Rate") } },
                        new UpgradeLevel { Level = 5, Description = "+15% Damage (38), +2 Magazine (14)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 15f, "+15% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 2f, "+2 Magazine") 
                        }},
                        new UpgradeLevel { Level = 6, Description = "+10% Fire Rate (2.42), +1 Reload Speed", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 10f, "+10% Fire Rate"),
                            Stat(StatType.ReloadSpeed, ModifierType.Add, 1f, "+1 Reload Speed") 
                        }},
                        new UpgradeLevel { Level = 7, Description = "+10% Damage (42), +2 Magazine (16), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 10f, "+10% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 2f, "+2 Magazine"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        }},
                        new UpgradeLevel { Level = 8, Description = "+10% Fire Rate (2.66), +1 Reload Speed", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 10f, "+10% Fire Rate"),
                            Stat(StatType.ReloadSpeed, ModifierType.Add, 1f, "+1 Reload Speed") 
                        }},
                        new UpgradeLevel { Level = 9, Description = "+15% Damage (48), +4 Magazine (20)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 15f, "+15% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 4f, "+4 Magazine") 
                        }},
                        new UpgradeLevel { Level = 10, Description = "Double Tap: 40% chance each shot fires twice", StatChanges = new StatChange[0] }
                    },
                    Ultimate = new UltimateDefinition { Name = "Double Tap", Description = "40% chance each shot fires twice (no ammo cost)", Type = UltimateAbilityType.DoubleTap, TriggerChance = 0.4f }
                },
                
                // UZI - 242 → 624 DPS (158% increase)
                new WeaponDefinition
                {
                    WeaponType = EWeapons.UZI,
                    Name = "UZI (SMG)", 
                    Description = "Overwhelm single targets up close and thin out small groups with a torrent of bullets.",
                    BasePrice = 300,
                    Upgrades = new UpgradeLevel[]
                    {
                        new UpgradeLevel { Level = 2, Description = "+10% Damage (24.2), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 10f, "+10% Damage"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 3, Description = "+4 Magazine Capacity (28)", StatChanges = new[] { Stat(StatType.MagazineCapacity, ModifierType.Add, 4f, "+4 Magazine") } },
                        new UpgradeLevel { Level = 4, Description = "-10% Recoil, +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.Recoil, ModifierType.Percentage, -10f, "-10% Recoil"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 5, Description = "+18% Fire Rate (13.0)", StatChanges = new[] { Stat(StatType.FireRate, ModifierType.Percentage, 18f, "+18% Fire Rate") } },
                        new UpgradeLevel { Level = 6, Description = "+12% Damage (27.1), +4 Magazine (32)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 12f, "+12% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 4f, "+4 Magazine") 
                        }},
                        new UpgradeLevel { Level = 7, Description = "+12% Fire Rate (14.6), -15% Recoil", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 12f, "+12% Fire Rate"),
                            Stat(StatType.Recoil, ModifierType.Percentage, -15f, "-15% Recoil") 
                        }},
                        new UpgradeLevel { Level = 8, Description = "+15% Damage (31.2), +8 Magazine (40), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 15f, "+15% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 8f, "+8 Magazine"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        }},
                        new UpgradeLevel { Level = 9, Description = "+10% Fire Rate (16.0), +1 Reload Speed", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 10f, "+10% Fire Rate"),
                            Stat(StatType.ReloadSpeed, ModifierType.Add, 1f, "+1 Reload Speed") 
                        }},
                        new UpgradeLevel { Level = 10, Description = "Critical Spray: 25% chance per shot for +100% damage", StatChanges = new StatChange[0] }
                    },
                    Ultimate = new UltimateDefinition { Name = "Critical Spray", Description = "25% chance per shot for +100% damage critical hit", Type = UltimateAbilityType.CriticalSpray, TriggerChance = 0.25f }
                },
                
                // SHOTGUN
                new WeaponDefinition
                {
                    WeaponType = EWeapons.SHOTGUN,
                    Name = "Shotgun",
                    Description = "Annihilate anything that gets too close with massive burst damage.",
                    BasePrice = 400,
                    Upgrades = new UpgradeLevel[]
                    {
                        new UpgradeLevel { Level = 2, Description = "+2 Magazine Capacity (9), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 2f, "+2 Magazine"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 3, Description = "+10% Fire Rate (0.94)", StatChanges = new[] { Stat(StatType.FireRate, ModifierType.Percentage, 10f, "+10% Fire Rate") } },
                        new UpgradeLevel { Level = 4, Description = "+1 Pellet per Shot (9), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.PelletCount, ModifierType.Add, 1f, "+1 Pellet"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 5, Description = "+20% Damage (30), Tighter Spread", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 20f, "+20% Damage"),
                            Stat(StatType.Accuracy, ModifierType.Percentage, 15f, "Tighter Spread") 
                        }},
                        new UpgradeLevel { Level = 6, Description = "+15% Fire Rate (1.08), +3 Magazine (12)", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 15f, "+15% Fire Rate"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 3f, "+3 Magazine") 
                        }},
                        new UpgradeLevel { Level = 7, Description = "+1 Pellet per Shot (10)", StatChanges = new[] { Stat(StatType.PelletCount, ModifierType.Add, 1f, "+1 Pellet") } },
                        new UpgradeLevel { Level = 8, Description = "+20% Damage (36), +1 Reload Speed, +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 20f, "+20% Damage"),
                            Stat(StatType.ReloadSpeed, ModifierType.Add, 1f, "+1 Reload Speed"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        }},
                        new UpgradeLevel { Level = 9, Description = "+15% Fire Rate (1.24), Tighter Spread", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 15f, "+15% Fire Rate"),
                            Stat(StatType.Accuracy, ModifierType.Percentage, 10f, "Tighter Spread") 
                        }},
                        new UpgradeLevel { Level = 10, Description = "Overwhelm Shells: Fires +2 additional pellets per shot", StatChanges = new StatChange[0] }
                    },
                    Ultimate = new UltimateDefinition { Name = "Overwhelm Shells", Description = "Fires +2 additional pellets per shot (12 total)", Type = UltimateAbilityType.ExtraPellets, TriggerChance = 1.0f }
                },
                
                // M4 RIFLE
                new WeaponDefinition
                {
                    WeaponType = EWeapons.M4,
                    Name = "M4 Rifle",
                    Description = "The jack-of-all-trades. Reliable, accurate, and effective against strong enemies.",
                    BasePrice = 800,
                    Upgrades = new UpgradeLevel[]
                    {
                        new UpgradeLevel { Level = 2, Description = "+8% Damage (59.4), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 8f, "+8% Damage"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 3, Description = "+8% Fire Rate (7.56)", StatChanges = new[] { Stat(StatType.FireRate, ModifierType.Percentage, 8f, "+8% Fire Rate") } },
                        new UpgradeLevel { Level = 4, Description = "+5 Magazine Capacity (35), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 5f, "+5 Magazine"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 5, Description = "+12% Damage (66.5) & +8% Fire Rate (8.16)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 12f, "+12% Damage"),
                            Stat(StatType.FireRate, ModifierType.Percentage, 8f, "+8% Fire Rate") 
                        }},
                        new UpgradeLevel { Level = 6, Description = "+10% Accuracy & -10% Recoil", StatChanges = new[] { 
                            Stat(StatType.Accuracy, ModifierType.Percentage, 10f, "+10% Accuracy"),
                            Stat(StatType.Recoil, ModifierType.Percentage, -10f, "-10% Recoil") 
                        }},
                        new UpgradeLevel { Level = 7, Description = "+8% Fire Rate (8.81) & +5 Magazine (40)", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 8f, "+8% Fire Rate"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 5f, "+5 Magazine") 
                        }},
                        new UpgradeLevel { Level = 8, Description = "+10% Damage (73.2) & +10% Accuracy, +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 10f, "+10% Damage"),
                            Stat(StatType.Accuracy, ModifierType.Percentage, 10f, "+10% Accuracy"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        }},
                        new UpgradeLevel { Level = 9, Description = "+8% Fire Rate (9.51) & -10% Recoil", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 8f, "+8% Fire Rate"),
                            Stat(StatType.Recoil, ModifierType.Percentage, -10f, "-10% Recoil") 
                        }},
                        new UpgradeLevel { Level = 10, Description = "Armor Piercer: Each shot deals +1% of target's max health", StatChanges = new StatChange[0] }
                    },
                    Ultimate = new UltimateDefinition { Name = "Armor Piercer", Description = "Each shot deals +1% of target's max health as bonus damage", Type = UltimateAbilityType.ArmorPiercer, TriggerChance = 1.0f }
                },
                
                // AWP SNIPER
                new WeaponDefinition
                {
                    WeaponType = EWeapons.AWP,
                    Name = "AWP Sniper",
                    Description = "One shot, one (or more) kills. A high-skill weapon that deletes priority targets.",
                    BasePrice = 1500,
                    Upgrades = new UpgradeLevel[]
                    {
                        new UpgradeLevel { Level = 2, Description = "+2 Magazine Capacity (7), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 2f, "+2 Magazine"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 3, Description = "+10% Damage (440)", StatChanges = new[] { Stat(StatType.Damage, ModifierType.Percentage, 10f, "+10% Damage") } },
                        new UpgradeLevel { Level = 4, Description = "+15% Fire Rate (0.46)", StatChanges = new[] { Stat(StatType.FireRate, ModifierType.Percentage, 15f, "+15% Fire Rate") } },
                        new UpgradeLevel { Level = 5, Description = "+25% Damage (550), +1 Magazine (8)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 25f, "+25% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 1f, "+1 Magazine") 
                        }},
                        new UpgradeLevel { Level = 6, Description = "+15% Fire Rate (0.53), +1 Reload Speed", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 15f, "+15% Fire Rate"),
                            Stat(StatType.ReloadSpeed, ModifierType.Add, 1f, "+1 Reload Speed") 
                        }},
                        new UpgradeLevel { Level = 7, Description = "+25% Damage (687), +2 Magazine (10)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 25f, "+25% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 2f, "+2 Magazine") 
                        }},
                        new UpgradeLevel { Level = 8, Description = "+15% Fire Rate (0.61), +1 Reload Speed, +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 15f, "+15% Fire Rate"),
                            Stat(StatType.ReloadSpeed, ModifierType.Add, 1f, "+1 Reload Speed"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        }},
                        new UpgradeLevel { Level = 9, Description = "+25% Damage (859), +5 Magazine (15)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 25f, "+25% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 5f, "+5 Magazine") 
                        }},
                        new UpgradeLevel { Level = 10, Description = "Executioner's Mark: Bonus damage = 25% of target's missing health", StatChanges = new StatChange[0] }
                    },
                    Ultimate = new UltimateDefinition { Name = "Executioner's Mark", Description = "Shots deal bonus damage equal to 25% of a target's missing health", Type = UltimateAbilityType.ExecutionersMark, TriggerChance = 1.0f }
                },
                
                // M249 LMG
                new WeaponDefinition
                {
                    WeaponType = EWeapons.M249,
                    Name = "M249 (LMG)",
                    Description = "Lay down a relentless wall of lead. Perfect for suppressing large areas and melting bosses.",
                    BasePrice = 1800,
                    Upgrades = new UpgradeLevel[]
                    {
                        new UpgradeLevel { Level = 2, Description = "+10 Magazine (110), +1% ammo regeneration on kill, +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 10f, "+10 Magazine"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 3, Description = "+8% Damage (43.2)", StatChanges = new[] { Stat(StatType.Damage, ModifierType.Percentage, 8f, "+8% Damage") } },
                        new UpgradeLevel { Level = 4, Description = "-10% Recoil while firing, +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.Recoil, ModifierType.Percentage, -10f, "-10% Recoil"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 5, Description = "+8% Fire Rate (13.2), +15 Magazine (125)", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 8f, "+8% Fire Rate"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 15f, "+15 Magazine") 
                        }},
                        new UpgradeLevel { Level = 6, Description = "+8% Damage (46.7), -15% Recoil", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 8f, "+8% Damage"),
                            Stat(StatType.Recoil, ModifierType.Percentage, -15f, "-15% Recoil") 
                        }},
                        new UpgradeLevel { Level = 7, Description = "+8% Fire Rate (14.0), +25 Magazine (150), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 8f, "+8% Fire Rate"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 25f, "+25 Magazine"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        }},
                        new UpgradeLevel { Level = 8, Description = "+8% Damage (50.4), Bullets gain minor piercing", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 8f, "+8% Damage"),
                            Stat(StatType.Piercing, ModifierType.Add, 1f, "Bullet Piercing") 
                        }},
                        new UpgradeLevel { Level = 9, Description = "+8% Fire Rate (15.1), +50 Magazine (200)", StatChanges = new[] { 
                            Stat(StatType.FireRate, ModifierType.Percentage, 8f, "+8% Fire Rate"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 50f, "+50 Magazine") 
                        }},
                        new UpgradeLevel { Level = 10, Description = "Ammunition Expert: +3% ammo regeneration on kill (Total: 4%)", StatChanges = new StatChange[0] }
                    },
                    Ultimate = new UltimateDefinition { Name = "Ammunition Expert", Description = "+3% ammo regeneration on kill (Total: 4%)", Type = UltimateAbilityType.AmmunitionExpert, TriggerChance = 1.0f }
                },
                
                // FLAMETHROWER
                new WeaponDefinition
                {
                    WeaponType = EWeapons.FLAMETHROWER,
                    Name = "Flamethrower",
                    Description = "Control space and roast hordes of weaker enemies. Sustained fire becomes increasingly deadly.",
                    BasePrice = 700,
                    Upgrades = new UpgradeLevel[]
                    {
                        new UpgradeLevel { Level = 2, Description = "+50 Max Fuel (150), +1% ammo regeneration on kill, +1 Extra Canister", StatChanges = new[] { 
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 50f, "+50 Fuel"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Canister")
                        } },
                        new UpgradeLevel { Level = 3, Description = "+10% Damage (22)", StatChanges = new[] { Stat(StatType.Damage, ModifierType.Percentage, 10f, "+10% Damage") } },
                        new UpgradeLevel { Level = 4, Description = "+10% Flame Width, +25 Max Fuel (175), +1 Extra Canister", StatChanges = new[] { 
                            Stat(StatType.FlameWidth, ModifierType.Percentage, 10f, "+10% Flame Width"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 25f, "+25 Fuel"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Canister")
                        }},
                        new UpgradeLevel { Level = 5, Description = "+15% Damage (25.3), +50 Max Fuel (225)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 15f, "+15% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 50f, "+50 Fuel") 
                        }},
                        new UpgradeLevel { Level = 6, Description = "+15% Flame Range, +15% Flame Width", StatChanges = new[] { 
                            Stat(StatType.Range, ModifierType.Percentage, 15f, "+15% Range"),
                            Stat(StatType.FlameWidth, ModifierType.Percentage, 15f, "+15% Flame Width") 
                        }},
                        new UpgradeLevel { Level = 7, Description = "+12% Damage (28.3), +75 Max Fuel (300), +1 Extra Canister", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 12f, "+12% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 75f, "+75 Fuel"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Canister")
                        }},
                        new UpgradeLevel { Level = 8, Description = "+10% Flame Range, +25 Max Fuel (325)", StatChanges = new[] { 
                            Stat(StatType.Range, ModifierType.Percentage, 10f, "+10% Range"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 25f, "+25 Fuel") 
                        }},
                        new UpgradeLevel { Level = 9, Description = "+15% Damage (32.5), +20% Flame Range", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 15f, "+15% Damage"),
                            Stat(StatType.Range, ModifierType.Percentage, 20f, "+20% Range") 
                        }},
                        new UpgradeLevel { Level = 10, Description = "Sustained Burn: Damage increases by 3% per second (Max +75%)", StatChanges = new StatChange[0] }
                    },
                    Ultimate = new UltimateDefinition { Name = "Sustained Burn", Description = "Damage increases by 3% for every second of continuous fire (Max +75%)", Type = UltimateAbilityType.SustainedBurn, TriggerChance = 1.0f }
                },
                
                // RPG-7
                new WeaponDefinition
                {
                    WeaponType = EWeapons.RPG7,
                    Name = "RPG-7",
                    Description = "Erase entire groups of enemies from existence. Very limited ammo, very big boom.",
                    BasePrice = 2000,
                    Upgrades = new UpgradeLevel[]
                    {
                        new UpgradeLevel { Level = 2, Description = "+1 Max Rocket (5), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 1f, "+1 Rocket"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 3, Description = "+10% Blast Radius (3.3m), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.BlastRadius, ModifierType.Percentage, 10f, "+10% Blast Radius"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 4, Description = "+10% Damage (330), +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 10f, "+10% Damage"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        } },
                        new UpgradeLevel { Level = 5, Description = "+25% Damage (412), +1 Max Rocket (6)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 25f, "+25% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 1f, "+1 Rocket") 
                        }},
                        new UpgradeLevel { Level = 6, Description = "+20% Blast Radius (4m), +1 Reload Speed", StatChanges = new[] { 
                            Stat(StatType.BlastRadius, ModifierType.Percentage, 20f, "+20% Blast Radius"),
                            Stat(StatType.ReloadSpeed, ModifierType.Add, 1f, "+1 Reload Speed") 
                        }},
                        new UpgradeLevel { Level = 7, Description = "+25% Damage (515), +1 Max Rocket (7)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 25f, "+25% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 1f, "+1 Rocket") 
                        }},
                        new UpgradeLevel { Level = 8, Description = "+20% Projectile Speed, +1 Reload Speed, +1 Extra Magazine", StatChanges = new[] { 
                            Stat(StatType.ProjectileSpeed, ModifierType.Percentage, 20f, "+20% Projectile Speed"),
                            Stat(StatType.ReloadSpeed, ModifierType.Add, 1f, "+1 Reload Speed"),
                            Stat(StatType.ExtraMagazines, ModifierType.Add, 1f, "+1 Extra Magazine")
                        }},
                        new UpgradeLevel { Level = 9, Description = "+25% Damage (644), +1 Max Rocket (8)", StatChanges = new[] { 
                            Stat(StatType.Damage, ModifierType.Percentage, 25f, "+25% Damage"),
                            Stat(StatType.MagazineCapacity, ModifierType.Add, 1f, "+1 Rocket") 
                        }},
                        new UpgradeLevel { Level = 10, Description = "Thermobaric Warhead: Blast radius +25% and stuns enemies", StatChanges = new StatChange[0] }
                    },
                    Ultimate = new UltimateDefinition { Name = "Thermobaric Warhead", Description = "Blast radius +25% (5m) and briefly stuns all enemies hit", Type = UltimateAbilityType.ThermobaricBlast, TriggerChance = 1.0f }
                }
            };
        }
        
        /// <summary>
        /// Helper method to create stat changes more easily
        /// </summary>
        private static StatChange Stat(StatType statType, ModifierType modifierType, float value, string description)
        {
            return new StatChange 
            { 
                StatType = statType, 
                ModifierType = modifierType, 
                Value = value, 
                Description = description 
            };
        }
        
        #endregion
        
        #region Generation Logic
        
        private static WeaponUpgradePathSO GenerateWeaponPath(WeaponDefinition weaponDef)
        {
            var path = ScriptableObject.CreateInstance<WeaponUpgradePathSO>();
            path.weaponType = weaponDef.WeaponType;
            path.weaponName = weaponDef.Name;
            path.weaponDescription = weaponDef.Description;
            
            // Generate all upgrades
            var upgrades = new WeaponUpgradeSO[10];
            for (int i = 0; i < weaponDef.Upgrades.Length; i++)
            {
                upgrades[i] = GenerateUpgrade(weaponDef.Upgrades[i]);
            }
            
            // Generate ultimate ability
            var ultimate = GenerateUltimateAbility(weaponDef.Ultimate);
            
            // Use reflection to set the private arrays
            var upgradesField = typeof(WeaponUpgradePathSO).GetField("upgrades", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var ultimateField = typeof(WeaponUpgradePathSO).GetField("ultimateAbility", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (upgradesField != null)
                upgradesField.SetValue(path, upgrades);
            else
                Debug.LogError($"Could not find upgrades field in WeaponUpgradePathSO for {weaponDef.Name}");
                
            if (ultimateField != null)
                ultimateField.SetValue(path, ultimate);
            else
                Debug.LogError($"Could not find ultimateAbility field in WeaponUpgradePathSO for {weaponDef.Name}");
            
            // Save assets
            SaveWeaponAssets(weaponDef.Name, path, upgrades, ultimate);
            
            return path;
        }
        
        private static WeaponUpgradeSO GenerateUpgrade(UpgradeLevel upgradeLevel)
        {
            var upgrade = ScriptableObject.CreateInstance<WeaponUpgradeSO>();
            upgrade.level = upgradeLevel.Level;
            upgrade.costPercentage = GetCostPercentage(upgradeLevel.Level);
            upgrade.description = upgradeLevel.Description;
            
            // Convert StatChanges to StatModifiers
            upgrade.statModifiers = upgradeLevel.StatChanges.Select(sc => new StatModifier
            {
                statType = sc.StatType,
                modifierType = sc.ModifierType,
                value = sc.Value,
                description = sc.Description
            }).ToArray();
            
            return upgrade;
        }
        
        private static UltimateAbilitySO GenerateUltimateAbility(UltimateDefinition ultimateDef)
        {
            var ultimate = ScriptableObject.CreateInstance<UltimateAbilitySO>();
            ultimate.abilityName = ultimateDef.Name;
            ultimate.description = ultimateDef.Description;
            ultimate.abilityType = ultimateDef.Type;
            ultimate.triggerChance = ultimateDef.TriggerChance;
            
            return ultimate;
        }
        
        /// <summary>
        /// Universal cost percentages from NEW_SHOP_INVENTORY.md
        /// </summary>
        private static int GetCostPercentage(int level)
        {
            return level switch
            {
                1 => 100, // Weapon purchase
                2 => 5,
                3 => 8,
                4 => 12,
                5 => 30,  // Power spike
                6 => 35,
                7 => 40,
                8 => 45,
                9 => 50,
                10 => 75, // Ultimate ability
                _ => 0
            };
        }
        
        private static void SaveWeaponAssets(string weaponName, WeaponUpgradePathSO path, WeaponUpgradeSO[] upgrades, UltimateAbilitySO ultimate)
        {
            string weaponPath = AssetPath + "Weapons/" + weaponName.Replace(" ", "") + "/";
            if (!System.IO.Directory.Exists(weaponPath))
                System.IO.Directory.CreateDirectory(weaponPath);
            
            // Save upgrades (levels 2-10)
            for (int i = 0; i < upgrades.Length; i++)
            {
                if (upgrades[i] != null)
                {
                    AssetDatabase.CreateAsset(upgrades[i], weaponPath + $"{weaponName.Replace(" ", "")}_Level{i + 2}.asset");
                }
            }
            
            // Save ultimate
            AssetDatabase.CreateAsset(ultimate, weaponPath + $"{weaponName.Replace(" ", "")}_Ultimate.asset");
            
            // Save path
            AssetDatabase.CreateAsset(path, weaponPath + $"{weaponName.Replace(" ", "")}_Path.asset");
        }
        
        private static void CreateWeaponRepository(List<WeaponUpgradePathSO> paths)
        {
            var repository = ScriptableObject.CreateInstance<WeaponUpgradeRepositorySO>();
            
            // Use reflection to set the private weaponPaths field
            var weaponPathsField = typeof(WeaponUpgradeRepositorySO).GetField("weaponPaths", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (weaponPathsField != null)
            {
                weaponPathsField.SetValue(repository, paths.ToArray());
            }
            else
            {
                Debug.LogError("Could not find weaponPaths field in WeaponUpgradeRepositorySO");
            }
            
            AssetDatabase.CreateAsset(repository, AssetPath + "MainWeaponRepository.asset");
            Debug.Log($"✅ Created weapon repository with {paths.Count} weapon paths");
        }
        
        #endregion
    }
}
#endif