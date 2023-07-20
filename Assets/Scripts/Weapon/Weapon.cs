using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] List<GameObject> weaponPrefabs;
    [SerializeField] Player player;

    private GameObject currentWeaponGO;
    private IWeapon weapon;

    private void Awake() {
        currentWeaponGO = Instantiate(weaponPrefabs[0], transform);
        weapon = currentWeaponGO.GetComponent<IWeapon>();
    }

    // Start is called before the first frame update
    void Start()
    {
        player.EquipWeapon(weapon);
    }
}
