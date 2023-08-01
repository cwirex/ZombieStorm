using Assets.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class LMG : Weapon {
        [SerializeField] float minSpreadAngle = 1f;
        [SerializeField] float maxSpreadAngle = 25f;
        [SerializeField] int spreadAngleSteps = 12;

        private float spreadAngle;
        private float spreadAngleStep;
        private void Start() {
            Stats = WeaponStatsRepository.LMG();
            minSpreadAngle *= Mathf.Deg2Rad;
            maxSpreadAngle *= Mathf.Deg2Rad;
            spreadAngle = minSpreadAngle;
            spreadAngleStep = (maxSpreadAngle - minSpreadAngle) / spreadAngleSteps;
        }

        override protected void Update() {
            if(Time.time > nextFireTime) {
                if (isShooting) {
                    if (spreadAngle < maxSpreadAngle) {
                        spreadAngle += spreadAngleStep;
                    }
                    Shoot();
                } else if (spreadAngle > minSpreadAngle) { // Not shooting, decrease angle
                    spreadAngle -= spreadAngleStep;
                }
                nextFireTime = Time.time + 1f / Stats.FireRate;
            }
        }

        public override void Shoot() {
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
        }
    }

}
