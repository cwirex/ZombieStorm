using Assets.Scripts.PlayerScripts;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    public class Ammo {
        public int AmmoLeft { get; private set; }
        public int CurrentAmmoInMagazine { get; private set; }
        public int MagazineCapacity { get; set; }

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

        public bool IsMagazineEmpty() {
            return CurrentAmmoInMagazine == 0;
        }

        public void UpdateUI() {
            UIController?.UpdateAmmoCounter(CurrentAmmoInMagazine, AmmoLeft, MagazineCapacity);
        }
    }
}