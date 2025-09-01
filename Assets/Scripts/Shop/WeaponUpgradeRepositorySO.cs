using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Shop
{
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