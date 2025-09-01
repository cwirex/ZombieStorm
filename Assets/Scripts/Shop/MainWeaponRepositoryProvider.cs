using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Provides singleton access to the auto-generated MainWeaponRepository
    /// This eliminates the need for manual assignment in the inspector
    /// </summary>
    public static class MainWeaponRepositoryProvider
    {
        private static WeaponUpgradeRepositorySO _instance;
        
        /// <summary>
        /// Gets the MainWeaponRepository instance. Loads it on first access.
        /// </summary>
        public static WeaponUpgradeRepositorySO Instance
        {
            get
            {
                if (_instance == null)
                {
                    LoadMainWeaponRepository();
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// Forces a reload of the MainWeaponRepository (useful after regeneration)
        /// </summary>
        public static void ReloadRepository()
        {
            _instance = null;
            LoadMainWeaponRepository();
        }
        
        /// <summary>
        /// Loads the MainWeaponRepository from the generated assets
        /// </summary>
        private static void LoadMainWeaponRepository()
        {
            // Try loading from Resources first (if user moved it there)
            _instance = Resources.Load<WeaponUpgradeRepositorySO>("MainWeaponRepository");
            
            if (_instance == null)
            {
                // Fallback: Load from known generation path using Resources.LoadAll
                var allRepositories = Resources.LoadAll<WeaponUpgradeRepositorySO>("");
                
                foreach (var repo in allRepositories)
                {
                    if (repo.name == "MainWeaponRepository")
                    {
                        _instance = repo;
                        break;
                    }
                }
            }
            
#if UNITY_EDITOR
            // In editor, we can use AssetDatabase as last resort
            if (_instance == null)
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("MainWeaponRepository t:WeaponUpgradeRepositorySO");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    _instance = UnityEditor.AssetDatabase.LoadAssetAtPath<WeaponUpgradeRepositorySO>(path);
                }
            }
#endif
            
            if (_instance == null)
            {
                Debug.LogError("MainWeaponRepositoryProvider: Failed to load MainWeaponRepository! " +
                             "Run 'Tools/Shop/Generate All Weapon Upgrades (Automated)' to create it.");
            }
        }
        
        /// <summary>
        /// Checks if the repository is properly loaded and contains weapon paths
        /// </summary>
        public static bool IsValid()
        {
            var repo = Instance;
            if (repo == null) return false;
            
            // Check if repository has weapon paths by trying to get a common weapon
            return repo.GetUpgradePath(EWeapons.PISTOL) != null;
        }
        
        /// <summary>
        /// Gets debug info about the loaded repository
        /// </summary>
        public static string GetDebugInfo()
        {
            var repo = Instance;
            if (repo == null)
            {
                return "MainWeaponRepository: Not loaded";
            }
            
            int weaponCount = 0;
            foreach (EWeapons weapon in System.Enum.GetValues(typeof(EWeapons)))
            {
                if (repo.GetUpgradePath(weapon) != null)
                {
                    weaponCount++;
                }
            }
            
            return $"MainWeaponRepository: {repo.name} (Contains {weaponCount} weapon paths)";
        }
    }
}