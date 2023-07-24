using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class Rifle : Weapon {
        private void Start() {
            Stats = WeaponStatsRepository.Rifle();
        }


    }
}
