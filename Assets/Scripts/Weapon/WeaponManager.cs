using Assets.Scripts.Shop;
using Assets.Scripts.Weapon;
using Assets.Scripts.PlayerScripts;
using Assets.Scripts.Audio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EWeapons {
    PISTOL, UZI, SHOTGUN, M4, AWP, M249, RPG7, FLAMETHROWER
}

/// <summary>
/// Holds Player Weapons
/// </summary>
public class WeaponManager : MonoBehaviour {
    [SerializeField] List<GameObject> weaponsPrefabs;
    [SerializeField] Player player;

    private Weapon weapon;
    private List<GameObject> weapons = new List<GameObject>();
    private int currentWeaponIndex = (int) EWeapons.PISTOL;
    
    // Weapon upgrade integration
    private Dictionary<EWeapons, WeaponStatsAdapter> weaponAdapters = new Dictionary<EWeapons, WeaponStatsAdapter>();

    private void Awake() {
        InstantiateWeapons();
    }
    void Start()
    {
        Ammo.UIController = FindObjectOfType<Assets.Scripts.PlayerScripts.UIController>();
        SelectWeapon(currentWeaponIndex);
        
        // Delay ammo initialization to ensure all weapon Start() methods have run
        StartCoroutine(InitializeAmmoDelayed());
    }
    
    private IEnumerator InitializeAmmoDelayed() {
        yield return new WaitForSeconds(0.1f); // Wait for all Start() methods to complete
        yield return StartCoroutine(InitializeAmmoCoroutine());
    }
    
    private IEnumerator InitializeAmmoCoroutine() {
        yield return StartCoroutine(InstantiateAmmos());
        yield return null;
        
        // Subscribe to wave start events for ammo restoration
        if (WaveManager.Instance != null) {
            WaveManager.Instance.OnWaveStarted += OnWaveStarted;
        }
        
        // Subscribe to weapon purchase events to enable new weapons
        if (Assets.Scripts.Shop.WeaponLevelTracker.Instance != null) {
            Assets.Scripts.Shop.WeaponLevelTracker.Instance.OnWeaponPurchased += OnWeaponPurchased;
        }
    }

    private void InstantiateWeapons() {
        foreach (var pf in weaponsPrefabs) {
            GameObject weaponGO = Instantiate(pf, transform);
            weaponGO.SetActive(false);
            weapons.Add(weaponGO);
        }
    }

    private IEnumerator InstantiateAmmos() {
        int originallySelectedWeapon = currentWeaponIndex;
        
        // Initialize weapons based on ownership from WeaponLevelTracker
        foreach (var weaponGO in weapons) {
            Weapon weapon = weaponGO.GetComponent<Weapon>();
            
            // Check if player owns this weapon
            bool isOwned = Assets.Scripts.Shop.WeaponLevelTracker.Instance?.OwnsWeapon(weapon.id) ?? (weapon.id == EWeapons.PISTOL);
            
            if (isOwned) {
                weaponGO.SetActive(true);
                
                // Wait for weapon's Start() to complete initialization
                yield return null; // Wait one frame
                yield return null; // Wait one more frame to be absolutely sure
                
                // Verify initialization completed
                if (weapon.Stats == null || weapon.Ammo.MagazineCapacity == 0) {
                    Debug.LogWarning($"Weapon {weapon.id} not fully initialized, trying fallback");
                    InitializeWeaponStatsFallback(weapon);
                }
                
                // IMPORTANT: Create weapon adapter and apply upgrades AFTER weapon has initialized base stats
                CreateAndRegisterWeaponAdapter(weapon);
                
                // Give starting reserve ammo only to owned weapons
                int reserveAmmo = GetStartingAmmoForWeapon(weapon.id);
                weapon.Ammo.SetReserveAmmo(reserveAmmo);
                weapon.Ammo.Reload(); // Load magazine from reserves
                
                
                // Only deactivate if this isn't the currently selected weapon
                if (weapons.IndexOf(weaponGO) != originallySelectedWeapon) {
                    weaponGO.SetActive(false);
                }
            } else {
                // Weapon is not owned, keep it deactivated
                weaponGO.SetActive(false);
                Debug.Log($"Weapon {weapon.id} not owned - keeping inactive");
            }
        }
    }
    
