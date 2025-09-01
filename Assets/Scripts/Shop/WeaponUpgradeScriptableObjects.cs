using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// ScriptableObject that defines a stat modifier for weapon upgrades
    /// </summary>
    [System.Serializable]
    public class StatModifier
    {
        [Header("Stat Modification")]
        public StatType statType;
        public ModifierType modifierType;
        public float value;
        
        [Header("Description")]
        [TextArea(2, 3)]
        public string description;
        
        /// <summary>
        /// Applies this modifier to the given stats
        /// </summary>
        /// <param name="stats">Stats to modify</param>
        public void Apply(Assets.Scripts.Weapon.IWeaponStats stats)
        {
            switch (statType)
            {
                case StatType.Damage:
                    stats.Damage = ApplyModifier(stats.Damage, value, modifierType);
                    break;
                case StatType.Range:
                    stats.Range = ApplyModifier(stats.Range, value, modifierType);
                    break;
                case StatType.FireRate:
                    stats.FireRate = ApplyModifier(stats.FireRate, value, modifierType);
                    break;
                case StatType.BulletSpeed:
                    stats.BulletSpeed = ApplyModifier(stats.BulletSpeed, value, modifierType);
                    break;
                case StatType.MagazineCapacity:
                    stats.MagazineCapacity = Mathf.RoundToInt(ApplyModifier(stats.MagazineCapacity, value, modifierType));
                    break;
                case StatType.Accuracy:
                    stats.Accuracy = ApplyModifier(stats.Accuracy, value, modifierType);
                    break;
                case StatType.Recoil:
                    stats.Recoil = ApplyModifier(stats.Recoil, value, modifierType);
                    break;
                case StatType.ReloadSpeed:
                    stats.ReloadSpeed = ApplyModifier(stats.ReloadSpeed, value, modifierType);
                    break;
                case StatType.ExtraMagazines:
                    stats.ExtraMagazines = Mathf.RoundToInt(ApplyModifier(stats.ExtraMagazines, value, modifierType));
                    break;
            }
        }
        
        private float ApplyModifier(float currentValue, float modifierValue, ModifierType type)
        {
            return type switch
            {
                ModifierType.Add => currentValue + modifierValue,
                ModifierType.Multiply => currentValue * modifierValue,
                ModifierType.Percentage => currentValue * (1f + modifierValue / 100f),
                ModifierType.Set => modifierValue,
                _ => currentValue
            };
        }
    }
    
    /// <summary>
    /// Types of modifiers that can be applied to stats
    /// </summary>
    public enum ModifierType
    {
        Add,        // Add flat value
        Multiply,   // Multiply by value
        Percentage, // Add percentage (10 = +10%)
        Set         // Set to specific value
    }
    
}