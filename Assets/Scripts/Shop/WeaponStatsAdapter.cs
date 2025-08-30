using Assets.Scripts.Weapon;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Adapter that bridges the existing WeaponStats class with our new IWeaponStats interface.
    /// Uses Adapter pattern to add upgrade capabilities without modifying existing code.
    /// </summary>
    public class WeaponStatsAdapter : IWeaponStats
    {
        private readonly WeaponStats baseStats;
        private readonly List<StatModifier> appliedModifiers;
        private readonly bool debugMode;
        
        // Store calculated values to avoid recalculation
        private float calculatedDamage;
        private float calculatedRange;
        private float calculatedFireRate;
        private float calculatedBulletSpeed;
        private int calculatedMagazineCapacity;
        private float calculatedAccuracy;
        private float calculatedRecoil;
        private float calculatedReloadSpeed;
        private int calculatedExtraMagazines;
        
        // Flag to track if stats need recalculation
        private bool needsRecalculation = true;
        
        public WeaponStatsAdapter(WeaponStats originalStats, bool debugMode = false)
        {
            this.baseStats = originalStats ?? throw new System.ArgumentNullException(nameof(originalStats));
            this.appliedModifiers = new List<StatModifier>();
            this.debugMode = debugMode;
            
            // Initialize extended stats with default values
            calculatedAccuracy = 100f; // 100% accuracy base
            calculatedRecoil = 0f;     // 0% recoil base
            calculatedReloadSpeed = 1f; // 1x reload speed base
            calculatedExtraMagazines = originalStats.ExtraMagazines; // Use base extra magazines
            
            RecalculateStats();
        }
        
        #region IWeaponStats Properties
        
        public float Damage 
        { 
            get 
            { 
                if (needsRecalculation) RecalculateStats();
                return calculatedDamage; 
            }
            set 
            { 
                calculatedDamage = value;
                needsRecalculation = false; // Direct set, no recalculation needed
            }
        }
        
        public float Range 
        { 
            get 
            { 
                if (needsRecalculation) RecalculateStats();
                return calculatedRange; 
            }
            set 
            { 
                calculatedRange = value;
                needsRecalculation = false;
            }
        }
        
        public float FireRate 
        { 
            get 
            { 
                if (needsRecalculation) RecalculateStats();
                return calculatedFireRate; 
            }
            set 
            { 
                calculatedFireRate = value;
                needsRecalculation = false;
            }
        }
        
        public float BulletSpeed 
        { 
            get 
            { 
                if (needsRecalculation) RecalculateStats();
                return calculatedBulletSpeed; 
            }
            set 
            { 
                calculatedBulletSpeed = value;
                needsRecalculation = false;
            }
        }
        
        public int MagazineCapacity 
        { 
            get 
            { 
                if (needsRecalculation) RecalculateStats();
                return calculatedMagazineCapacity; 
            }
            set 
            { 
                calculatedMagazineCapacity = value;
                needsRecalculation = false;
            }
        }
        
        public float Accuracy 
        { 
            get 
            { 
                if (needsRecalculation) RecalculateStats();
                return calculatedAccuracy; 
            }
            set 
            { 
                calculatedAccuracy = value;
                needsRecalculation = false;
            }
        }
        
        public float Recoil 
        { 
            get 
            { 
                if (needsRecalculation) RecalculateStats();
                return calculatedRecoil; 
            }
            set 
            { 
                calculatedRecoil = value;
                needsRecalculation = false;
            }
        }
        
        public float ReloadSpeed 
        { 
            get 
            { 
                if (needsRecalculation) RecalculateStats();
                return calculatedReloadSpeed; 
            }
            set 
            { 
                calculatedReloadSpeed = value;
                needsRecalculation = false;
            }
        }
        
        public int ExtraMagazines 
        { 
            get 
            { 
                if (needsRecalculation) RecalculateStats();
                return calculatedExtraMagazines; 
            }
            set 
            { 
                calculatedExtraMagazines = value;
                needsRecalculation = false;
            }
        }
        
        #endregion
        
        /// <summary>
        /// Gets the original WeaponStats object (for backward compatibility)
        /// </summary>
        public WeaponStats OriginalStats => baseStats;
        
        /// <summary>
        /// Applies an upgrade modifier to this weapon
        /// </summary>
        /// <param name="modifier">Stat modifier to apply</param>
        public void ApplyModifier(StatModifier modifier)
        {
            if (modifier == null)
            {
                Debug.LogError("Cannot apply null modifier");
                return;
            }
            
            appliedModifiers.Add(modifier);
            needsRecalculation = true;
            
            if (debugMode)
            {
                Debug.Log($"Applied modifier: {modifier.statType} {modifier.modifierType} {modifier.value}");
            }
        }
        
        /// <summary>
        /// Applies multiple upgrade modifiers
        /// </summary>
        /// <param name="modifiers">List of modifiers to apply</param>
        public void ApplyModifiers(IEnumerable<StatModifier> modifiers)
        {
            if (modifiers == null)
                return;
            
            foreach (var modifier in modifiers)
            {
                if (modifier != null)
                {
                    appliedModifiers.Add(modifier);
                }
            }
            
            needsRecalculation = true;
            
            if (debugMode)
            {
                Debug.Log($"Applied {appliedModifiers.Count} modifiers");
            }
        }
        
        /// <summary>
        /// Removes all applied modifiers and recalculates stats
        /// </summary>
        public void ClearModifiers()
        {
            appliedModifiers.Clear();
            needsRecalculation = true;
            
            if (debugMode)
            {
                Debug.Log("Cleared all modifiers");
            }
        }
        
        /// <summary>
        /// Forces recalculation of all stats based on base stats + modifiers
        /// </summary>
        public void RecalculateStats()
        {
            // Start with base stats
            calculatedDamage = baseStats.Damage;
            calculatedRange = baseStats.Range;
            calculatedFireRate = baseStats.FireRate;
            calculatedBulletSpeed = baseStats.BulletSpeed;
            calculatedMagazineCapacity = baseStats.MagazineCapacity;
            
            // Reset extended stats to defaults
            calculatedAccuracy = 100f;
            calculatedRecoil = 0f;
            calculatedReloadSpeed = 1f;
            calculatedExtraMagazines = baseStats.ExtraMagazines;
            
            // Apply all modifiers
            foreach (var modifier in appliedModifiers)
            {
                ApplyModifierToStats(modifier);
            }
            
            needsRecalculation = false;
            
            if (debugMode)
            {
                LogStatsBreakdown();
            }
        }
        
        /// <summary>
        /// Applies a single modifier to the calculated stats
        /// </summary>
        /// <param name="modifier">Modifier to apply</param>
        private void ApplyModifierToStats(StatModifier modifier)
        {
            switch (modifier.statType)
            {
                case StatType.Damage:
                    calculatedDamage = ApplyModifierValue(calculatedDamage, modifier.value, modifier.modifierType);
                    break;
                case StatType.Range:
                    calculatedRange = ApplyModifierValue(calculatedRange, modifier.value, modifier.modifierType);
                    break;
                case StatType.FireRate:
                    calculatedFireRate = ApplyModifierValue(calculatedFireRate, modifier.value, modifier.modifierType);
                    break;
                case StatType.BulletSpeed:
                    calculatedBulletSpeed = ApplyModifierValue(calculatedBulletSpeed, modifier.value, modifier.modifierType);
                    break;
                case StatType.MagazineCapacity:
                    calculatedMagazineCapacity = Mathf.RoundToInt(ApplyModifierValue(calculatedMagazineCapacity, modifier.value, modifier.modifierType));
                    break;
                case StatType.Accuracy:
                    calculatedAccuracy = ApplyModifierValue(calculatedAccuracy, modifier.value, modifier.modifierType);
                    break;
                case StatType.Recoil:
                    calculatedRecoil = ApplyModifierValue(calculatedRecoil, modifier.value, modifier.modifierType);
                    break;
                case StatType.ReloadSpeed:
                    calculatedReloadSpeed = ApplyModifierValue(calculatedReloadSpeed, modifier.value, modifier.modifierType);
                    break;
                case StatType.ExtraMagazines:
                    calculatedExtraMagazines = Mathf.RoundToInt(ApplyModifierValue(calculatedExtraMagazines, modifier.value, modifier.modifierType));
                    break;
            }
        }
        
        /// <summary>
        /// Applies a modifier value based on its type
        /// </summary>
        /// <param name="currentValue">Current stat value</param>
        /// <param name="modifierValue">Modifier value</param>
        /// <param name="modifierType">Type of modification</param>
        /// <returns>Modified value</returns>
        private float ApplyModifierValue(float currentValue, float modifierValue, ModifierType modifierType)
        {
            return modifierType switch
            {
                ModifierType.Add => currentValue + modifierValue,
                ModifierType.Multiply => currentValue * modifierValue,
                ModifierType.Percentage => currentValue * (1f + modifierValue / 100f),
                ModifierType.Set => modifierValue,
                _ => currentValue
            };
        }
        
        /// <summary>
        /// Synchronizes the original WeaponStats object with our calculated values
        /// This allows existing code to continue working unchanged
        /// </summary>
        public void SyncToOriginalStats()
        {
            if (needsRecalculation)
                RecalculateStats();
            
            baseStats.Damage = calculatedDamage;
            baseStats.Range = calculatedRange;
            baseStats.FireRate = calculatedFireRate;
            baseStats.BulletSpeed = calculatedBulletSpeed;
            baseStats.MagazineCapacity = calculatedMagazineCapacity;
            baseStats.ExtraMagazines = calculatedExtraMagazines;
            
            if (debugMode)
            {
                Debug.Log("Synchronized adapter stats to original WeaponStats object");
            }
        }
        
        /// <summary>
        /// Creates a copy of the current stats as a new WeaponStats object
        /// </summary>
        /// <returns>New WeaponStats with current calculated values</returns>
        public WeaponStats ToWeaponStats()
        {
            if (needsRecalculation)
                RecalculateStats();
            
            return new WeaponStats(
                calculatedDamage,
                calculatedRange,
                calculatedFireRate,
                calculatedBulletSpeed,
                calculatedMagazineCapacity
            );
        }
        
        /// <summary>
        /// Gets information about all applied modifiers
        /// </summary>
        /// <returns>List of modifier descriptions</returns>
        public List<string> GetModifierDescriptions()
        {
            var descriptions = new List<string>();
            
            foreach (var modifier in appliedModifiers)
            {
                if (!string.IsNullOrEmpty(modifier.description))
                {
                    descriptions.Add(modifier.description);
                }
                else
                {
                    descriptions.Add($"{modifier.statType} {modifier.modifierType} {modifier.value}");
                }
            }
            
            return descriptions;
        }
        
        /// <summary>
        /// Gets the number of modifiers applied
        /// </summary>
        public int GetModifierCount() => appliedModifiers.Count;
        
        /// <summary>
        /// Calculates DPS (Damage Per Second) based on current stats
        /// </summary>
        public float CalculateDPS()
        {
            if (needsRecalculation)
                RecalculateStats();
            
            return calculatedDamage * calculatedFireRate;
        }
        
        /// <summary>
        /// Calculates effective magazine size considering reload speed
        /// </summary>
        public float CalculateEffectiveMagazineSize()
        {
            if (needsRecalculation)
                RecalculateStats();
            
            // Higher reload speed = larger effective magazine
            return calculatedMagazineCapacity * calculatedReloadSpeed;
        }
        
        /// <summary>
        /// For debugging - logs detailed stats breakdown
        /// </summary>
        private void LogStatsBreakdown()
        {
            Debug.Log($"=== Weapon Stats Breakdown ===");
            Debug.Log($"Base Stats - Damage: {baseStats.Damage}, Range: {baseStats.Range}, FireRate: {baseStats.FireRate}");
            Debug.Log($"Final Stats - Damage: {calculatedDamage}, Range: {calculatedRange}, FireRate: {calculatedFireRate}");
            Debug.Log($"Extended Stats - Accuracy: {calculatedAccuracy}%, Recoil: {calculatedRecoil}, ReloadSpeed: {calculatedReloadSpeed}x");
            Debug.Log($"DPS: {CalculateDPS():F1}, Effective Magazine: {CalculateEffectiveMagazineSize():F1}");
            Debug.Log($"Applied Modifiers: {appliedModifiers.Count}");
        }
        
        /// <summary>
        /// Static factory method to create adapter from existing WeaponStats
        /// </summary>
        /// <param name="originalStats">Original weapon stats</param>
        /// <param name="debugMode">Enable debug logging</param>
        /// <returns>New WeaponStatsAdapter</returns>
        public static WeaponStatsAdapter FromWeaponStats(WeaponStats originalStats, bool debugMode = false)
        {
            return new WeaponStatsAdapter(originalStats, debugMode);
        }
        
        /// <summary>
        /// Static factory method to create adapter with initial modifiers
        /// </summary>
        /// <param name="originalStats">Original weapon stats</param>
        /// <param name="initialModifiers">Modifiers to apply immediately</param>
        /// <param name="debugMode">Enable debug logging</param>
        /// <returns>New WeaponStatsAdapter with modifiers applied</returns>
        public static WeaponStatsAdapter FromWeaponStatsWithModifiers(
            WeaponStats originalStats, 
            IEnumerable<StatModifier> initialModifiers, 
            bool debugMode = false)
        {
            var adapter = new WeaponStatsAdapter(originalStats, debugMode);
            adapter.ApplyModifiers(initialModifiers);
            return adapter;
        }
    }
    
    /// <summary>
    /// Extension methods for WeaponStats to easily create adapters
    /// </summary>
    public static class WeaponStatsExtensions
    {
        /// <summary>
        /// Converts WeaponStats to WeaponStatsAdapter
        /// </summary>
        /// <param name="stats">Original stats</param>
        /// <param name="debugMode">Enable debug logging</param>
        /// <returns>WeaponStatsAdapter</returns>
        public static WeaponStatsAdapter ToAdapter(this WeaponStats stats, bool debugMode = false)
        {
            return WeaponStatsAdapter.FromWeaponStats(stats, debugMode);
        }
        
        /// <summary>
        /// Creates an adapter with initial modifiers
        /// </summary>
        /// <param name="stats">Original stats</param>
        /// <param name="modifiers">Initial modifiers</param>
        /// <param name="debugMode">Enable debug logging</param>
        /// <returns>WeaponStatsAdapter</returns>
        public static WeaponStatsAdapter ToAdapterWithModifiers(
            this WeaponStats stats, 
            IEnumerable<StatModifier> modifiers, 
            bool debugMode = false)
        {
            return WeaponStatsAdapter.FromWeaponStatsWithModifiers(stats, modifiers, debugMode);
        }
    }
}