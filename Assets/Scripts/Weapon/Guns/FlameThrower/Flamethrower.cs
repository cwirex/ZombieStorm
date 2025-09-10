using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Assets.Scripts.Audio;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class Flamethrower : Weapon {
        [SerializeField] private float damagePerSecond = 200f;
        [SerializeField] private float ticksPerSecond = 5f;
        [SerializeField] private float ammoConsumptionPerSecond = 10f; // Fuel consumption rate

        private FlamethrowerBullet flames;
        private GameObject flamesGO;
        private float lastAmmoConsumeTime = 0f;

        protected override void Awake() {
            base.Awake();
            id = EWeapons.FLAMETHROWER;
        }

        private void Start() {
            Stats = WeaponStatsRepository.Flamethrower();
            Ammo.MagazineCapacity = Stats.MagazineCapacity;
            
            flames = GetComponentInChildren<FlamethrowerBullet>();
            if (flames == null) Debug.LogError("GetComponentInChildren<FlamethrowerBullet> returned NULL in Flamethrower initialization");
            flames.SetStats(new WeaponStats(damagePerSecond, 0, ticksPerSecond, 0, Stats.MagazineCapacity));
            flamesGO = flames.gameObject;
            flamesGO.SetActive(false);
        }

        override protected void Update() {
            // Consume ammo continuously while shooting
            if (isShooting) {
                ConsumeAmmoOverTime();
            }
        }

        override public void Shoot() {
            //
        }

        override internal void OnShootPerformed() {
            // Cannot shoot while reloading
            if (Ammo.IsReloading) return;
            
            // Only start shooting if we have ammo
            if (!Ammo.IsMagazineEmpty()) {
                if (!isShooting) {
                    // Trigger sound only when starting to shoot (not continuously)
                    SoundEvents.TriggerFlamethrowerStart();
                }
                isShooting = true;
                lastAmmoConsumeTime = Time.time;
                flamesGO?.SetActive(true);
            } else {
                // Start timed reload instead of instant reload
                StartReload();
            }
        }

        override internal void OnShootCanceled() {
            if (isShooting) {
                // Trigger stop sound when stopping shooting
                SoundEvents.TriggerFlamethrowerStop();
            }
            isShooting = false;
            float smallDelay = 0.12f;
            StartCoroutine(DelayCancel(smallDelay));
        }
        
        private void ConsumeAmmoOverTime() {
            float currentTime = Time.time;
            float timeSinceLastConsume = currentTime - lastAmmoConsumeTime;
            
            // Consume ammo based on time elapsed
            if (timeSinceLastConsume >= 1f / ammoConsumptionPerSecond) {
                if (Ammo.IsMagazineEmpty()) {
                    // Out of fuel - stop shooting
                    OnShootCanceled();
                    StartReload();
                } else {
                    Ammo.Use(1); // Consume 1 fuel unit
                    lastAmmoConsumeTime = currentTime;
                }
            }
        }

        private IEnumerator DelayCancel(float time) {
            yield return new WaitForSeconds(time);
            flamesGO?.SetActive(isShooting);
        }

    }
}
