using Assets.Scripts.Weapon;
using System.Collections;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Toxic : Enemy {

    [SerializeField] private GameObject weaponPF;
    [SerializeField] private float Range;
    [SerializeField] private float FireRate;
    [SerializeField] private float BulletSpeed;


    private ToxicWeapon weapon;
    private Transform weaponTF;
    private Transform playerTF;

    private void Awake() {
        weaponTF = transform.Find("Weapon");
        if (weaponTF == null) {
            weaponTF = transform;
        }
    }

    protected override void Start() {
        base.Start();
        var weaponGO = Instantiate(weaponPF, weaponTF);
        weapon = weaponGO.GetComponent<ToxicWeapon>();
        weapon.SetStats(new WeaponStats(Damage, Range, FireRate, BulletSpeed, 0));
        playerTF = FindObjectOfType<Player>().transform;
    }

    void Update() {
        float distanceToPlayer = (playerTF.position - weaponTF.position).magnitude;
        if(distanceToPlayer <= Range) {
            weapon.OnShootPerformed();
        } else {
            weapon.OnShootCanceled();
        }
    }
}
