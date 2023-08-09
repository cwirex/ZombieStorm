using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class RPG : Weapon {
        override protected void Awake() {
            base.Awake();
            id = EWeapons.RPG7;

        }
        private void Start() {
            Stats = WeaponStatsRepository.RPG();
        }


    }
}
