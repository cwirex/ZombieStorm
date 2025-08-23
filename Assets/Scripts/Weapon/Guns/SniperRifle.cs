using System;
using System.Collections;
using UnityEngine;
using UnityEngineInternal;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
    public class SniperRifle : Weapon {
        [SerializeField] LineRenderer shotTrail;
        [SerializeField] float trailDuriation = 0.4f;

        override protected void Awake() {
            base.Awake();
            id = EWeapons.AWP;

        }
        private void Start() {
            Stats = WeaponStatsRepository.SniperRifle();
            Ammo.MagazineCapacity = Stats.MagazineCapacity;
        }

        public override void Shoot() {
            if (Time.time > nextFireTime) {
                // Check ammo before shooting
                if (!Ammo.IsMagazineEmpty()) {
                    Ammo.Use(1); // AWP uses 1 bullet per shot
                    
                    RaycastHit[] hits;

                    hits = Physics.RaycastAll(Thread.position, transform.forward);
                    Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

                    Vector3 trailStartpoint = Thread.position;
                    Vector3 trailEndpoint = Thread.position + Thread.forward * Stats.Range;

                    // apply damage to all enemies (in front of a wall)
                    foreach (var hit in hits) {
                        if (hit.collider.TryGetComponent(out IDamagable damagable)) {
                            Vector3 direction = (trailEndpoint - trailStartpoint).normalized;
                            damagable.TakeDamage(Stats.Damage, direction);
                        } 
                        else if(hit.collider.TryGetComponent(out Explosive explosive)) {
                            explosive.TriggerExplosion();
                        }
                        else if (hit.collider.TryGetComponent(out Obstacle obstacle)) {
                            trailEndpoint = hit.point;  // stop ray on a wall
                            break;
                        }
                    }

                    nextFireTime = Time.time + 1f / Stats.FireRate;
                    StartCoroutine(MakeLineTrail(trailStartpoint, trailEndpoint, trailDuriation));
                } else {
                    // Try to reload if magazine is empty
                    if (Ammo.Reload()) {
                        // Successfully reloaded, could try shooting again
                    } else {
                        Debug.Log("AWP: No ammo left to reload.");
                    }
                }
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