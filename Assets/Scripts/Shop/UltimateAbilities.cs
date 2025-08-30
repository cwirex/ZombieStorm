using UnityEngine;
using System.Collections;
using Assets.Scripts.Weapon;

namespace Assets.Scripts.Shop
{
    /// <summary>
    /// Base class for all ultimate abilities
    /// Provides common functionality and structure
    /// </summary>
    public abstract class BaseUltimateAbility : MonoBehaviour, IUltimateAbility
    {
        [Header("Ability Info")]
        [SerializeField] protected string abilityName;
        [TextArea(3, 5)]
        [SerializeField] protected string description;
        [SerializeField] protected UltimateAbilityType abilityType;
        
        [Header("Visual & Audio")]
        [SerializeField] protected GameObject effectPrefab;
        [SerializeField] protected AudioClip activationSound;
        [SerializeField] protected Color effectColor = Color.white;
        
        protected bool isActive = false;
        protected IWeapon targetWeapon;
        
        public string Name => abilityName;
        public string Description => description;
        public bool IsActive => isActive;
        
        public virtual void Activate(IWeapon weapon)
        {
            if (isActive)
            {
                Debug.LogWarning($"{abilityName} is already active");
                return;
            }
            
            if (weapon == null)
            {
                Debug.LogError($"Cannot activate {abilityName}: weapon is null");
                return;
            }
            
            targetWeapon = weapon;
            isActive = true;
            
            PlayActivationEffects();
            OnActivate();
            
            Debug.Log($"Ultimate ability activated: {abilityName}");
        }
        
        public virtual void Deactivate(IWeapon weapon)
        {
            if (!isActive)
                return;
            
            OnDeactivate();
            isActive = false;
            targetWeapon = null;
            
            Debug.Log($"Ultimate ability deactivated: {abilityName}");
        }
        
        protected virtual void OnActivate() { }
        protected virtual void OnDeactivate() { }
        
        protected virtual void PlayActivationEffects()
        {
            // Play sound effect
            if (activationSound != null && Camera.main != null)
            {
                AudioSource.PlayClipAtPoint(activationSound, Camera.main.transform.position);
            }
            
            // Spawn visual effect
            if (effectPrefab != null && targetWeapon != null)
            {
                // Assuming weapon has a transform (this would need to be adapted to actual weapon structure)
                var weaponTransform = (targetWeapon as MonoBehaviour)?.transform;
                if (weaponTransform != null)
                {
                    Instantiate(effectPrefab, weaponTransform.position, weaponTransform.rotation);
                }
            }
        }
    }
    
    /// <summary>
    /// PISTOL Ultimate: Akimbo - Gain second pistol, double magazine & fire rate
    /// </summary>
    public class AkimboUltimate : BaseUltimateAbility
    {
        [Header("Akimbo Settings")]
        [SerializeField] private GameObject secondPistolPrefab;
        [SerializeField] private Transform secondPistolMount;
        
        private GameObject secondPistolInstance;
        private float originalFireRate;
        private int originalMagazineCapacity;
        
        protected override void OnActivate()
        {
            if (targetWeapon?.Stats == null)
                return;
            
            // Store original values
            originalFireRate = targetWeapon.Stats.FireRate;
            originalMagazineCapacity = targetWeapon.Stats.MagazineCapacity;
            
            // Double fire rate and magazine capacity
            targetWeapon.Stats.FireRate *= 2f;
            targetWeapon.Stats.MagazineCapacity *= 2;
            
            // Spawn second pistol visual (if prefab is set)
            SpawnSecondPistol();
        }
        
        protected override void OnDeactivate()
        {
            if (targetWeapon?.Stats == null)
                return;
            
            // Restore original values
            targetWeapon.Stats.FireRate = originalFireRate;
            targetWeapon.Stats.MagazineCapacity = originalMagazineCapacity;
            
            // Remove second pistol
            DestroySecondPistol();
        }
        
        private void SpawnSecondPistol()
        {
            if (secondPistolPrefab != null && secondPistolMount != null)
            {
                secondPistolInstance = Instantiate(secondPistolPrefab, secondPistolMount);
                secondPistolInstance.transform.localPosition = Vector3.zero;
                secondPistolInstance.transform.localRotation = Quaternion.identity;
            }
        }
        
        private void DestroySecondPistol()
        {
            if (secondPistolInstance != null)
            {
                Destroy(secondPistolInstance);
                secondPistolInstance = null;
            }
        }
    }
    
