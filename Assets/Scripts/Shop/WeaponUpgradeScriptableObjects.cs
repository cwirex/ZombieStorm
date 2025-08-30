using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// ScriptableObject that defines a stat modifier for weapon upgrades
    /// </summary>
    [System.Serializable]
    public class StatModifier
    {
        [Header("Stat Modification")]
        public StatType statType;
        public ModifierType modifierType;
        public float value;
        
        [Header("Description")]
        [TextArea(2, 3)]
        public string description;
        
        /// <summary>
        /// Applies this modifier to the given stats
        /// </summary>
        /// <param name="stats">Stats to modify</param>
        public void Apply(Assets.Scripts.Weapon.IWeaponStats stats)
        {
            switch (statType)
            {
                case StatType.Damage:
                    stats.Damage = ApplyModifier(stats.Damage, value, modifierType);
                    break;
                case StatType.Range:
                    stats.Range = ApplyModifier(stats.Range, value, modifierType);
                    break;
                case StatType.FireRate:
                    stats.FireRate = ApplyModifier(stats.FireRate, value, modifierType);
                    break;
                case StatType.BulletSpeed:
                    stats.BulletSpeed = ApplyModifier(stats.BulletSpeed, value, modifierType);
                    break;
                case StatType.MagazineCapacity:
                    stats.MagazineCapacity = Mathf.RoundToInt(ApplyModifier(stats.MagazineCapacity, value, modifierType));
                    break;
                case StatType.Accuracy:
                    stats.Accuracy = ApplyModifier(stats.Accuracy, value, modifierType);
                    break;
                case StatType.Recoil:
                    stats.Recoil = ApplyModifier(stats.Recoil, value, modifierType);
                    break;
                case StatType.ReloadSpeed:
                    stats.ReloadSpeed = ApplyModifier(stats.ReloadSpeed, value, modifierType);
                    break;
                case StatType.ExtraMagazines:
                    stats.ExtraMagazines = Mathf.RoundToInt(ApplyModifier(stats.ExtraMagazines, value, modifierType));
                    break;
            }
        }
        
        private float ApplyModifier(float currentValue, float modifierValue, ModifierType type)
        {
            return type switch
            {
                ModifierType.Add => currentValue + modifierValue,
                ModifierType.Multiply => currentValue * modifierValue,
                ModifierType.Percentage => currentValue * (1f + modifierValue / 100f),
                ModifierType.Set => modifierValue,
                _ => currentValue
            };
        }
    }
    
    /// <summary>
    /// Types of modifiers that can be applied to stats
    /// </summary>
    public enum ModifierType
    {
        Add,        // Add flat value
        Multiply,   // Multiply by value
        Percentage, // Add percentage (10 = +10%)
        Set         // Set to specific value
    }
    
    /// <summary>
    /// ScriptableObject that defines a single weapon upgrade level
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "Shop/Weapon Upgrade")]
    public class WeaponUpgradeSO : ScriptableObject, IWeaponUpgrade
    {
        [Header("Upgrade Info")]
        public int level;
        [Range(0, 100)]
        public int costPercentage;
        [TextArea(2, 4)]
        public string description;
        
        [Header("Stat Modifications")]
        public StatModifier[] statModifiers;
        
        [Header("Visual")]
        public Sprite upgradeIcon;
        public Color upgradeColor = Color.white;
        
        public int GetCostPercentage() => costPercentage;
        public string GetDescription() => description;
        
        public void ApplyTo(Assets.Scripts.Weapon.IWeaponStats stats)
        {
            if (stats == null)
            {
                Debug.LogError($"Cannot apply upgrade {name}: stats is null");
                return;
            }
            
            foreach (var modifier in statModifiers)
            {
                try
                {
                    modifier.Apply(stats);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error applying modifier in {name}: {ex.Message}");
                }
            }
        }
    }
    
    /// <summary>
    /// ScriptableObject that defines an ultimate ability for level 10 weapons
    /// </summary>
    [CreateAssetMenu(fileName = "UltimateAbility", menuName = "Shop/Ultimate Ability")]
    public class UltimateAbilitySO : ScriptableObject, IUltimateAbility
    {
        [Header("Ability Info")]
        public string abilityName;
        [TextArea(3, 5)]
        public string description;
        public UltimateAbilityType abilityType;
        
        [Header("Ability Parameters")]
        public float triggerChance = 1.0f; // For OnHit abilities (1.0 = always, 0.1 = 10% chance)
        public float effectDuration = 0f;   // For timed effects
        public float effectValue = 0f;      // General purpose value
        public float effectRadius = 0f;     // For area effects
        
        [Header("Visual & Audio")]
        public GameObject effectPrefab;
        public AudioClip activationSound;
        public Color effectColor = Color.white;
        public Sprite abilityIcon;
        
        private bool isActive = false;
        
        public string Name => abilityName;
        public string Description => description;
        public bool IsActive => isActive;
        
        public void Activate(IWeapon weapon)
        {
            if (weapon == null)
            {
                Debug.LogError($"Cannot activate {abilityName}: weapon is null");
                return;
            }
            
            if (isActive)
            {
                Debug.LogWarning($"{abilityName} is already active");
                return;
            }
            
            isActive = true;
            
            // Play activation sound
            if (activationSound != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(activationSound, Camera.main.transform.position);
            }
            
            Debug.Log($"Ultimate ability activated: {abilityName} for {weapon.WeaponType}");
            
            // Specific activation logic would be handled by the weapon implementation
            // This is a base implementation that can be overridden
        }
        
        public void Deactivate(IWeapon weapon)
        {
            if (!isActive)
                return;
            
            isActive = false;
            Debug.Log($"Ultimate ability deactivated: {abilityName} for {weapon?.WeaponType}");
        }
    }
    
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
            if (level < 1 || level > upgrades.Length)
            {
                Debug.LogError($"Invalid level {level} for {weaponType}. Must be 1-{upgrades.Length}");
                return null;
            }
            
            return upgrades[level - 1]; // Convert to 0-based index
        }
        
        public int GetMaxLevel() => upgrades.Length;
        
        public bool HasUltimateAbility() => ultimateAbility != null;
        
        public IUltimateAbility GetUltimateAbility() => ultimateAbility;
        
        /// <summary>
        /// Validates that all upgrade slots are filled
        /// </summary>
        public bool IsValid()
        {
            if (upgrades.Length != 10)
            {
                Debug.LogError($"{name}: Upgrade path must have exactly 10 levels");
                return false;
            }
            
            for (int i = 0; i < upgrades.Length; i++)
            {
                if (upgrades[i] == null)
                {
                    Debug.LogError($"{name}: Missing upgrade for level {i + 1}");
                    return false;
                }
                
                if (upgrades[i].level != i + 1)
                {
                    Debug.LogWarning($"{name}: Upgrade level mismatch at index {i}. Expected {i + 1}, got {upgrades[i].level}");
                }
            }
            
            if (!HasUltimateAbility())
            {
                Debug.LogWarning($"{name}: No ultimate ability assigned for level 10");
            }
            
            return true;
        }
        
        /// <summary>
        /// Gets upgrade descriptions for all levels
        /// </summary>
        public string[] GetAllDescriptions()
        {
            var descriptions = new string[upgrades.Length];
            for (int i = 0; i < upgrades.Length; i++)
            {
                descriptions[i] = upgrades[i]?.GetDescription() ?? "No upgrade";
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
            
            // Auto-assign weapon type to upgrades if they match
            for (int i = 0; i < upgrades.Length; i++)
            {
                if (upgrades[i] != null && upgrades[i].level == 0)
                {
                    upgrades[i].level = i + 1;
                }
            }
        }
    }
    
    /// <summary>
    /// Repository that manages weapon upgrade paths loaded from ScriptableObjects
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponUpgradeRepository", menuName = "Shop/Weapon Upgrade Repository")]
    public class WeaponUpgradeRepositorySO : ScriptableObject, IWeaponUpgradeRepository
    {
        [Header("Weapon Upgrade Paths")]
        [SerializeField] private WeaponUpgradePathSO[] weaponPaths;
        
        private Dictionary<EWeapons, IWeaponUpgradePath> pathLookup;
        
        private void OnEnable()
        {
            BuildLookupTable();
        }
        
        private void BuildLookupTable()
        {
            pathLookup = new Dictionary<EWeapons, IWeaponUpgradePath>();
            
            if (weaponPaths == null)
                return;
            
            foreach (var path in weaponPaths)
            {
                if (path != null)
                {
                    pathLookup[path.weaponType] = path;
                    
                    // Validate path
                    if (!path.IsValid())
                    {
                        Debug.LogError($"Invalid weapon upgrade path: {path.name}");
                    }
                }
            }
        }
        
        public IWeaponUpgradePath GetUpgradePath(EWeapons weaponType)
        {
            if (pathLookup == null)
                BuildLookupTable();
            
            return pathLookup.GetValueOrDefault(weaponType);
        }
        
        public void RegisterUpgradePath(EWeapons weaponType, IWeaponUpgradePath path)
        {
            if (pathLookup == null)
                pathLookup = new Dictionary<EWeapons, IWeaponUpgradePath>();
            
            pathLookup[weaponType] = path;
        }
        
        /// <summary>
        /// Gets all registered weapon types
        /// </summary>
        public EWeapons[] GetRegisteredWeapons()
        {
            if (pathLookup == null)
                BuildLookupTable();
            
            var weapons = new EWeapons[pathLookup.Count];
            pathLookup.Keys.CopyTo(weapons, 0);
            return weapons;
        }
        
        /// <summary>
        /// Validates all weapon paths in the repository
        /// </summary>
        [ContextMenu("Validate All Paths")]
        public void ValidateAllPaths()
        {
            BuildLookupTable();
            
            Debug.Log("=== Weapon Upgrade Repository Validation ===");
            
            foreach (var weapon in System.Enum.GetValues(typeof(EWeapons)))
            {
                var weaponType = (EWeapons)weapon;
                var path = GetUpgradePath(weaponType);
                
                if (path == null)
                {
                    Debug.LogWarning($"No upgrade path found for {weaponType}");
                }
                else
                {
                    Debug.Log($"✓ {weaponType}: Path found with {path.GetMaxLevel()} levels");
                }
            }
        }
    }
}