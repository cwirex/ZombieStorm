using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    /// <summary>
    /// Structure for holding Weapon Stats
    /// </summary>
    public class WeaponStats {
        public float Damage { get; set; }
        public float Range { get; set; }
        public float FireRate { get; set; }
        public float BulletSpeed { get; set; }
        public int MagazineCapacity { get; set; }

        public WeaponStats(float damage, float range, float fireRate, float bulletSpeed, int magazineCapacity) {
            Damage = damage;
            Range = range;
            FireRate = fireRate;
            BulletSpeed = bulletSpeed;
            MagazineCapacity = magazineCapacity;
        }
    }
}