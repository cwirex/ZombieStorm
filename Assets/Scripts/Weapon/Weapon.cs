using System;
using System.Collections;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    public abstract class Weapon : MonoBehaviour {
        [SerializeField] protected GameObject pfBullet;
        protected WeaponStats Stats;
        public EWeapons id;

        protected float nextFireTime = 0f;
        protected Transform Thread;
        protected bool isShooting = false;

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
                Vector3 bulletSpawnPosition = Thread.position;
                GameObject bulletGO = Instantiate(pfBullet, bulletSpawnPosition, Thread.rotation);
                Bullet bullet = bulletGO.GetComponent<Bullet>();
                bullet.SetDamage(Stats.Damage);
                Rigidbody bulletRigidbody = bulletGO.GetComponent<Rigidbody>();
                bulletRigidbody.velocity = Thread.forward * Stats.BulletSpeed;
                Destroy(bulletGO, Stats.Range);

                nextFireTime = Time.time + 1f / Stats.FireRate;
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