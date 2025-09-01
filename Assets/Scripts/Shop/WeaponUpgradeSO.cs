using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// ScriptableObject that defines a single weapon upgrade level
    /// </summary>
    [CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "Shop/Weapon Upgrade")]
    public class WeaponUpgradeSO : ScriptableObject, IWeaponUpgrade
    {
        [Header("Upgrade Info")]
        public int level;
        [Range(0, 100)]
        public int costPercentage;
        [TextArea(2, 4)]
        public string description;
        
        [Header("Stat Modifications")]
        public StatModifier[] statModifiers;
        
        [Header("Visual")]
        public Sprite upgradeIcon;
        public Color upgradeColor = Color.white;
        
        public int GetCostPercentage() => costPercentage;
        public string GetDescription() => description;
        
        public void ApplyTo(Assets.Scripts.Weapon.IWeaponStats stats)
        {
            if (stats == null)
            {
                Debug.LogError($"Cannot apply upgrade {name}: stats is null");
                return;
            }
            
            foreach (var modifier in statModifiers)
            {
                try
                {
                    modifier.Apply(stats);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error applying modifier in {name}: {ex.Message}");
                }
            }
        }
    }
}