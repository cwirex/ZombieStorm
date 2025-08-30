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
            // Base Stats: 30 Dmg, 2.0 FR, 10 Mag, 3 Magazines = 60 DPS
            WeaponStats stats = BaseStats();
            stats.ExtraMagazines = 2; // 3 total magazines (1 + 2 extra)
            return stats;
        }

        public static WeaponStats Rifle() {
            // M4 Base Stats: 55 Dmg, 7.0 FR, 30 Mag, 3 Magazines = 385 DPS
            WeaponStats stats = BaseStats();
            stats.Damage = 55f;
            stats.FireRate = 7.0f;
            stats.BulletSpeed = 25f;
            stats.MagazineCapacity = 30;
            stats.Range = 8f;
            stats.ExtraMagazines = 2; // 3 total magazines (1 + 2 extra)
            return stats;
        }

        public static WeaponStats Shotgun() {
            // Shotgun Base Stats: 25 Dmg, 8 Pellets, 0.85 FR, 7 Mag, 3 Magazines = 200 burst
            WeaponStats stats = BaseStats();
            stats.Damage = 25f;   // Per pellet (8 pellets = 200 total)
            stats.FireRate = 0.85f;
            stats.BulletSpeed = 15f;
            stats.Range = 3f;
            stats.MagazineCapacity = 7;
            stats.ExtraMagazines = 2; // 3 total magazines (1 + 2 extra)
            return stats;
        }

        public static WeaponStats SniperRifle() {
            // AWP Base Stats: 400 Dmg, 0.4 FR, 5 Mag, 2 Magazines
            WeaponStats stats = BaseStats();
            stats.Damage = 400f;
            stats.FireRate = 0.4f;
            stats.BulletSpeed = 0f; // Hitscan
            stats.Range = 100f;
            stats.MagazineCapacity = 5;
            stats.ExtraMagazines = 1; // 2 total magazines (1 + 1 extra)
            return stats;
        }

        public static WeaponStats SMG() {
            // UZI Base Stats: 22 Dmg, 11.0 FR, 24 Mag, 4 Magazines = 242 DPS
            WeaponStats stats = BaseStats();
            stats.Damage = 22f;
            stats.FireRate = 11.0f;
            stats.BulletSpeed = 18f;
            stats.Range = 4f;
            stats.MagazineCapacity = 24;
            stats.ExtraMagazines = 3; // 4 total magazines (1 + 3 extra)
            return stats;
        }

        public static WeaponStats M249() {
            // LMG Base Stats: 40 Dmg, 12.0 FR, 100 Mag, 2 Magazines = 480 DPS
            WeaponStats stats = BaseStats();
            stats.Damage = 40f;
            stats.FireRate = 12.0f;
            stats.BulletSpeed = 22f;
            stats.Range = 10f;
            stats.MagazineCapacity = 100;
            stats.ExtraMagazines = 1; // 2 total magazines (1 + 1 extra)
            return stats;
        }

        public static WeaponStats RPG() {
            // RPG Base Stats: 300 Dmg, 3m Radius, 4 Rockets, 3 Magazines
            WeaponStats stats = BaseStats();
            stats.Damage = 300f;
            stats.FireRate = 0.5f; // Slow fire rate
            stats.BulletSpeed = 16f;
            stats.Range = 15f;
            stats.MagazineCapacity = 4;
            stats.ExtraMagazines = 2; // 3 total magazines (1 + 2 extra)
            return stats;
        }
        
        public static WeaponStats Flamethrower() {
            // Flamethrower Base Stats: 20 Dmg/tick, 20.0 FR, 100 Fuel, 3 Canisters = 400 DPS
            WeaponStats stats = BaseStats();
            stats.Damage = 20f; // Per tick
            stats.FireRate = 20.0f; // Very fast ticks
            stats.BulletSpeed = 8f; // Flame speed
            stats.Range = 4f;
            stats.MagazineCapacity = 100; // Fuel capacity
            stats.ExtraMagazines = 2; // 3 total fuel canisters (1 + 2 extra)
            return stats;
        }
        
    }
}