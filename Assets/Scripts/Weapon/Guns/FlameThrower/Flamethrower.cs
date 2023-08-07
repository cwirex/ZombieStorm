using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class Flamethrower : Weapon {
        [SerializeField] private float damagePerSecond = 200f;
        [SerializeField] private float ticksPerSecond = 5f;

        private FlamethrowerBullet flames;
        private GameObject flamesGO;

        private void Start() {
            flames = GetComponentInChildren<FlamethrowerBullet>();
            if (flames == null) Debug.LogError("GetComponentInChildren<FlamethrowerBullet> returned NULL in Flamethrower initialization");
            flames.SetStats(new WeaponStats(damagePerSecond, 0, ticksPerSecond, 0));
            flamesGO = flames.gameObject;
            flamesGO.SetActive(false);
        }

        override protected void Update() {
            //
        }

        override public void Shoot() {
            //
        }

        override internal void OnShootPerformed() {
            isShooting = true;
            flamesGO?.SetActive(true);
        }

        override internal void OnShootCanceled() {
            isShooting = false;
            float smallDelay = 0.12f;
            StartCoroutine(DelayCancel(smallDelay));
        }

        private IEnumerator DelayCancel(float time) {
            yield return new WaitForSeconds(time);
            flamesGO?.SetActive(isShooting);
        }

    }
}
