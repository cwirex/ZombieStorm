using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using Assets.Scripts.Shop;

namespace Assets.Scripts.Weapon {
    public abstract class Weapon : MonoBehaviour, IWeapon {
        [SerializeField] protected GameObject pfBullet;

        public WeaponStats Stats { get; set; }
        public Ammo Ammo = new();
        public EWeapons id;
        
        // Ultimate ability support
        public IUltimateAbility UltimateAbility { get; set; }
        public bool HasUltimateAbility => UltimateAbility != null;

        protected float nextFireTime = 0f;
        protected Transform Thread;
        protected bool isShooting = false;
        
        // Interface implementations
        public EWeapons WeaponType => id;
        Assets.Scripts.Weapon.IWeaponStats IWeapon.Stats => Stats;

        protected virtual void Awake() {
            Thread = transform.Find("Thread");
            if (Thread == null) {
                Thread = transform;
            }
        }

        protected virtual void Update() {
            if (isShooting) {
                Shoot();
            }
        }

        public virtual void Shoot() {
            if(Time.time > nextFireTime) {
                if (!Ammo.IsMagazineEmpty()) {
                    // Check for ultimate abilities that trigger on shot
                    bool extraShot = HandleUltimateAbilityOnShot();
                    
                    Ammo.Use(1);
                    //TODO Update view

                    Vector3 bulletSpawnPosition = Thread.position;
                    GameObject bulletGO = Instantiate(pfBullet, bulletSpawnPosition, Thread.rotation);
                    Bullet bullet = bulletGO.GetComponent<Bullet>();
                    
                    // Apply ultimate ability damage modifiers
                    float finalDamage = CalculateFinalDamage(Stats.Damage);
                    bullet.SetDamage(finalDamage);
                    bullet.SetSourceWeapon(this); // Set weapon reference for kill tracking
                    
                    Rigidbody bulletRigidbody = bulletGO.GetComponent<Rigidbody>();
                    bulletRigidbody.velocity = Thread.forward * Stats.BulletSpeed;
                    Destroy(bulletGO, Stats.Range);

                    // Handle extra shot for Double Tap (Pistol ultimate)
                    if (extraShot) {
                        FireExtraShot();
                    }

                    nextFireTime = Time.time + 1f / Stats.FireRate;
                } else {
                    if (Ammo.Reload()) {
                        // Shoot();
                    } else {
                        Debug.Log("No ammo in magazine and unable to reload.");
                    }
                }
            }   
        }
        
        /// <summary>
        /// Handles ultimate abilities that trigger on each shot
        /// </summary>
        /// <returns>True if an extra shot should be fired (Double Tap)</returns>
        protected virtual bool HandleUltimateAbilityOnShot() {
            if (!HasUltimateAbility || !UltimateAbility.IsActive) 
                return false;
                
            // Handle Double Tap (Pistol) - 40% chance to fire twice
            if (id == EWeapons.PISTOL && UnityEngine.Random.Range(0f, 1f) < 0.4f) {
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// Calculates final damage including ultimate ability modifiers
        /// </summary>
        protected virtual float CalculateFinalDamage(float baseDamage) {
            if (!HasUltimateAbility || !UltimateAbility.IsActive) 
                return baseDamage;
                
            float finalDamage = baseDamage;
            
            // Handle Critical Spray (UZI) - 25% chance for +100% damage
            if (id == EWeapons.UZI && UnityEngine.Random.Range(0f, 1f) < 0.25f) {
                finalDamage *= 2f; // +100% damage
            }
            
            return finalDamage;
        }
        
        /// <summary>
        /// Fires an extra shot for Double Tap ability (no ammo cost)
        /// </summary>
        protected virtual void FireExtraShot() {
            Vector3 bulletSpawnPosition = Thread.position;
            GameObject bulletGO = Instantiate(pfBullet, bulletSpawnPosition, Thread.rotation);
            Bullet bullet = bulletGO.GetComponent<Bullet>();
            bullet.SetDamage(Stats.Damage);
            bullet.SetSourceWeapon(this); // Set weapon reference for kill tracking
            Rigidbody bulletRigidbody = bulletGO.GetComponent<Rigidbody>();
            bulletRigidbody.velocity = Thread.forward * Stats.BulletSpeed;
            Destroy(bulletGO, Stats.Range);
        }
        
        /// <summary>
        /// Called when this weapon kills an enemy - for ammo regeneration abilities
        /// </summary>
        public virtual void OnEnemyKilled() {
            // Handle ammo regeneration for LMG and Flamethrower
            if (id == EWeapons.M249 || id == EWeapons.FLAMETHROWER) {
                Ammo.RegenerateAmmoOnKill();
            }
        }

        internal virtual void OnShootPerformed() {
            Shoot();
            isShooting = true;
        }

        internal virtual void OnShootCanceled() {
            isShooting = false;
        }
    }
}