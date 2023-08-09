using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class Rifle : Weapon {
        override protected void Awake() {
            base.Awake();
            id = EWeapons.M4;

        }
        private void Start() {
            Stats = WeaponStatsRepository.Rifle();
        }


    }
}
