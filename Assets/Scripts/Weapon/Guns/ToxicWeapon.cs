using Assets.Scripts.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

    /// <summary>
    /// Concrete implementation of Weapon
    /// </summary>
public class ToxicWeapon : Weapon {
    private Transform playerTF;
    private void Start() {
        playerTF = FindObjectOfType<Player>()?.transform;
        
        // Initialize ammo system if stats are already set
        if (Stats != null) {
            Ammo.MagazineCapacity = Stats.MagazineCapacity;
        }
    }

    internal void SetStats(WeaponStats weaponStats) {
        Stats = weaponStats;
    }

    override public void Shoot() {
        if (Time.time > nextFireTime) {
            Vector3 bulletSpawnPosition = Thread.position;
            GameObject bulletGO = Instantiate(pfBullet, bulletSpawnPosition, Thread.rotation);
            ToxicBullet bullet = bulletGO.GetComponent<ToxicBullet>();
            bullet.SetDamage(Stats.Damage);
            Rigidbody bulletRigidbody = bulletGO.GetComponent<Rigidbody>();
            Vector3 shootDirection = (playerTF.position - bulletRigidbody.position).normalized;
            shootDirection.y = 0;
            bulletRigidbody.velocity = shootDirection * Stats.BulletSpeed;
            Destroy(bulletGO, 3f);

            nextFireTime = Time.time + 1f / Stats.FireRate;
        }
    }
}