    private void InitializeWeaponStatsFallback(Weapon weapon) {
        // Manually initialize weapon stats based on type
        switch (weapon.id) {
            case EWeapons.PISTOL:
                weapon.Stats = WeaponStatsRepository.Pistol();
                break;
            case EWeapons.UZI:
                weapon.Stats = WeaponStatsRepository.SMG();
                break;
            case EWeapons.SHOTGUN:
                weapon.Stats = WeaponStatsRepository.Shotgun();
                break;
            case EWeapons.M4:
                weapon.Stats = WeaponStatsRepository.Rifle();
                break;
            case EWeapons.AWP:
                weapon.Stats = WeaponStatsRepository.SniperRifle();
                break;
            case EWeapons.M249:
                weapon.Stats = WeaponStatsRepository.M249();
                break;
            case EWeapons.RPG7:
                weapon.Stats = WeaponStatsRepository.RPG();
                break;
            case EWeapons.FLAMETHROWER:
                weapon.Stats = WeaponStatsRepository.Flamethrower();
                break;
        }
        
        // Set magazine capacity
        if (weapon.Stats != null) {
            weapon.Ammo.MagazineCapacity = weapon.Stats.MagazineCapacity;
        }
    }
    
    private int GetStartingAmmoForWeapon(EWeapons weaponType) {
        // First try to get actual weapon instance to use current (upgraded) stats
        var weapon = GetWeapon(weaponType);
        if (weapon != null && weapon.Stats != null) {
            // Use the weapon's current stats (which include upgrades)
            int upgradeAmmo = weapon.Stats.ExtraMagazines * weapon.Stats.MagazineCapacity;
            return upgradeAmmo;
        }
        
        // Fallback to base stats if weapon not found
        WeaponStats stats = GetWeaponStats(weaponType);
        if (stats == null) return 0;
        
        // Calculate total ammo: (ExtraMagazines * MagazineCapacity)
        // Player starts with current magazine loaded + extra magazines in reserve
        int baseAmmo = stats.ExtraMagazines * stats.MagazineCapacity;
        
        return baseAmmo;
    }
    
    private WeaponStats GetWeaponStats(EWeapons weaponType) {
        switch (weaponType) {
            case EWeapons.PISTOL:
                return WeaponStatsRepository.Pistol();
            case EWeapons.UZI:
                return WeaponStatsRepository.SMG();
            case EWeapons.SHOTGUN:
                return WeaponStatsRepository.Shotgun();
            case EWeapons.M4:
                return WeaponStatsRepository.Rifle();
            case EWeapons.AWP:
                return WeaponStatsRepository.SniperRifle();
            case EWeapons.M249:
                return WeaponStatsRepository.M249();
            case EWeapons.RPG7:
                return WeaponStatsRepository.RPG();
            case EWeapons.FLAMETHROWER:
                return WeaponStatsRepository.Flamethrower();
            default:
                return null;
        }
    }
    
    /// <summary>
    /// Gets a weapon instance by type
    /// </summary>
    public Weapon GetWeapon(EWeapons weaponType)
    {
        int weaponIndex = (int)weaponType;
        if (weaponIndex >= 0 && weaponIndex < weapons.Count && weapons[weaponIndex] != null)
        {
            return weapons[weaponIndex].GetComponent<Weapon>();
        }
        return null;
    }

    public void SelectWeapon(int weaponIndex) {
        // Validate weapon index
        if (weaponIndex < 0 || weaponIndex >= weapons.Count || weapons[weaponIndex] == null) {
            return; // Invalid weapon index or weapon not available
        }

        // Stop any continuous weapon sounds (like flamethrower) before switching
        if (weaponIndex != currentWeaponIndex) {
            SoundEvents.TriggerWeaponSwitch();
            
            // Cancel reload on current weapon before switching
            if (weapons[currentWeaponIndex] != null) {
                Weapon currentWeapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
                currentWeapon?.OnWeaponDeactivated();
            }
        }

        weapons[currentWeaponIndex]?.SetActive(false);
        currentWeaponIndex = weaponIndex;
        weapons[weaponIndex].SetActive(true);
        weapon = weapons[weaponIndex].GetComponent<Weapon>();
        player?.EquipWeapon(weapon);
        weapon.Ammo.UpdateUI();
    }

    /// <summary>
    /// Selects a weapon by its type (EWeapons enum), ensuring consistent key bindings
    /// </summary>
    public void SelectWeaponByType(EWeapons weaponType) {
        // Find the weapon in the list by its type
        for (int i = 0; i < weapons.Count; i++) {
            if (weapons[i] != null) {
                Weapon weaponComponent = weapons[i].GetComponent<Weapon>();
                if (weaponComponent != null && weaponComponent.id == weaponType) {
                    // Check if player owns this weapon
                    bool isOwned = Assets.Scripts.Shop.WeaponLevelTracker.Instance?.OwnsWeapon(weaponType) ?? (weaponType == EWeapons.PISTOL);
                    if (isOwned) {
                        SelectWeapon(i);
                        return;
                    }
                    break; // Found the weapon but not owned
                }
            }
        }
        // Weapon not found or not owned - do nothing
    }

