using Assets.Scripts.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        SelectWeapon(currentWeaponIndex);
    }

    private void InstantiateWeapons() {
        foreach (var pf in weaponsPrefabs) {
            GameObject weaponGO = Instantiate(pf, transform);
            weaponGO.SetActive(false);
            weapons.Add(weaponGO);
        }
    }

    private void SelectWeapon(int weaponIndex) {
        weapons[currentWeaponIndex]?.SetActive(false);
        currentWeaponIndex = weaponIndex;
        weapons[weaponIndex].SetActive(true);
        weapon = weapons[weaponIndex].GetComponent<Weapon>();
        player?.EquipWeapon(weapon);
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
