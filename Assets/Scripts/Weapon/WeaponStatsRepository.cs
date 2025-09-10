using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Repository that holds stats for each Weapon
    /// </summary>
    public static class WeaponStatsRepository { 

        private static WeaponStats BaseStats() {
            float dmg = 20f;
            float range = 3f;
            float fireRate = 1.8f;
            float bulletSpeed = 17f;
            int magazineCapacity = 10;
            return new WeaponStats(dmg, range, fireRate, bulletSpeed, magazineCapacity);
        }

        public static WeaponStats Pistol() {
            // Base Stats: 30 Dmg, 2.0 FR, 10 Mag, 3 Magazines = 60 DPS
            WeaponStats stats = BaseStats();
            stats.ExtraMagazines = 11; // More magazines for pistol (base weapon)
            return stats;
        }

        public static WeaponStats Rifle() {
            WeaponStats stats = BaseStats();
            stats.Damage = 46f;
            stats.FireRate = 4.2f;
            stats.BulletSpeed = 20f;
            stats.MagazineCapacity = 25;
            stats.Range = 8f;
            stats.ExtraMagazines = 3; // 3 total magazines (1 + 2 extra)
            return stats;
        }

        public static WeaponStats Shotgun() {
            // Shotgun Base Stats: 25 Dmg, 8 Pellets, 0.85 FR, 7 Mag, 3 Magazines = 200 burst
            WeaponStats stats = BaseStats();
            stats.Damage = 18f;   // Per pellet (8 pellets = 200 total)
            stats.FireRate = 0.8f;
            stats.BulletSpeed = 15f;
            stats.Range = 3f;
            stats.MagazineCapacity = 7;
            stats.ExtraMagazines = 3; // 3 total magazines (1 + 2 extra)
            return stats;
        }

        public static WeaponStats SniperRifle() {
            // AWP Base Stats: 400 Dmg, 0.4 FR, 5 Mag, 2 Magazines
            WeaponStats stats = BaseStats();
            stats.Damage = 200f;
            stats.FireRate = 0.4f;
            stats.BulletSpeed = 0f; // Hitscan
            stats.Range = 20f;
            stats.MagazineCapacity = 5;
            stats.ExtraMagazines = 3; // 2 total magazines (1 + 1 extra)
            return stats;
        }

        public static WeaponStats SMG() {
            // UZI Base Stats: 22 Dmg, 11.0 FR, 24 Mag, 4 Magazines = 242 DPS
            WeaponStats stats = BaseStats();
            stats.Damage = 13.3f;
            stats.FireRate = 10.0f;
            stats.BulletSpeed = 18f;
            stats.Range = 4f;
            stats.MagazineCapacity = 28;
            stats.ExtraMagazines = 4; // 4 total magazines (1 + 3 extra)
            return stats;
        }

        public static WeaponStats M249() {
            // LMG Base Stats: 40 Dmg, 12.0 FR, 100 Mag, 2 Magazines = 480 DPS
            WeaponStats stats = BaseStats();
            stats.Damage = 36f;
            stats.FireRate = 8.8f;
            stats.BulletSpeed = 19f;
            stats.Range = 10f;
            stats.MagazineCapacity = 100;
            stats.ExtraMagazines = 2; // 2 total magazines (1 + 1 extra)
            return stats;
        }

        public static WeaponStats RPG() {
            // RPG Base Stats: 300 Dmg, 3m Radius, 4 Rockets, 3 Magazines
            WeaponStats stats = BaseStats();
            stats.Damage = 200f;
            stats.FireRate = 0.5f; // Slow fire rate
            stats.BulletSpeed = 11f;
            stats.Range = 15f;
            stats.MagazineCapacity = 3;
            stats.ExtraMagazines = 3; // 3 total magazines (1 + 2 extra)
            return stats;
        }
        
        public static WeaponStats Flamethrower() {
            // Flamethrower Base Stats: 20 Dmg/tick, 20.0 FR, 100 Fuel, 3 Canisters = 400 DPS
            WeaponStats stats = BaseStats();
            stats.Damage = 40f; // Per tick
            stats.FireRate = 30.0f; // Very fast ticks
            stats.BulletSpeed = 8f; // Flame speed
            stats.Range = 4f;
            stats.MagazineCapacity = 70; // Fuel capacity
            stats.ExtraMagazines = 3; // 3 total fuel canisters (1 + 2 extra)
            return stats;
        }
        
    }
}