    /// <summary>
    /// Swap's Player's current weapon (based on provided weapon id)
    /// </summary>
    /// <param name="weaponIndex">Concrete weapon index</param>
    internal void SwapWeapon(int weaponIndex) {
        if (weaponIndex >= 0 && weaponIndex < weapons.Count) {
            Weapon targetWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            bool isOwned = Assets.Scripts.Shop.WeaponLevelTracker.Instance?.OwnsWeapon(targetWeapon.id) ?? (targetWeapon.id == EWeapons.PISTOL);
            
            if (isOwned) {
                SelectWeapon(weaponIndex);
            } else {
                Debug.Log($"Cannot select weapon {targetWeapon.id} - not owned");
            }
        }
    }

    /// <summary>
    /// Swap's Player's current weapon (based on provided direction)
    /// </summary>
    /// <param name="selectNext">If True selects weapon with higher ID</param>
    internal void SwapWeapon(bool selectNext) {
        int direction = selectNext ? 1 : -1;
        int attempts = 0;
        int idx = currentWeaponIndex;
        
        // Find next owned weapon
        do {
            idx = (idx + direction + weapons.Count) % weapons.Count;
            attempts++;
            
            if (attempts > weapons.Count) break; // Prevent infinite loop
            
            Weapon targetWeapon = weapons[idx].GetComponent<Weapon>();
            bool isOwned = Assets.Scripts.Shop.WeaponLevelTracker.Instance?.OwnsWeapon(targetWeapon.id) ?? (targetWeapon.id == EWeapons.PISTOL);
            
            if (isOwned) {
                SelectWeapon(idx);
                return;
            }
        } while (idx != currentWeaponIndex);
    }
    
    /// <summary>
    /// Called when a wave starts - restores ammo for all owned weapons
    /// </summary>
    private void OnWaveStarted(int waveNumber) {
        Debug.Log($"WeaponManager: Restoring ammo for wave {waveNumber}");
        
        foreach (var weaponGO in weapons) {
            Weapon weapon = weaponGO.GetComponent<Weapon>();
            bool isOwned = Assets.Scripts.Shop.WeaponLevelTracker.Instance?.OwnsWeapon(weapon.id) ?? (weapon.id == EWeapons.PISTOL);
            
            if (isOwned) {
                // RESET ammo completely: clear all ammo and set to max capacity
                int reserveAmmo = GetStartingAmmoForWeapon(weapon.id);
                
                // Clear all ammo first
                weapon.Ammo.ClearAmmo();
                
                // Set reserve ammo (magazines worth)
                weapon.Ammo.SetReserveAmmo(reserveAmmo);
                
                // Force reload to fill magazine from reserves
                weapon.Ammo.Reload();
                
            }
        }
    }
    
    /// <summary>
    /// Called when a weapon is purchased - enables the weapon
    /// </summary>
    private void OnWeaponPurchased(EWeapons weaponType, int level) {
        Debug.Log($"WeaponManager: Enabling purchased weapon {weaponType}");
        
        int weaponIndex = (int)weaponType;
        if (weaponIndex >= 0 && weaponIndex < weapons.Count) {
            GameObject weaponGO = weapons[weaponIndex];
            Weapon weapon = weaponGO.GetComponent<Weapon>();
            
            // Enable the weapon and give it starting ammo
            weaponGO.SetActive(true);
            
            // Ensure weapon is properly initialized
            if (weapon.Stats == null || weapon.Ammo.MagazineCapacity == 0) {
                InitializeWeaponStatsFallback(weapon);
            }
            
            // Create weapon adapter for newly purchased weapon (starts at level 1, no upgrades yet)
            CreateAndRegisterWeaponAdapter(weapon);
            
            int reserveAmmo = GetStartingAmmoForWeapon(weapon.id);
            weapon.Ammo.SetReserveAmmo(reserveAmmo);
            weapon.Ammo.Reload(); // Load magazine from reserves
            
            // Deactivate if not currently selected
            if (weaponIndex != currentWeaponIndex) {
                weaponGO.SetActive(false);
            }
            
            Debug.Log($"Enabled purchased weapon {weaponType} with {reserveAmmo} reserve ammo");
        }
    }
    
