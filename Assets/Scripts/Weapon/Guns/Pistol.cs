using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class Pistol : Weapon {

        protected override void Awake() {
            base.Awake();
            id = EWeapons.PISTOL;

        }
        private void Start() {
            Stats = WeaponStatsRepository.Pistol();
            Ammo.MagazineCapacity = Stats.MagazineCapacity;

        }


    }
}
