using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Structure for holding Weapon Stats
    /// Implements IWeaponStats for compatibility with shop upgrade system
    /// </summary>
    public class WeaponStats : IWeaponStats {
        public float Damage { get; set; }
        public float Range { get; set; }
        public float FireRate { get; set; }
        public float BulletSpeed { get; set; }
        public int MagazineCapacity { get; set; }
        
        // Additional properties for upgrade system
        public float Accuracy { get; set; } = 1.0f;
        public float Recoil { get; set; } = 0.0f;
        public float ReloadSpeed { get; set; } = 1.0f;
        
        // Magazine system
        public int ExtraMagazines { get; set; } = 0;

        public WeaponStats(float damage, float range, float fireRate, float bulletSpeed, int magazineCapacity) {
            Damage = damage;
            Range = range;
            FireRate = fireRate;
            BulletSpeed = bulletSpeed;
            MagazineCapacity = magazineCapacity;
            
            // Set reasonable defaults
            Accuracy = 1.0f;
            Recoil = 0.0f;
            ReloadSpeed = 1.0f;
            ExtraMagazines = 0;
        }
        
        /// <summary>
        /// Copy constructor for creating upgraded versions
        /// </summary>
        public WeaponStats(WeaponStats source) {
            Damage = source.Damage;
            Range = source.Range;
            FireRate = source.FireRate;
            BulletSpeed = source.BulletSpeed;
            MagazineCapacity = source.MagazineCapacity;
            Accuracy = source.Accuracy;
            Recoil = source.Recoil;
            ReloadSpeed = source.ReloadSpeed;
            ExtraMagazines = source.ExtraMagazines;
        }
        
        /// <summary>
        /// Calculate DPS (Damage Per Second) for this weapon
        /// </summary>
        public float CalculateDPS() {
            return Damage * FireRate;
        }
    }
}