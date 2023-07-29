using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class SniperRifle : Weapon {
        [SerializeField] LineRenderer shotTrail;
        [SerializeField] float trailDuriation = 0.4f;
        private void Start() {
            Stats = WeaponStatsRepository.SniperRifle();
        }

        public override void Shoot() {
            if (Time.time > nextFireTime) {
                RaycastHit[] hits;

                hits = Physics.RaycastAll(Thread.position, transform.forward);
                Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

                Vector3 trailStartpoint = Thread.position;
                Vector3 trailEndpoint = Thread.position + Thread.forward * Stats.Range;

                // apply damage to all enemies (in front of a wall)
                foreach (var hit in hits) {
                    if (hit.collider.TryGetComponent(out IDamagable damagable)) {
                        damagable.Damage(Stats.Damage);
                    } 
                    else if (hit.collider.TryGetComponent(out Obstacle obstacle)) {
                        trailEndpoint = hit.point;  // stop ray on a wall
                        break;
                    }
                }

                nextFireTime = Time.time + 1f / Stats.FireRate;
                StartCoroutine(MakeLineTrail(trailStartpoint, trailEndpoint, trailDuriation));
            }
            
        }

        private IEnumerator MakeLineTrail(Vector3 trailStartpoint, Vector3 trailEndpoint, float trailDuriation) {
            shotTrail.SetPosition(0, trailStartpoint);
            shotTrail.SetPosition(1, trailEndpoint);
            shotTrail.enabled = true;
            yield return new WaitForSeconds(trailDuriation);
            shotTrail.enabled = false;
        }
    }

}