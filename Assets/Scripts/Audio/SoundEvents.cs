using System;
using Assets.Scripts.Weapon;

namespace Assets.Scripts.Audio
{
    /// <summary>
    /// Event system for sound triggers - allows loose coupling between game systems and audio
    /// Other systems can trigger sounds without directly referencing SoundManager
    /// </summary>
    public static class SoundEvents
    {
        // Explosion events
        public static event Action OnTntExplosion;           // TNT/Suicider explosion
        public static event Action OnMegaBomberExplosion;    // Gigant bomber explosion
        
        // Player events
        public static event Action OnPlayerTakeDamage;       // Player hit by enemy
        public static event Action OnPlayerStartWalking;     // Start walking
        public static event Action OnPlayerStopWalking;      // Stop walking
        public static event Action OnPlayerDeath;            // Player dies
        
        // Weapon events
        public static event Action<EWeapons> OnWeaponShoot;  // Any weapon shoots
        public static event Action OnFlamethrowerStart;      // Flamethrower starts shooting
        public static event Action OnFlamethrowerStop;       // Flamethrower stops shooting
        public static event Action OnWeaponSwitch;           // When weapon is switched
        public static event Action OnWeaponReload;           // When weapon starts reloading
        
        // Item & Shop events
        public static event Action OnHealUsed;               // Medkit used successfully
        public static event Action OnWeaponPurchased;        // Weapon bought/upgraded
        public static event Action OnTntPlaced;              // TNT placed successfully
        
        // Game system events
        public static event Action OnGameInitialized;        // Game starts for first time
        public static event Action OnRoundStart;             // Round starts after countdown
        public static event Action OnZombieDie;              // Zombie dies
        public static event Action OnRPGExplosion;           // RPG explosion
        public static event Action OnGameOverStart;          // Game over screen shown
        public static event Action OnGameOverEnd;            // Game over screen closed (restart/quit)
        public static event Action OnSpacePressed;           // Space pressed to start round (Sound A)
        public static event Action OnCountdownTick;          // Each countdown tick (Sound B)
        
        #region Event Triggers (called by game systems)
        
        public static void TriggerTntExplosion() => OnTntExplosion?.Invoke();
        public static void TriggerMegaBomberExplosion() => OnMegaBomberExplosion?.Invoke();
        
        public static void TriggerPlayerTakeDamage() => OnPlayerTakeDamage?.Invoke();
        public static void TriggerPlayerStartWalking() => OnPlayerStartWalking?.Invoke();
        public static void TriggerPlayerStopWalking() => OnPlayerStopWalking?.Invoke();
        public static void TriggerPlayerDeath() => OnPlayerDeath?.Invoke();
        
        public static void TriggerWeaponShoot(EWeapons weaponType) => OnWeaponShoot?.Invoke(weaponType);
        public static void TriggerFlamethrowerStart() => OnFlamethrowerStart?.Invoke();
        public static void TriggerFlamethrowerStop() => OnFlamethrowerStop?.Invoke();
        public static void TriggerWeaponSwitch() => OnWeaponSwitch?.Invoke();
        public static void TriggerWeaponReload() => OnWeaponReload?.Invoke();
        
        public static void TriggerHealUsed() => OnHealUsed?.Invoke();
        public static void TriggerWeaponPurchased() => OnWeaponPurchased?.Invoke();
        public static void TriggerTntPlaced() => OnTntPlaced?.Invoke();
        
        public static void TriggerGameInitialized() => OnGameInitialized?.Invoke();
        public static void TriggerRoundStart() => OnRoundStart?.Invoke();
        public static void TriggerZombieDie() => OnZombieDie?.Invoke();
        public static void TriggerRPGExplosion() => OnRPGExplosion?.Invoke();
        public static void TriggerGameOverStart() => OnGameOverStart?.Invoke();
        public static void TriggerGameOverEnd() => OnGameOverEnd?.Invoke();
        public static void TriggerSpacePressed() => OnSpacePressed?.Invoke();
        public static void TriggerCountdownTick() => OnCountdownTick?.Invoke();
        
