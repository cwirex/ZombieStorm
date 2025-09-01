using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// ScriptableObject that defines the complete upgrade path for a weapon
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponUpgradePath", menuName = "Shop/Weapon Upgrade Path")]
    public class WeaponUpgradePathSO : ScriptableObject, IWeaponUpgradePath
    {
        [Header("Weapon Info")]
        public EWeapons weaponType;
        public string weaponName;
        [TextArea(2, 3)]
        public string weaponDescription;
        
        [Header("Upgrade Path")]
        [SerializeField] private WeaponUpgradeSO[] upgrades = new WeaponUpgradeSO[10];
        [SerializeField] private UltimateAbilitySO ultimateAbility;
        
        [Header("Visual")]
        public Sprite weaponIcon;
        public Color weaponColor = Color.white;
        
        public IWeaponUpgrade GetUpgradeForLevel(int level)
        {
            if (level < 1 || level > 10)
            {
                Debug.LogError($"Invalid level {level} for {weaponType}. Must be 1-10");
                return null;
            }
            
            // Level 1 is weapon purchase (no upgrade)
            if (level == 1)
            {
                return null;
            }
            
            // Levels 2-10 map to indices 0-8
            int index = level - 2;
            return upgrades[index];
        }
        
        public int GetMaxLevel() => 10;
        
        public bool HasUltimateAbility() => ultimateAbility != null;
        
        public IUltimateAbility GetUltimateAbility() => ultimateAbility;
        
        /// <summary>
        /// Validates that all upgrade slots are filled
        /// Level 1 is weapon purchase (no upgrade), levels 2-10 are actual upgrades
        /// Array structure: [level2, level3, level4, level5, level6, level7, level8, level9, level10, null]
        /// </summary>
        public bool IsValid()
        {
            if (upgrades.Length != 10)
            {
                Debug.LogError($"{name}: Upgrade path must have exactly 10 levels");
                return false;
            }
            
            // Check levels 2-10 (indices 0-8)
            for (int i = 0; i < 9; i++)
            {
                int expectedLevel = i + 2; // Level 2 at index 0, Level 3 at index 1, etc.
                
                if (upgrades[i] == null)
                {
                    Debug.LogError($"{name}: Missing upgrade for level {expectedLevel}");
                    return false;
                }
                
                if (upgrades[i].level != expectedLevel)
                {
                    Debug.LogWarning($"{name}: Upgrade level mismatch at index {i}. Expected {expectedLevel}, got {upgrades[i].level}");
                }
            }
            
            // Index 9 should be null (level 1 slot, not used)
            if (upgrades[9] != null)
            {
                Debug.LogWarning($"{name}: Index 9 should be null (level 1 is weapon purchase), but contains upgrade for level {upgrades[9].level}");
            }
            
            if (!HasUltimateAbility())
            {
                Debug.LogWarning($"{name}: No ultimate ability assigned for level 10");
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets upgrade descriptions for all levels (1-10)
        /// </summary>
        public string[] GetAllDescriptions()
        {
            var descriptions = new string[10];
            
            // Level 1 is weapon purchase
            descriptions[0] = "Weapon Purchase";
            
            // Levels 2-10 (indices 0-8 in upgrades array)
            for (int level = 2; level <= 10; level++)
            {
                int upgradeIndex = level - 2;
                descriptions[level - 1] = upgrades[upgradeIndex]?.GetDescription() ?? "No upgrade";
            }
            
            return descriptions;
        }
        
        /// <summary>
        /// For editor validation
        /// </summary>
        private void OnValidate()
        {
            // Ensure array is always 10 elements
            if (upgrades.Length != 10)
            {
                System.Array.Resize(ref upgrades, 10);
            }
            
            // Auto-assign correct levels to upgrades (levels 2-10 for indices 0-8)
            for (int i = 0; i < 9; i++)
            {
                if (upgrades[i] != null && upgrades[i].level == 0)
                {
                    upgrades[i].level = i + 2; // Level 2 for index 0, Level 3 for index 1, etc.
                }
            }
            
            // Ensure index 9 (level 1 slot) is always null
            upgrades[9] = null;
        }
    }
}