    /// <summary>
    /// Creates weapon adapter and applies current upgrade level to weapon stats
    /// </summary>
    private void CreateAndRegisterWeaponAdapter(Weapon weapon)
    {
        if (weapon == null || weapon.Stats == null) return;
        
        // Create adapter from current weapon stats
        var adapter = WeaponStatsAdapter.FromWeaponStats(weapon.Stats, debugMode: false);
        
        // Get current weapon level and apply all upgrades
        int currentLevel = Assets.Scripts.Shop.WeaponLevelTracker.Instance?.GetWeaponLevel(weapon.id) ?? 1;
        
        if (currentLevel > 1)
        {
            // Get upgrade service from ShopManager if available
            var shopManager = Assets.Scripts.Shop.ShopManager.Instance;
            if (shopManager != null)
            {
                // Apply all upgrades from level 2 to current level
                shopManager.ApplyAllUpgradesToAdapter(weapon.id, currentLevel, adapter);
            }
        }
        
        // Sync upgraded stats back to the original weapon
        adapter.SyncToOriginalStats();
        
        // Register adapter for future upgrades
        weaponAdapters[weapon.id] = adapter;
        
    }
    
    /// <summary>
    /// Gets weapon adapter for upgrade system integration
    /// </summary>
    public WeaponStatsAdapter GetWeaponAdapter(EWeapons weaponType)
    {
        weaponAdapters.TryGetValue(weaponType, out var adapter);
        return adapter;
    }
    
    /// <summary>
    /// Gets all registered weapon adapters
    /// </summary>
    public Dictionary<EWeapons, WeaponStatsAdapter> GetAllWeaponAdapters()
    {
        return new Dictionary<EWeapons, WeaponStatsAdapter>(weaponAdapters);
    }
    
    /// <summary>
    /// Recreates a weapon instance with upgraded stats - called after weapon upgrades
    /// </summary>
    public void RecreateWeaponWithUpgrades(EWeapons weaponType)
    {
        int weaponIndex = (int)weaponType;
        if (weaponIndex < 0 || weaponIndex >= weapons.Count || weaponIndex >= weaponsPrefabs.Count)
        {
            Debug.LogError($"Invalid weapon index for {weaponType}");
            return;
        }
        
        bool wasCurrentWeapon = (weaponIndex == currentWeaponIndex);
        
        // Destroy old weapon instance
        if (weapons[weaponIndex] != null)
        {
            Destroy(weapons[weaponIndex]);
        }
        
        // Create new weapon instance
        GameObject newWeaponGO = Instantiate(weaponsPrefabs[weaponIndex], transform);
        newWeaponGO.SetActive(false); // Start inactive
        weapons[weaponIndex] = newWeaponGO;
        
        Weapon newWeapon = newWeaponGO.GetComponent<Weapon>();
        
        // Start the recreation process
        StartCoroutine(InitializeRecreatedWeapon(newWeapon, wasCurrentWeapon));
    }
    
    private IEnumerator InitializeRecreatedWeapon(Weapon weapon, bool shouldBeActive)
    {
        
        // Enable weapon temporarily to let it initialize
        weapon.gameObject.SetActive(true);
        
        // Wait for weapon's Start() to complete base initialization
        yield return null;
        yield return null; // Extra frame for safety
        
        // Verify weapon initialized properly
        if (weapon.Stats == null || weapon.Ammo.MagazineCapacity == 0)
        {
            Debug.LogWarning($"Recreated weapon {weapon.id} not fully initialized, trying fallback");
            InitializeWeaponStatsFallback(weapon);
        }
        
        // Now apply upgrades to the fresh weapon
        CreateAndRegisterWeaponAdapter(weapon);
        
        // CRITICAL: Update magazine capacity with upgraded stats AFTER applying upgrades
        weapon.Ammo.MagazineCapacity = weapon.Stats.MagazineCapacity;
        
        // Set up ammo with upgraded stats
        int reserveAmmo = GetStartingAmmoForWeapon(weapon.id);
        weapon.Ammo.SetReserveAmmo(reserveAmmo);
        weapon.Ammo.Reload();
        
        // Activate if this should be the current weapon
        if (shouldBeActive)
        {
            SelectWeapon((int)weapon.id);
        }
        else
        {
            weapon.gameObject.SetActive(false);
        }
    }
    
    private void OnDestroy() {
        // Unsubscribe from events
        if (WaveManager.Instance != null) {
            WaveManager.Instance.OnWaveStarted -= OnWaveStarted;
        }
        
        if (Assets.Scripts.Shop.WeaponLevelTracker.Instance != null) {
            Assets.Scripts.Shop.WeaponLevelTracker.Instance.OnWeaponPurchased -= OnWeaponPurchased;
        }
    }
}