using UnityEngine;
using Assets.Scripts.Weapon;

namespace Assets.Scripts.Audio
{
    /// <summary>
    /// Central sound management system with drag & drop AudioClip assignment
    /// Singleton pattern for global access across the game
    /// </summary>
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }
        
        [Header("=== EXPLOSION SOUNDS ===")]
        [SerializeField] private AudioClip explosion1;          // TNT / Suicider enemy
        [SerializeField] private AudioClip explosion2;          // Mega bomber enemy
        
        [Header("=== PLAYER SOUNDS ===")]
        [SerializeField] private AudioClip bloodyPunch;         // Player takes damage
        [SerializeField] private AudioClip walkingChainMail;    // Walking loop
        [SerializeField] private AudioClip indianaJonesPunch;   // Player death
        
        [Header("=== WEAPON SOUNDS ===")]
        [SerializeField] private AudioClip m4RifleSound;        // M4 Rifle
        [SerializeField] private AudioClip pistolSound;         // Pistol
        [SerializeField] private AudioClip uziSound;            // UZI
        [SerializeField] private AudioClip lmgSound;            // LMG/M249
        [SerializeField] private AudioClip shotgunSound;        // Shotgun
        [SerializeField] private AudioClip sniperSound;         // AWP Sniper
        [SerializeField] private AudioClip flamethrowerSound;   // Flamethrower (start only)
        [SerializeField] private AudioClip rpgSound;            // RPG-7
        [SerializeField] private AudioClip rpgExplosionSound;   // RPG explosion
        
        [Header("=== ITEM & SHOP SOUNDS ===")]
        [SerializeField] private AudioClip heal;                // Medkit use
        [SerializeField] private AudioClip buySell;             // Buy/Upgrade in shop
        [SerializeField] private AudioClip placeTnt;            // TNT placement
        
        [Header("=== GAME SYSTEM SOUNDS ===")]
        [SerializeField] private AudioClip ghostManifestation;  // Game initialization
        [SerializeField] private AudioClip roundStartSound;     // Round start after countdown
        [SerializeField] private AudioClip zombieDieSound;      // Zombie death
        [SerializeField] private AudioClip gameOverAmbient;     // Game over screen ambient (5min loop)
        [SerializeField] private AudioClip spacePressSound;     // Sound A: When space is pressed to start round
        [SerializeField] private AudioClip countdownTickSound;  // Sound B: Each countdown tick (3-2-1)
        
        [Header("=== AUDIO SOURCES ===")]
        [SerializeField] private AudioSource effectsSource;     // One-shot effects
        [SerializeField] private AudioSource walkingSource;     // Looping walking sound
        [SerializeField] private AudioSource weaponSource;      // Weapon shots
        [SerializeField] private AudioSource ambientSource;     // Long ambient sounds like Ghost Manifestation
        [SerializeField] private AudioSource explosionSource;   // Explosion sounds
        [SerializeField] private AudioSource gameSystemSource;  // Round start, zombie die, etc.
        [SerializeField] private AudioSource gameOverSource;    // Game over screen ambient music
        
        [Header("=== VOLUME SETTINGS ===")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float effectsVolume = 0.8f;
        [Range(0f, 1f)] public float weaponVolume = 0.7f;
        [Range(0f, 1f)] public float walkingVolume = 0.5f;
        [Range(0f, 1f)] public float explosionVolume = 0.9f;
        [Range(0f, 1f)] public float ambientVolume = 0.6f;
        [Range(0f, 1f)] public float gameOverVolume = 0.4f;
        
        private bool isWalkingSoundPlaying = false;
        private bool flamethrowerShooting = false;
        
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
                
                // Subscribe to sound events
                SoundEvents.SubscribeSoundManager(this);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe to prevent memory leaks
            if (Instance == this)
            {
                SoundEvents.UnsubscribeSoundManager(this);
            }
        }
        
        private void InitializeAudioSources()
        {
            // Create AudioSources if not assigned in inspector
            if (effectsSource == null)
            {
                GameObject effectsGO = new GameObject("EffectsAudioSource");
                effectsGO.transform.SetParent(transform);
                effectsSource = effectsGO.AddComponent<AudioSource>();
            }
            
            if (walkingSource == null)
            {
                GameObject walkingGO = new GameObject("WalkingAudioSource");
                walkingGO.transform.SetParent(transform);
                walkingSource = walkingGO.AddComponent<AudioSource>();
                walkingSource.loop = true;
            }
            
            if (weaponSource == null)
            {
                GameObject weaponGO = new GameObject("WeaponAudioSource");
                weaponGO.transform.SetParent(transform);
                weaponSource = weaponGO.AddComponent<AudioSource>();
            }
            
            if (ambientSource == null)
            {
                GameObject ambientGO = new GameObject("AmbientAudioSource");
                ambientGO.transform.SetParent(transform);
                ambientSource = ambientGO.AddComponent<AudioSource>();
            }
            
            if (explosionSource == null)
            {
                GameObject explosionGO = new GameObject("ExplosionAudioSource");
                explosionGO.transform.SetParent(transform);
                explosionSource = explosionGO.AddComponent<AudioSource>();
            }
            
            if (gameSystemSource == null)
            {
                GameObject gameSystemGO = new GameObject("GameSystemAudioSource");
                gameSystemGO.transform.SetParent(transform);
                gameSystemSource = gameSystemGO.AddComponent<AudioSource>();
            }
            
            if (gameOverSource == null)
            {
                GameObject gameOverGO = new GameObject("GameOverAudioSource");
                gameOverGO.transform.SetParent(transform);
                gameOverSource = gameOverGO.AddComponent<AudioSource>();
                gameOverSource.loop = true; // Game over music should loop
            }
            
            UpdateVolumeSettings();
        }
        
        private void UpdateVolumeSettings()
        {
            if (effectsSource != null) effectsSource.volume = effectsVolume * masterVolume;
            if (walkingSource != null) walkingSource.volume = walkingVolume * masterVolume;
            if (weaponSource != null) weaponSource.volume = weaponVolume * masterVolume;
            if (ambientSource != null) ambientSource.volume = ambientVolume * masterVolume;
            if (explosionSource != null) explosionSource.volume = explosionVolume * masterVolume;
            if (gameSystemSource != null) gameSystemSource.volume = effectsVolume * masterVolume;
            if (gameOverSource != null) gameOverSource.volume = gameOverVolume * masterVolume;
        }
        
        #region Explosion Sounds
        
        public void PlayExplosion1()
        {
            PlaySound(explosion1, explosionSource);
        }
        
        public void PlayExplosion2()
        {
            PlaySound(explosion2, explosionSource);
        }
        
        public void PlayRPGExplosion()
        {
            PlaySound(rpgExplosionSound, explosionSource);
        }
        
        #endregion
        
        #region Player Sounds
        
        public void PlayBloodyPunch()
        {
            PlaySound(bloodyPunch, effectsSource);
        }
        
        public void PlayPlayerDeath()
        {
            // Stop all player sounds before playing death sound
            StopWalkingSound();
            StopAllWeaponSounds();
            
            PlaySound(indianaJonesPunch, effectsSource);
        }
        
        public void StartWalkingSound()
        {
            if (!isWalkingSoundPlaying && walkingChainMail != null)
            {
                walkingSource.clip = walkingChainMail;
                walkingSource.Play();
                isWalkingSoundPlaying = true;
            }
        }
        
        public void StopWalkingSound()
        {
            if (isWalkingSoundPlaying)
            {
                walkingSource.Stop();
                isWalkingSoundPlaying = false;
            }
        }
        
        #endregion
        
        #region Weapon Sounds
        
        public void PlayWeaponSound(EWeapons weaponType)
        {
            AudioClip clipToPlay = weaponType switch
            {
                EWeapons.M4 => m4RifleSound,
                EWeapons.PISTOL => pistolSound,
                EWeapons.UZI => uziSound,
                EWeapons.M249 => lmgSound,
                EWeapons.SHOTGUN => shotgunSound,
                EWeapons.AWP => sniperSound,
                EWeapons.FLAMETHROWER => HandleFlamethrowerSound(),
                EWeapons.RPG7 => rpgSound,
                _ => null
            };
            
            if (clipToPlay != null)
            {
                PlaySound(clipToPlay, weaponSource);
            }
        }
        
        private AudioClip HandleFlamethrowerSound()
        {
            // The flamethrower sound is handled via events, not through this method
            // Return null to prevent double-playing of sound
            return null;
        }
        
        public void OnFlamethrowerStartShooting()
        {
            // Play flamethrower start sound only if not already playing
            if (!flamethrowerShooting && flamethrowerSound != null && weaponSource != null)
            {
                weaponSource.clip = flamethrowerSound;
                weaponSource.loop = true;
                weaponSource.Play();
                flamethrowerShooting = true;
            }
        }
        
        public void OnFlamethrowerStopShooting()
        {
            // Stop the continuous flamethrower sound
            if (flamethrowerShooting && weaponSource != null)
            {
                weaponSource.Stop();
                weaponSource.loop = false;
                weaponSource.clip = null;
            }
            flamethrowerShooting = false;
        }
        
        /// <summary>
        /// Stops all weapon sounds (useful for weapon switching)
        /// </summary>
        public void StopAllWeaponSounds()
        {
            OnFlamethrowerStopShooting();
            // Add other continuous weapon sounds here if needed
        }
        
        #endregion
        
        #region Item & Shop Sounds
        
        public void PlayHealSound()
        {
            PlaySound(heal, effectsSource);
        }
        
        public void PlayBuySellSound()
        {
            PlaySound(buySell, effectsSource);
        }
        
        public void PlayPlaceTntSound()
        {
            PlaySound(placeTnt, effectsSource);
        }
        
        #endregion
        
        #region Game System Sounds
        
        public void PlayGameInitializationSound()
        {
            if (ghostManifestation != null && ambientSource != null)
            {
                ambientSource.clip = ghostManifestation;
                ambientSource.Play();
            }
        }
        
        public void StopGameInitializationSound()
        {
            if (ambientSource != null)
            {
                ambientSource.Stop();
            }
        }
        
        public void PlayRoundStartSound()
        {
            PlaySound(roundStartSound, gameSystemSource);
        }
        
        public void PlayZombieDieSound()
        {
            PlaySound(zombieDieSound, gameSystemSource);
        }
        
        public void PlayGameOverAmbient()
        {
            if (gameOverAmbient != null && gameOverSource != null)
            {
                gameOverSource.clip = gameOverAmbient;
                gameOverSource.Play();
                Debug.Log("Started game over ambient music");
            }
        }
        
        public void StopGameOverAmbient()
        {
            if (gameOverSource != null)
            {
                gameOverSource.Stop();
                gameOverSource.clip = null;
                Debug.Log("Stopped game over ambient music");
            }
        }
        
        public void PlaySpacePressSound()
        {
            PlaySound(spacePressSound, gameSystemSource);
        }
        
        public void PlayCountdownTickSound()
        {
            PlaySound(countdownTickSound, gameSystemSource);
        }
        
        #endregion
        
        #region Utility Methods
        
        private void PlaySound(AudioClip clip, AudioSource audioSource)
        {
            if (clip != null && audioSource != null)
            {
                audioSource.PlayOneShot(clip);
            }
            else if (clip == null)
            {
                Debug.LogWarning("SoundManager: AudioClip is null - make sure to assign it in the Inspector!");
            }
        }
        
        /// <summary>
        /// Call this when volume settings change in options menu
        /// </summary>
        public void RefreshVolumeSettings()
        {
            UpdateVolumeSettings();
        }
        
        /// <summary>
        /// Stop all sounds (useful for pause/game over)
        /// </summary>
        public void StopAllSounds()
        {
            effectsSource?.Stop();
            walkingSource?.Stop();
            weaponSource?.Stop();
            ambientSource?.Stop();
            explosionSource?.Stop();
            gameSystemSource?.Stop();
            gameOverSource?.Stop();
            isWalkingSoundPlaying = false;
            flamethrowerShooting = false;
        }
        
        #endregion
        
        #region Debug
        
        [ContextMenu("Test All Sounds")]
        private void TestAllSounds()
        {
            Debug.Log("Testing all sounds...");
            
            // Test explosion sounds
            if (explosion1 != null) PlayExplosion1();
            if (explosion2 != null) PlayExplosion2();
            
            // Test player sounds
            if (bloodyPunch != null) PlayBloodyPunch();
            if (indianaJonesPunch != null) PlayPlayerDeath();
            
            // Test weapon sounds
            foreach (EWeapons weapon in System.Enum.GetValues(typeof(EWeapons)))
            {
                PlayWeaponSound(weapon);
            }
            
            // Test item sounds
            if (heal != null) PlayHealSound();
            if (buySell != null) PlayBuySellSound();
            if (placeTnt != null) PlayPlaceTntSound();
            
            // Test game system sounds
            if (ghostManifestation != null) PlayGameInitializationSound();
        }
        
        #endregion
    }
}