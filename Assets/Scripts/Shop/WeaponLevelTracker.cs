using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Tracks weapon levels for the upgrade system.
    /// Single Responsibility: Only manages weapon level data.
    /// </summary>
    public class WeaponLevelTracker : MonoBehaviour
    {
        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        
        // Events for UI updates
        public event Action<EWeapons, int> OnWeaponLevelChanged;
        public event Action<EWeapons, int> OnWeaponPurchased;
        
        // Level tracking (starts at 1 when weapon is purchased)
        private Dictionary<EWeapons, int> weaponLevels = new();
        private Dictionary<EWeapons, bool> ownedWeapons = new();
        
        // Static instance for easy access
        public static WeaponLevelTracker Instance { get; private set; }
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            
            InitializeDefaults();
        }
        
        private void InitializeDefaults()
        {
            // Player starts with pistol at level 1 (default weapon with almost infinite ammo)
            ownedWeapons[EWeapons.PISTOL] = true;
            weaponLevels[EWeapons.PISTOL] = 1;
            
            // All other weapons start unowned
            foreach (EWeapons weapon in Enum.GetValues(typeof(EWeapons)))
            {
                if (weapon != EWeapons.PISTOL)
                {
                    ownedWeapons[weapon] = false;
                    weaponLevels[weapon] = 0; // 0 = not purchased
                }
            }
            
            if (debugMode)
            {
                Debug.Log("WeaponLevelTracker initialized. Pistol owned at level 1 as default weapon.");
            }
        }
        
        /// <summary>
        /// Gets the current level of a weapon (0 = not owned, 1-10 = owned levels)
        /// </summary>
        public int GetWeaponLevel(EWeapons weapon)
        {
            return weaponLevels.GetValueOrDefault(weapon, 0);
        }
        
        /// <summary>
        /// Sets weapon level (internal use only)
        /// </summary>
        public void SetWeaponLevel(EWeapons weapon, int level)
        {
            if (level < 0 || level > 10)
            {
                Debug.LogError($"Invalid weapon level: {level}. Must be 0-10.");
                return;
            }
            
            int oldLevel = weaponLevels.GetValueOrDefault(weapon, 0);
            weaponLevels[weapon] = level;
            
            // If level goes from 0 to 1, weapon was purchased
            if (oldLevel == 0 && level == 1)
            {
                ownedWeapons[weapon] = true;
                OnWeaponPurchased?.Invoke(weapon, level);
                
                if (debugMode)
                {
                    Debug.Log($"Weapon purchased: {weapon}");
                }
            }
            
            // If level increases, weapon was upgraded
            if (level > oldLevel)
            {
                OnWeaponLevelChanged?.Invoke(weapon, level);
                
                if (debugMode)
                {
                    Debug.Log($"Weapon {weapon} upgraded to level {level}");
                }
            }
        }
        
        /// <summary>
        /// Checks if player owns the weapon
        /// </summary>
        public bool OwnsWeapon(EWeapons weapon)
        {
            return ownedWeapons.GetValueOrDefault(weapon, false);
        }
        
        /// <summary>
        /// Checks if weapon can be leveled up (must be owned and below max level)
        /// </summary>
        public bool CanLevelUp(EWeapons weapon)
        {
            return OwnsWeapon(weapon) && GetWeaponLevel(weapon) < GetMaxLevel();
        }
        
        /// <summary>
        /// Checks if weapon can be purchased (not owned yet)
        /// </summary>
        public bool CanPurchase(EWeapons weapon)
        {
            return !OwnsWeapon(weapon);
        }
        
        /// <summary>
        /// Purchases a weapon (sets to level 1)
        /// </summary>
        public bool PurchaseWeapon(EWeapons weapon)
        {
            if (!CanPurchase(weapon))
            {
                if (debugMode)
                {
                    Debug.LogWarning($"Cannot purchase {weapon} - already owned or invalid.");
                }
                return false;
            }
            
            SetWeaponLevel(weapon, 1);
            return true;
        }
        
        /// <summary>
        /// Upgrades weapon by one level
        /// </summary>
        public bool UpgradeWeapon(EWeapons weapon)
        {
            if (!CanLevelUp(weapon))
            {
                if (debugMode)
                {
                    Debug.LogWarning($"Cannot upgrade {weapon} - not owned or max level reached.");
                }
                return false;
            }
            
            int currentLevel = GetWeaponLevel(weapon);
            SetWeaponLevel(weapon, currentLevel + 1);
            return true;
        }
        
        /// <summary>
        /// Gets maximum level for weapons (always 10)
        /// </summary>
        public int GetMaxLevel()
        {
            return 10;
        }
        
        /// <summary>
        /// Checks if weapon has reached ultimate level (level 10)
        /// </summary>
        public bool HasUltimateAbility(EWeapons weapon)
        {
            return GetWeaponLevel(weapon) >= 10;
        }
        
        /// <summary>
        /// Gets all owned weapons with their levels
        /// </summary>
        public Dictionary<EWeapons, int> GetOwnedWeapons()
        {
            var owned = new Dictionary<EWeapons, int>();
            foreach (var kvp in weaponLevels)
            {
                if (kvp.Value > 0) // Level 0 = not owned
                {
                    owned[kvp.Key] = kvp.Value;
                }
            }
            return owned;
        }
        
        /// <summary>
        /// For debugging - resets all weapon levels
        /// </summary>
        [ContextMenu("Reset All Weapons")]
        public void ResetAllWeapons()
        {
            InitializeDefaults();
            Debug.Log("All weapon levels reset to defaults.");
        }
        
        /// <summary>
        /// For debugging - max out all weapons
        /// </summary>
        [ContextMenu("Max All Weapons")]
        public void MaxAllWeapons()
        {
            foreach (EWeapons weapon in Enum.GetValues(typeof(EWeapons)))
            {
                SetWeaponLevel(weapon, 10);
            }
            Debug.Log("All weapons maxed out to level 10.");
        }
        
        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}