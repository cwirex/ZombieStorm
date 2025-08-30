using Assets.Scripts.Weapon;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Interface for objects that can be upgraded through the shop system
    /// </summary>
    public interface IUpgradeable
    {
        bool CanUpgrade();
        int GetUpgradeCost();
        void ApplyUpgrade();
        int GetCurrentLevel();
        int GetMaxLevel();
    }

    /// <summary>
    /// Strategy pattern for calculating dynamic prices
    /// </summary>
    public interface IPricingStrategy
    {
        int CalculatePrice(int currentQuantity);
    }

    /// <summary>
    /// Repository pattern for managing weapon upgrade paths
    /// </summary>
    public interface IWeaponUpgradeRepository
    {
        IWeaponUpgradePath GetUpgradePath(EWeapons weaponType);
        void RegisterUpgradePath(EWeapons weaponType, IWeaponUpgradePath path);
    }

    /// <summary>
    /// Defines the complete upgrade path for a weapon (10 levels)
    /// </summary>
    public interface IWeaponUpgradePath
    {
        IWeaponUpgrade GetUpgradeForLevel(int level);
        int GetMaxLevel();
        bool HasUltimateAbility();
        IUltimateAbility GetUltimateAbility();
    }

    /// <summary>
    /// Individual upgrade that can be applied to a weapon
    /// </summary>
    public interface IWeaponUpgrade
    {
        int GetCostPercentage();
        string GetDescription();
        void ApplyTo(Assets.Scripts.Weapon.IWeaponStats stats);
    }


    /// <summary>
    /// Ultimate ability that activates at weapon level 10
    /// </summary>
    public interface IUltimateAbility
    {
        string Name { get; }
        string Description { get; }
        void Activate(IWeapon weapon);
        void Deactivate(IWeapon weapon);
        bool IsActive { get; }
    }

    /// <summary>
    /// Extended weapon interface for ultimate abilities
    /// </summary>
    public interface IWeapon
    {
        EWeapons WeaponType { get; }
        Assets.Scripts.Weapon.IWeaponStats Stats { get; }
        bool HasUltimateAbility { get; }
        IUltimateAbility UltimateAbility { get; }
    }

    /// <summary>
    /// Defines types of stats that can be modified
    /// Based on NEW_SHOP_INVENTORY.md specifications
    /// </summary>
    public enum StatType
    {
        // Basic weapon stats
        Damage,
        Range, 
        FireRate,
        BulletSpeed,
        MagazineCapacity,
        Accuracy,
        Recoil,
        ReloadSpeed,
        
        // Weapon-specific stats
        PelletCount,        // Shotgun pellets
        FlameWidth,         // Flamethrower width
        BlastRadius,        // RPG blast radius
        ProjectileSpeed,    // RPG rocket speed
        AmmoEfficiency,     // Flamethrower fuel consumption
        
        // Special abilities
        CritChance,         // M4 critical hit chance
        ExecutionerBonus,   // AWP missing health bonus
        DamageRampup,       // M249 sustained fire bonus
        BurningPools,       // Flamethrower napalm effect
        StunChance,         // RPG/M249 stun effects
        Piercing,           // M249 bullet penetration
        
        // Magazine system
        ExtraMagazines      // Additional magazines for reserve ammo
    }

    /// <summary>
    /// Defines specific types of ultimate abilities
    /// Based on NEW_SHOP_INVENTORY.md specifications
    /// </summary>
    public enum UltimateAbilityType
    {
        // Pistol - 40% chance to fire twice
        DoubleTap,
        
        // UZI - 25% chance for +100% damage critical hit
        CriticalSpray,
        
        // Shotgun - Fires +2 additional pellets per shot
        ExtraPellets,
        
        // M4 - Each shot deals +1% of target's max health as bonus damage
        ArmorPiercer,
        
        // AWP - Shots deal bonus damage equal to 25% of target's missing health
        ExecutionersMark,
        
        // LMG - +3% ammo regeneration on kill (Total: 4%)
        AmmunitionExpert,
        
        // Flamethrower - Damage increases by 3% for every second of continuous fire (Max +75%)
        SustainedBurn,
        
        // RPG7 - Blast radius +25% and briefly stuns all enemies hit
        ThermobaricBlast
    }

    /// <summary>
    /// Types of consumable items for dynamic pricing
    /// </summary>
    public enum ConsumableType
    {
        Medkit,
        TNT
    }
}