using UnityEngine;

namespace Assets.Scripts.Weapon
{
    /// <summary>
    /// Extended weapon stats interface that supports upgrades
    /// </summary>
    public interface IWeaponStats
    {
        float Damage { get; set; }
        float Range { get; set; }
        float FireRate { get; set; }
        float BulletSpeed { get; set; }
        int MagazineCapacity { get; set; }
        
        // Additional properties for upgrade system
        float Accuracy { get; set; }
        float Recoil { get; set; }
        float ReloadSpeed { get; set; }
        
        // Magazine system
        int ExtraMagazines { get; set; }
    }
}