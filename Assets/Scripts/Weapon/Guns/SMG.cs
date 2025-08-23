using Assets.Scripts.Weapon;
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
            if (Time.time > nextFireTime) {
                // Check ammo before shooting
                if (!Ammo.IsMagazineEmpty()) {
                    Ammo.Use(1);
                    
                    // Calculate random spread angle
                    Vector2 spread = Random.insideUnitCircle * spreadAngle;
                    Vector3 spreadDirection = Thread.forward + new Vector3(spread.x, 0, spread.y);

                    // Shoot bullet
                    GameObject bulletGO = Instantiate(pfBullet, Thread.position, Thread.rotation);
                    Bullet bullet = bulletGO.GetComponent<Bullet>();
                    bullet.SetDamage(Stats.Damage);
                    Rigidbody bulletRigidbody = bulletGO.GetComponent<Rigidbody>();
                    bulletRigidbody.velocity = spreadDirection.normalized * Stats.BulletSpeed;

                    Destroy(bulletGO, Stats.Range);
                    nextFireTime = Time.time + 1f / Stats.FireRate;
                } else {
                    // Try to reload if magazine is empty
                    if (Ammo.Reload()) {
                        // Successfully reloaded, could try shooting again
                    } else {
                        Debug.Log("SMG: No ammo left to reload.");
                    }
                }
            }
        }
    }

}
