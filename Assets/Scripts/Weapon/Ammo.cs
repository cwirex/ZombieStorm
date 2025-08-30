using Assets.Scripts.PlayerScripts;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    public class Ammo {
        public int AmmoLeft { get; private set; }
        public int CurrentAmmoInMagazine { get; private set; }
        public int MagazineCapacity { get; set; }
        
        // Ammo regeneration properties for LMG/Flamethrower
        public float AmmoRegenPercentage { get; set; } = 0f; // Percentage of total ammo restored on kill
        private int maxAmmoCapacity = 0; // Store original max ammo for regeneration calculations

        public static UIController UIController { get; set; }

        /// <returns>True if reload was successful</returns>
        public bool Reload() {
            if(AmmoLeft <= 0 || CurrentAmmoInMagazine == MagazineCapacity) 
                return false;

            int ammoNeeded = MagazineCapacity - CurrentAmmoInMagazine;
            int ammoToReload = Mathf.Min(AmmoLeft, ammoNeeded);
            CurrentAmmoInMagazine += ammoToReload;
            AmmoLeft -= ammoToReload;
            UpdateUI();
            return true;
        }

        /// <returns>True if there is enough ammo to use</returns>
        public bool Use(int amount) {
            if (CurrentAmmoInMagazine < amount) {
                CurrentAmmoInMagazine = 0;
                return false;
            } else {
                CurrentAmmoInMagazine -= amount;
                UpdateUI();
                return true;
            }
        }

        public void AddAmmo(int amount) {
            AmmoLeft += amount;
        }
        
        /// <summary>
        /// Sets the maximum ammo capacity for regeneration calculations
        /// </summary>
        public void SetMaxAmmoCapacity(int maxCapacity) {
            maxAmmoCapacity = maxCapacity;
        }
        
        /// <summary>
        /// Regenerates ammo based on percentage when enemy is killed (for LMG/Flamethrower)
        /// </summary>
        public void RegenerateAmmoOnKill() {
            if (AmmoRegenPercentage <= 0f || maxAmmoCapacity <= 0) return;
            
            int ammoToRestore = Mathf.RoundToInt(maxAmmoCapacity * (AmmoRegenPercentage / 100f));
            if (ammoToRestore > 0) {
                AmmoLeft += ammoToRestore;
                // Cap at max capacity to prevent infinite ammo buildup
                AmmoLeft = Mathf.Min(AmmoLeft, maxAmmoCapacity);
                UpdateUI();
            }
        }

        public bool IsMagazineEmpty() {
            return CurrentAmmoInMagazine == 0;
        }

        public void UpdateUI() {
            UIController?.UpdateAmmoCounter(CurrentAmmoInMagazine, AmmoLeft, MagazineCapacity);
        }
    }
}