using UnityEngine;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// ScriptableObject that defines an ultimate ability for level 10 weapons
    /// </summary>
    [CreateAssetMenu(fileName = "UltimateAbility", menuName = "Shop/Ultimate Ability")]
    public class UltimateAbilitySO : ScriptableObject, IUltimateAbility
    {
        [Header("Ability Info")]
        public string abilityName;
        [TextArea(3, 5)]
        public string description;
        public UltimateAbilityType abilityType;
        
        [Header("Ability Parameters")]
        public float triggerChance = 1.0f; // For OnHit abilities (1.0 = always, 0.1 = 10% chance)
        public float effectDuration = 0f;   // For timed effects
        public float effectValue = 0f;      // General purpose value
        public float effectRadius = 0f;     // For area effects
        
        [Header("Visual & Audio")]
        public GameObject effectPrefab;
        public AudioClip activationSound;
        public Color effectColor = Color.white;
        public Sprite abilityIcon;
        
        private bool isActive = false;
        
        public string Name => abilityName;
        public string Description => description;
        public bool IsActive => isActive;
        
        public void Activate(IWeapon weapon)
        {
            if (weapon == null)
            {
                Debug.LogError($"Cannot activate {abilityName}: weapon is null");
                return;
            }
            
            if (isActive)
            {
                Debug.LogWarning($"{abilityName} is already active");
                return;
            }
            
            isActive = true;
            
            // Play activation sound
            if (activationSound != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(activationSound, Camera.main.transform.position);
            }
            
            Debug.Log($"Ultimate ability activated: {abilityName} for {weapon.WeaponType}");
            
            // Specific activation logic would be handled by the weapon implementation
            // This is a base implementation that can be overridden
        }
        
        public void Deactivate(IWeapon weapon)
        {
            if (!isActive)
                return;
            
            isActive = false;
            Debug.Log($"Ultimate ability deactivated: {abilityName} for {weapon?.WeaponType}");
        }
    }
}