using Assets.Scripts.PlayerScripts;
using Assets.Scripts.Weapon;
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

    private void Awake() {
        InstantiateWeapons();
    }
    void Start()
    {
        Ammo.UIController = FindObjectOfType<UIController>();
        SelectWeapon(currentWeaponIndex);
        
        // Delay ammo initialization to ensure all weapon Start() methods have run
        StartCoroutine(InitializeAmmoDelayed());
    }
    
    private IEnumerator InitializeAmmoDelayed() {
        yield return new WaitForSeconds(0.1f); // Wait a bit longer to ensure all Start() methods complete
        InstantiateAmmos();
    }

    private void InstantiateWeapons() {
        foreach (var pf in weaponsPrefabs) {
            GameObject weaponGO = Instantiate(pf, transform);
            weaponGO.SetActive(false);
            weapons.Add(weaponGO);
        }
    }

    private void InstantiateAmmos() {
        int originallySelectedWeapon = currentWeaponIndex; // Remember which weapon was selected
        
        foreach (var weaponGO in weapons) {
            weaponGO.SetActive(true);
            Weapon weapon = weaponGO.GetComponent<Weapon>();
            
            // Give the weapon a moment to initialize if needed
            if (weapon.Stats == null || weapon.Ammo.MagazineCapacity == 0) {
                Debug.LogWarning($"Weapon {weapon.id} not fully initialized, trying fallback");
                InitializeWeaponStatsFallback(weapon);
            }
            
            // Give different amounts of starting ammo based on weapon type
            int startingAmmo = GetStartingAmmoForWeapon(weapon.id);
            
            // Start with a loaded magazine + extra ammo
            weapon.Ammo.AddAmmo(startingAmmo);
            weapon.Ammo.Reload(); // Fill the magazine from the ammo pool
            
            Debug.Log($"Initialized {weapon.id}: Magazine={weapon.Ammo.MagazineCapacity}, AmmoLeft={weapon.Ammo.AmmoLeft}, InMag={weapon.Ammo.CurrentAmmoInMagazine}");
            
            // Only deactivate if this isn't the currently selected weapon
            if (weapons.IndexOf(weaponGO) != originallySelectedWeapon) {
                weaponGO.SetActive(false);
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
        // Get weapon stats to calculate ammo based on magazine system
        WeaponStats stats = GetWeaponStats(weaponType);
        if (stats == null) return 0;
        
        // Calculate total ammo: (ExtraMagazines * MagazineCapacity)
        // Player starts with current magazine loaded + extra magazines in reserve
        int totalExtraAmmo = stats.ExtraMagazines * stats.MagazineCapacity;
        
        return totalExtraAmmo;
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

    private void SelectWeapon(int weaponIndex) {
        weapons[currentWeaponIndex]?.SetActive(false);
        currentWeaponIndex = weaponIndex;
        weapons[weaponIndex].SetActive(true);
        weapon = weapons[weaponIndex].GetComponent<Weapon>();
        player?.EquipWeapon(weapon);
        weapon.Ammo.UpdateUI();
    }

    /// <summary>
    /// Swap's Player's current weapon (based on provided weapon id)
    /// </summary>
    /// <param name="weaponIndex">Concrete weapon index</param>
    internal void SwapWeapon(int weaponIndex) {
        if (weaponIndex >= 0 && weaponIndex < weapons.Count) {
            SelectWeapon(weaponIndex);
        }
    }

    /// <summary>
    /// Swap's Player's current weapon (based on provided direction)
    /// </summary>
    /// <param name="selectNext">If True selects weapon with higher ID</param>
    internal void SwapWeapon(bool selectNext) {
        int direction = selectNext ? 1 : -1;
        int idx = (currentWeaponIndex + direction + weapons.Count) % weapons.Count;
        SelectWeapon(idx);
    }
}
