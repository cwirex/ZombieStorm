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
            return new WeaponStats(dmg, range, fireRate, bulletSpeed);
        }

        public static WeaponStats Pistol() {
            return BaseStats();
        }

        public static WeaponStats Rifle() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 1.2f;
            stats.BulletSpeed *= 1.1f;
            stats.FireRate *= 3f;   // 3 times faster
            return stats;
        }

        public static WeaponStats Shotgun() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 0.5f;   // lower dmg on single bullet
            stats.BulletSpeed *= 0.7f;
            stats.FireRate /= 3f; // 3 times slower
            stats.Range = 1f;

            return stats;
        }


        public static WeaponStats SniperRifle() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 10f;
            stats.FireRate /= 10f;
            stats.BulletSpeed = 0f; // RAY
            stats.Range = 100f;

            return stats;
        }

        public static WeaponStats SMG() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 0.8f;
            stats.BulletSpeed *= 0.9f;
            stats.FireRate *= 5f;
            stats.Range = 1.1f;
            return stats;
        }

        public static WeaponStats LMG() {
            WeaponStats stats = SMG();
            stats.Damage *= 1.4f;
            stats.BulletSpeed *= 1.3f;
            stats.FireRate *= 1.2f;
            stats.Range *= 3;
            return stats;
        }

        public static WeaponStats RPG() {
            WeaponStats stats = BaseStats();
            stats.Damage *= 5f;
            stats.BulletSpeed *= 0.7f;
            stats.FireRate *= 0.2f;
            return stats;
        }
    }
}