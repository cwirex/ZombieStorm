using Assets.Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds Player Weapons
/// </summary>
public class WeaponManager : MonoBehaviour {
    [SerializeField] List<GameObject> weaponPrefabs;
    [SerializeField] Player player;

    private Weapon weapon;

    private void Awake() {
        GameObject currentWeaponGO = Instantiate(weaponPrefabs[3], transform);
        weapon = currentWeaponGO.GetComponent<Weapon>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player.EquipWeapon(weapon);
    }
}
