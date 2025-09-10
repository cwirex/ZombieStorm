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
    private UIController uiController;

    private void Start() {
        weaponManager = GetComponentInChildren<WeaponManager>();
        playerInventory = GetComponent<PlayerInventory>();
        uiController = FindObjectOfType<UIController>();
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
        } else if (args.variant == InteractVariant.SelectWeapon1) {
            weaponManager?.SelectWeaponByType(EWeapons.PISTOL);    // Key 1 = Pistol
        } else if (args.variant == InteractVariant.SelectWeapon2) {
            weaponManager?.SelectWeaponByType(EWeapons.UZI);       // Key 2 = UZI  
        } else if (args.variant == InteractVariant.SelectWeapon3) {
            weaponManager?.SelectWeaponByType(EWeapons.SHOTGUN);   // Key 3 = Shotgun
        } else if (args.variant == InteractVariant.SelectWeapon4) {
            weaponManager?.SelectWeaponByType(EWeapons.M4);        // Key 4 = M4
        } else if (args.variant == InteractVariant.SelectWeapon5) {
            weaponManager?.SelectWeaponByType(EWeapons.AWP);       // Key 5 = AWP
        } else if (args.variant == InteractVariant.SelectWeapon6) {
            weaponManager?.SelectWeaponByType(EWeapons.M249);      // Key 6 = M249
        } else if (args.variant == InteractVariant.SelectWeapon7) {
            weaponManager?.SelectWeaponByType(EWeapons.RPG7);      // Key 7 = RPG7
        } else if (args.variant == InteractVariant.SelectWeapon8) {
            weaponManager?.SelectWeaponByType(EWeapons.FLAMETHROWER); // Key 8 = Flamethrower
        } else if (args.variant == InteractVariant.ReloadPerformed) {
            currentWeapon?.StartReload(); // R key = Manual reload
        } else if(args.variant == InteractVariant.ExitPerformed) {
            uiController?.TogglePause();
        }
    }

    internal void EquipWeapon(Weapon weapon) {
        currentWeapon = weapon;
    }

    
}