    /// <summary>
    /// UZI Ultimate: Hollow-Point Fury - Shots slow enemies by 30% for 1 second
    /// </summary>
    public class HollowPointFuryUltimate : BaseUltimateAbility
    {
        [Header("Hollow-Point Settings")]
        [SerializeField] private float slowPercentage = 0.3f;
        [SerializeField] private float slowDuration = 1f;
        
        protected override void OnActivate()
        {
            // This would typically hook into the weapon's bullet hit system
            // For now, we'll set a flag that the weapon can check
            Debug.Log($"Hollow-Point Fury active: {slowPercentage * 100}% slow for {slowDuration}s");
        }
        
        public float GetSlowPercentage() => slowPercentage;
        public float GetSlowDuration() => slowDuration;
    }
    
    /// <summary>
    /// SHOTGUN Ultimate: Overwhelm Shells - Fire +2 additional pellets per shot
    /// </summary>
    public class OverwhelmShellsUltimate : BaseUltimateAbility
    {
        [Header("Overwhelm Settings")]
        [SerializeField] private int additionalPellets = 2;
        
        protected override void OnActivate()
        {
            Debug.Log($"Overwhelm Shells active: +{additionalPellets} pellets per shot");
        }
        
        public int GetAdditionalPellets() => additionalPellets;
    }
    
    /// <summary>
    /// M4 Ultimate: Headhunter Rounds - 10% chance for critical hits (+200% damage)
    /// </summary>
    public class HeadhunterRoundsUltimate : BaseUltimateAbility
    {
        [Header("Headhunter Settings")]
        [SerializeField] private float critChance = 0.1f;      // 10%
        [SerializeField] private float critMultiplier = 3f;    // +200% = 3x total
        
        protected override void OnActivate()
        {
            Debug.Log($"Headhunter Rounds active: {critChance * 100}% crit chance for {critMultiplier}x damage");
        }
        
        /// <summary>
        /// Checks if this shot should be a critical hit
        /// </summary>
        /// <returns>True if critical hit should occur</returns>
        public bool ShouldCrit()
        {
            if (!isActive)
                return false;
            
            return Random.value <= critChance;
        }
        
        public float GetCritMultiplier() => critMultiplier;
    }
    
    /// <summary>
    /// FLAMETHROWER Ultimate: Napalm Canister - Flames leave burning ground pools
    /// </summary>
    public class NapalmCanisterUltimate : BaseUltimateAbility
    {
        [Header("Napalm Settings")]
        [SerializeField] private GameObject napalmPoolPrefab;
        [SerializeField] private float poolDuration = 3f;
        [SerializeField] private float poolDamage = 10f;
        
        protected override void OnActivate()
        {
            Debug.Log($"Napalm Canister active: Creates burning pools for {poolDuration}s");
        }
        
        /// <summary>
        /// Creates a napalm pool at the specified position
        /// </summary>
        /// <param name="position">Position to create pool</param>
        public void CreateNapalmPool(Vector3 position)
        {
            if (!isActive || napalmPoolPrefab == null)
                return;
            
            var pool = Instantiate(napalmPoolPrefab, position, Quaternion.identity);
            
            // Set up pool duration and damage
            var poolComponent = pool.GetComponent<NapalmPool>();
            if (poolComponent != null)
            {
                poolComponent.Initialize(poolDuration, poolDamage);
            }
            else
            {
                // Fallback: destroy after duration
                Destroy(pool, poolDuration);
            }
        }
    }
    
    /// <summary>
    /// AWP Ultimate: Executioner's Mark - Bonus damage = 25% of target's missing health
    /// </summary>
    public class ExecutionersMarkUltimate : BaseUltimateAbility
    {
        [Header("Executioner Settings")]
        [SerializeField] private float missingHealthMultiplier = 0.25f; // 25%
        
        protected override void OnActivate()
        {
            Debug.Log($"Executioner's Mark active: +{missingHealthMultiplier * 100}% missing health as bonus damage");
        }
        
        /// <summary>
        /// Calculates bonus damage based on target's missing health
        /// </summary>
        /// <param name="currentHealth">Target's current health</param>
        /// <param name="maxHealth">Target's maximum health</param>
        /// <returns>Bonus damage to add</returns>
        public float CalculateBonusDamage(float currentHealth, float maxHealth)
        {
            if (!isActive || maxHealth <= 0)
                return 0f;
            
            float missingHealth = maxHealth - currentHealth;
            return missingHealth * missingHealthMultiplier;
        }
    }
    
