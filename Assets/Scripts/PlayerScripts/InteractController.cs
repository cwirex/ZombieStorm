using Assets.Scripts.Player;
using Assets.Scripts.PlayerScripts;
using Assets.Scripts.Weapon;
using System.Collections;
using UnityEngine;

public class InteractController : MonoBehaviour {
    [SerializeField] internal GameInput gameInput;

    private WeaponManager weaponManager;
    private Weapon currentWeapon;
    private PlayerInventory playerInventory;

    private void Start() {
        weaponManager = GetComponentInChildren<WeaponManager>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void OnEnable() {
        gameInput.eventHandler += OnPlayerInteract;

    }

    private void OnDisable() {
        gameInput.eventHandler -= OnPlayerInteract;
    }

    private void OnPlayerInteract(object sender, InteractEventArgs args) {
        if (args.variant == InteractVariant.ShootPerformed) {
            currentWeapon?.OnShootPerformed();
        } else if (args.variant == InteractVariant.ShootCanceled) {
            currentWeapon?.OnShootCanceled();
        } else if (args.variant == InteractVariant.Interact) {
            playerInventory?.UseItem<TNT>();
        } else if (args.variant == InteractVariant.HealPerformed) {
            playerInventory?.UseItem<Medkit>();
        } else if (args.variant == InteractVariant.SelectWeaponNext) {
            weaponManager?.SwapWeapon(true);
        } else if (args.variant == InteractVariant.SelectWeaponPrevious) {
            weaponManager?.SwapWeapon(false);
        }
    }

    internal void EquipWeapon(Weapon weapon) {
        currentWeapon = weapon;
    }

    
}
