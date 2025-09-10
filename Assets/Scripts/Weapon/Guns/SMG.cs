using Assets.Scripts.Weapon;
using Assets.Scripts.Audio;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class SMG : Weapon {
        [SerializeField] float spreadAngle = 12f;

        override protected void Awake() {
            base.Awake();
            id = EWeapons.UZI;

        }

        private void Start() {
            Stats = WeaponStatsRepository.SMG();
            Ammo.MagazineCapacity = Stats.MagazineCapacity;
            spreadAngle *= Mathf.Deg2Rad;
        }

        public override void Shoot() {
            // Cannot shoot while reloading
            if (Ammo.IsReloading) return;
            
            if (Time.time > nextFireTime) {
                // Check ammo before shooting
                if (!Ammo.IsMagazineEmpty()) {
                    Ammo.Use(1);
                    
                    // Trigger weapon sound
                    SoundEvents.TriggerWeaponShoot(id);
                    
                    // Calculate random spread angle
                    Vector2 spread = Random.insideUnitCircle * spreadAngle;
                    Vector3 spreadDirection = Thread.forward + new Vector3(spread.x, 0, spread.y);

                    // Shoot bullet
                    GameObject bulletGO = Instantiate(pfBullet, Thread.position, Thread.rotation);
                    Bullet bullet = bulletGO.GetComponent<Bullet>();
                    bullet.SetDamage(Stats.Damage);
                    bullet.SetSourceWeapon(this); // Set weapon reference for kill tracking
                    Rigidbody bulletRigidbody = bulletGO.GetComponent<Rigidbody>();
                    bulletRigidbody.velocity = spreadDirection.normalized * Stats.BulletSpeed;

                    Destroy(bulletGO, Stats.Range);
                    nextFireTime = Time.time + 1f / Stats.FireRate;
                } else {
                    // Start timed reload instead of instant reload
                    StartReload();
                }
            }
        }
    }

}
