using Assets.Scripts.PlayerScripts;
using Assets.Scripts.Audio;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Weapon {
    public class Ammo {
        public int AmmoLeft { get; private set; }
        public int CurrentAmmoInMagazine { get; private set; }
        public int MagazineCapacity { get; set; }
        
        // Ammo regeneration properties for LMG/Flamethrower
        public float AmmoRegenPercentage { get; set; } = 0f; // Percentage of total ammo restored on kill
        private int maxAmmoCapacity = 0; // Store original max ammo for regeneration calculations
        
        // Reload state management
        public bool IsReloading { get; private set; } = false;
        private Coroutine reloadCoroutine;
        private Coroutine animationCoroutine;
        private MonoBehaviour coroutineRunner; // Need a MonoBehaviour to run coroutines
        private Transform weaponTransform; // For reload animation

        public static UIController UIController { get; set; }

        /// <summary>
        /// Sets the MonoBehaviour that will run reload coroutines
        /// </summary>
        public void SetCoroutineRunner(MonoBehaviour runner) {
            coroutineRunner = runner;
            weaponTransform = runner.transform; // Use the weapon's transform for animation
        }
        
        /// <summary>
        /// Starts a timed reload process
        /// </summary>
        /// <param name="reloadTime">Time in seconds for reload</param>
        /// <returns>True if reload was started</returns>
        public bool StartReload(float reloadTime) {
            // Cannot reload if already reloading, no ammo left, or magazine is full
            if (IsReloading || AmmoLeft <= 0 || CurrentAmmoInMagazine == MagazineCapacity || coroutineRunner == null) 
                return false;

            // Start the timed reload process
            reloadCoroutine = coroutineRunner.StartCoroutine(ReloadCoroutine(reloadTime));
            
            // Start reload animation
            if (weaponTransform != null) {
                animationCoroutine = coroutineRunner.StartCoroutine(ReloadAnimationCoroutine(reloadTime));
            }
            
            // Start UI ammo slider reload animation
            UIController?.StartAmmoReloadAnimation(reloadTime);
            
            IsReloading = true;
            
            // Play reload sound
            SoundEvents.TriggerWeaponReload();
            
            return true;
        }
        
        /// <summary>
        /// Cancels the current reload if in progress
        /// </summary>
        public void CancelReload() {
            if (IsReloading && coroutineRunner != null) {
                if (reloadCoroutine != null) {
                    coroutineRunner.StopCoroutine(reloadCoroutine);
                    reloadCoroutine = null;
                }
                
                if (animationCoroutine != null) {
                    coroutineRunner.StopCoroutine(animationCoroutine);
                    animationCoroutine = null;
                }
                
                // Stop UI reload animation
                UIController?.StopAmmoReloadAnimation();
                
                // Reset weapon scale if animation was interrupted
                if (weaponTransform != null) {
                    weaponTransform.localScale = Vector3.one;
                }
                
                // Update UI to show current state
                UpdateUI();
                
                IsReloading = false;
            }
        }
        
        private IEnumerator ReloadCoroutine(float reloadTime) {
            yield return new WaitForSeconds(reloadTime);
            
            // Complete the reload
            int ammoNeeded = MagazineCapacity - CurrentAmmoInMagazine;
            int ammoToReload = Mathf.Min(AmmoLeft, ammoNeeded);
            CurrentAmmoInMagazine += ammoToReload;
            AmmoLeft -= ammoToReload;
            
            IsReloading = false;
            reloadCoroutine = null;
            animationCoroutine = null; // Animation should also complete by now
            UpdateUI();
        }
        
        private IEnumerator ReloadAnimationCoroutine(float reloadTime) {
            if (weaponTransform == null) yield break;
            
            Vector3 originalScale = weaponTransform.localScale;
            float animationDuration = reloadTime;
            float elapsedTime = 0f;
            
            // Simple scale-based reload animation
            while (elapsedTime < animationDuration) {
                float progress = elapsedTime / animationDuration;
                
                // Create a "reload dip" animation - weapon gets smaller then returns to normal
                float animationCurve = Mathf.Sin(progress * Mathf.PI); // 0->1->0 curve
                float scaleMultiplier = 1f - (animationCurve * 0.1f); // 10% scale reduction at peak
                
                weaponTransform.localScale = originalScale * scaleMultiplier;
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Ensure weapon returns to original scale
            weaponTransform.localScale = originalScale;
        }

        /// <summary>
        /// Instant reload (legacy method for compatibility)
        /// </summary>
        /// <returns>True if reload was successful</returns>
        public bool Reload() {
            if(AmmoLeft <= 0 || CurrentAmmoInMagazine == MagazineCapacity) 
                return false;

            int ammoNeeded = MagazineCapacity - CurrentAmmoInMagazine;
            int ammoToReload = Mathf.Min(AmmoLeft, ammoNeeded);
            CurrentAmmoInMagazine += ammoToReload;
            AmmoLeft -= ammoToReload;
            UpdateUI();
            return true;
        }

        /// <returns>True if there is enough ammo to use</returns>
        public bool Use(int amount) {
            if (CurrentAmmoInMagazine < amount) {
                CurrentAmmoInMagazine = 0;
                return false;
            } else {
                CurrentAmmoInMagazine -= amount;
                UpdateUI();
                return true;
            }
        }

        public void AddAmmo(int amount) {
            AmmoLeft += amount;
        }
        
        /// <summary>
        /// Sets ammo to zero (clears all ammo)
        /// </summary>
        public void ClearAmmo() {
            AmmoLeft = 0;
            CurrentAmmoInMagazine = 0;
        }
        
        /// <summary>
        /// Sets total reserve ammo (not including magazine)
        /// </summary>
        public void SetReserveAmmo(int amount) {
            AmmoLeft = amount;
        }
        
        /// <summary>
        /// Sets the maximum ammo capacity for regeneration calculations
        /// </summary>
        public void SetMaxAmmoCapacity(int maxCapacity) {
            maxAmmoCapacity = maxCapacity;
        }
        
        /// <summary>
        /// Regenerates ammo based on percentage when enemy is killed (for LMG/Flamethrower)
        /// </summary>
        public void RegenerateAmmoOnKill() {
            if (AmmoRegenPercentage <= 0f || maxAmmoCapacity <= 0) return;
            
            int ammoToRestore = Mathf.RoundToInt(maxAmmoCapacity * (AmmoRegenPercentage / 100f));
            if (ammoToRestore > 0) {
                AmmoLeft += ammoToRestore;
                // Cap at max capacity to prevent infinite ammo buildup
                AmmoLeft = Mathf.Min(AmmoLeft, maxAmmoCapacity);
                UpdateUI();
            }
        }

        public bool IsMagazineEmpty() {
            return CurrentAmmoInMagazine == 0;
        }

        public void UpdateUI() {
            UIController?.UpdateAmmoCounter(CurrentAmmoInMagazine, AmmoLeft, MagazineCapacity);
        }
    }
}