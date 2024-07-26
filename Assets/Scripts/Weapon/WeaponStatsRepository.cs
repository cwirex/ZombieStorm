using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Repository that holds stats for each Weapon
    /// </summary>
    public static class WeaponStatsRepository { 

        private static WeaponStats BaseStats() {
            float dmg = 30f;
            float range = 5f;
            float fireRate = 2f;
            float bulletSpeed = 20f;
            int magazineCapacity = 10;
            return new WeaponStats(dmg, range, fireRate, bulletSpeed, magazineCapacity);
        }

        public static WeaponStats Pistol() {
            return BaseStats();
        }

        public static WeaponStats Rifle() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 2.3f;
            stats.BulletSpeed *= 1.1f;
            stats.FireRate *= 3f;   // 3 times faster
            stats.MagazineCapacity = 30;
            return stats;
        }

        public static WeaponStats Shotgun() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 0.5f;   // lower dmg on single bullet
            stats.BulletSpeed *= 0.7f;
            stats.FireRate /= 2.2f;
            stats.Range = 1f;
            stats.MagazineCapacity = 7;
            return stats;
        }


        public static WeaponStats SniperRifle() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 12f;
            stats.FireRate /= 6f;
            stats.BulletSpeed = 0f; // RAY
            stats.Range = 100f;
            stats.MagazineCapacity = 5;
            return stats;
        }

        public static WeaponStats SMG() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 0.8f;
            stats.BulletSpeed *= 0.9f;
            stats.FireRate *= 5f;
            stats.Range = 1.1f;
            stats.MagazineCapacity = 24;
            return stats;
        }

        public static WeaponStats M249() {
            WeaponStats stats = SMG();
            stats.Damage *= 1.4f;
            stats.BulletSpeed *= 1.3f;
            stats.FireRate *= 1.2f;
            stats.Range *= 3;
            stats.MagazineCapacity = 100;
            return stats;
        }

        public static WeaponStats RPG() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 8f;
            stats.BulletSpeed *= 0.7f;
            stats.FireRate *= 0.3f;
            return stats;
        }
    }
}