using Assets.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class Shotgun : Weapon {
        [SerializeField] float spreadAngle = 30f;
        [SerializeField] int nBullets = 7;

        override protected void Awake() {
            base.Awake();
            id = EWeapons.SHOTGUN;

        }
        private void Start() {
            Stats = WeaponStatsRepository.Shotgun();
            Ammo.MagazineCapacity = Stats.MagazineCapacity;

            spreadAngle *= Mathf.Deg2Rad;
        }

        public override void Shoot() {
            if (Time.time > nextFireTime) {
                for (int i = 0; i < nBullets; i++) {
                    // Calculate random spread angle for each bullet.
                    Vector2 spread = Random.insideUnitCircle * spreadAngle;
                    Vector3 spreadDirection = Thread.forward + new Vector3(spread.x, 0, spread.y);

                    // Shoot bullet
                    GameObject bulletGO = Instantiate(pfBullet, Thread.position, Thread.rotation);
                    Bullet bullet = bulletGO.GetComponent<Bullet>();
                    bullet.SetDamage(Stats.Damage);
                    Rigidbody bulletRigidbody = bulletGO.GetComponent<Rigidbody>();
                    bulletRigidbody.velocity = spreadDirection.normalized * Stats.BulletSpeed;

                    Destroy(bulletGO, Stats.Range);
                }

                nextFireTime = Time.time + 1f / Stats.FireRate;
            }
        }
    }

}