        #endregion
        
        #region SoundManager Subscription (internal use)
        
        /// <summary>
        /// Called by SoundManager to subscribe to all events
        /// </summary>
        internal static void SubscribeSoundManager(SoundManager soundManager)
        {
            // Explosion sounds
            OnTntExplosion += soundManager.PlayExplosion1;
            OnMegaBomberExplosion += soundManager.PlayExplosion2;
            
            // Player sounds
            OnPlayerTakeDamage += soundManager.PlayBloodyPunch;
            OnPlayerStartWalking += soundManager.StartWalkingSound;
            OnPlayerStopWalking += soundManager.StopWalkingSound;
            OnPlayerDeath += soundManager.PlayPlayerDeath;
            
            // Weapon sounds
            OnWeaponShoot += soundManager.PlayWeaponSound;
            OnFlamethrowerStart += soundManager.OnFlamethrowerStartShooting;
            OnFlamethrowerStop += soundManager.OnFlamethrowerStopShooting;
            OnWeaponSwitch += soundManager.StopAllWeaponSounds;
            OnWeaponReload += soundManager.PlayReloadSound;
            
            // Item & Shop sounds
            OnHealUsed += soundManager.PlayHealSound;
            OnWeaponPurchased += soundManager.PlayBuySellSound;
            OnTntPlaced += soundManager.PlayPlaceTntSound;
            
            // Game system sounds
            OnGameInitialized += soundManager.PlayGameInitializationSound;
            OnRoundStart += soundManager.PlayRoundStartSound;
            OnZombieDie += soundManager.PlayZombieDieSound;
            OnRPGExplosion += soundManager.PlayRPGExplosion;
            OnGameOverStart += soundManager.PlayGameOverAmbient;
            OnGameOverEnd += soundManager.StopGameOverAmbient;
            OnSpacePressed += soundManager.PlaySpacePressSound;
            OnCountdownTick += soundManager.PlayCountdownTickSound;
        }
        
        /// <summary>
        /// Called when SoundManager is destroyed to prevent memory leaks
        /// </summary>
        internal static void UnsubscribeSoundManager(SoundManager soundManager)
        {
            // Explosion sounds
            OnTntExplosion -= soundManager.PlayExplosion1;
            OnMegaBomberExplosion -= soundManager.PlayExplosion2;
            
            // Player sounds
            OnPlayerTakeDamage -= soundManager.PlayBloodyPunch;
            OnPlayerStartWalking -= soundManager.StartWalkingSound;
            OnPlayerStopWalking -= soundManager.StopWalkingSound;
            OnPlayerDeath -= soundManager.PlayPlayerDeath;
            
            // Weapon sounds
            OnWeaponShoot -= soundManager.PlayWeaponSound;
            OnFlamethrowerStart -= soundManager.OnFlamethrowerStartShooting;
            OnFlamethrowerStop -= soundManager.OnFlamethrowerStopShooting;
            OnWeaponSwitch -= soundManager.StopAllWeaponSounds;
            OnWeaponReload -= soundManager.PlayReloadSound;
            
            // Item & Shop sounds
            OnHealUsed -= soundManager.PlayHealSound;
            OnWeaponPurchased -= soundManager.PlayBuySellSound;
            OnTntPlaced -= soundManager.PlayPlaceTntSound;
            
            // Game system sounds
            OnGameInitialized -= soundManager.PlayGameInitializationSound;
            OnRoundStart -= soundManager.PlayRoundStartSound;
            OnZombieDie -= soundManager.PlayZombieDieSound;
            OnRPGExplosion -= soundManager.PlayRPGExplosion;
            OnGameOverStart -= soundManager.PlayGameOverAmbient;
            OnGameOverEnd -= soundManager.StopGameOverAmbient;
            OnSpacePressed -= soundManager.PlaySpacePressSound;
            OnCountdownTick -= soundManager.PlayCountdownTickSound;
        }
        
        #endregion
    }
}