    /// <summary>
    /// M249 Ultimate: Sustained Barrage - +2% damage per second of continuous fire (max +50%)
    /// </summary>
    public class SustainedBarrageUltimate : BaseUltimateAbility
    {
        [Header("Sustained Barrage Settings")]
        [SerializeField] private float damageIncreasePerSecond = 0.02f; // 2%
        [SerializeField] private float maxDamageIncrease = 0.5f;        // 50%
        
        private float continuousFiringTime = 0f;
        private bool isFiring = false;
        
        protected override void OnActivate()
        {
            Debug.Log($"Sustained Barrage active: +{damageIncreasePerSecond * 100}% damage per second (max +{maxDamageIncrease * 100}%)");
            continuousFiringTime = 0f;
        }
        
        protected override void OnDeactivate()
        {
            continuousFiringTime = 0f;
            isFiring = false;
        }
        
        private void Update()
        {
            if (!isActive)
                return;
            
            if (isFiring)
            {
                continuousFiringTime += Time.deltaTime;
            }
            else
            {
                // Reset if not firing
                continuousFiringTime = 0f;
            }
        }
        
        /// <summary>
        /// Call this when weapon starts firing
        /// </summary>
        public void StartFiring()
        {
            isFiring = true;
        }
        
        /// <summary>
        /// Call this when weapon stops firing
        /// </summary>
        public void StopFiring()
        {
            isFiring = false;
            continuousFiringTime = 0f;
        }
        
        /// <summary>
        /// Gets current damage multiplier based on continuous firing time
        /// </summary>
        /// <returns>Damage multiplier (1.0 = no bonus, 1.5 = +50%)</returns>
        public float GetDamageMultiplier()
        {
            if (!isActive)
                return 1f;
            
            float bonusMultiplier = continuousFiringTime * damageIncreasePerSecond;
            bonusMultiplier = Mathf.Min(bonusMultiplier, maxDamageIncrease);
            
            return 1f + bonusMultiplier;
        }
    }
    
    /// <summary>
    /// RPG Ultimate: Thermobaric Warhead - +25% blast radius, stuns all enemies hit
    /// </summary>
    public class ThermobaricWarheadUltimate : BaseUltimateAbility
    {
        [Header("Thermobaric Settings")]
        [SerializeField] private float radiusMultiplier = 1.25f; // +25%
        [SerializeField] private float stunDuration = 2f;
        
        protected override void OnActivate()
        {
            Debug.Log($"Thermobaric Warhead active: {radiusMultiplier}x blast radius, {stunDuration}s stun");
        }
        
        public float GetRadiusMultiplier() => radiusMultiplier;
        public float GetStunDuration() => stunDuration;
    }
    
    /// <summary>
    /// Component for napalm pools created by the flamethrower ultimate
    /// </summary>
    public class NapalmPool : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float duration = 3f;
        [SerializeField] private float damageInterval = 0.5f; // Damage every 0.5 seconds
        
        private float remainingDuration;
        private float nextDamageTime;
        
        public void Initialize(float poolDuration, float poolDamage)
        {
            duration = poolDuration;
            damage = poolDamage;
            remainingDuration = duration;
            nextDamageTime = 0f;
        }
        
        private void Update()
        {
            remainingDuration -= Time.deltaTime;
            
            if (remainingDuration <= 0f)
            {
                Destroy(gameObject);
                return;
            }
            
            // Handle damage to enemies in range (simplified)
            if (Time.time >= nextDamageTime)
            {
                DealDamageToEnemiesInRange();
                nextDamageTime = Time.time + damageInterval;
            }
        }
        
        private void DealDamageToEnemiesInRange()
        {
            // This would need to be implemented based on your enemy system
            // For now, it's just a placeholder
            Collider[] enemies = Physics.OverlapSphere(transform.position, 1f);
            foreach (var enemy in enemies)
            {
                if (enemy.CompareTag("Enemy"))
                {
                    // Deal damage to enemy - would need to implement based on your enemy system
                    // This is a placeholder that would need to match your actual IDamageable interface
                    Debug.Log($"Would deal {damage} damage to {enemy.name}");
                    // var damageable = enemy.GetComponent<IDamageable>();
                    // damageable?.Damage(damage);
                }
            }
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}