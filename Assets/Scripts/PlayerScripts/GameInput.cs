using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.UIElements.ToolbarMenu;


public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    public EventHandler<InteractEventArgs> eventHandler;
    
    void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.Shoot.performed += Shoot_performed;
        playerInputActions.Player.Shoot.canceled += Shoot_canceled;
        playerInputActions.Player.SelectWeapon.performed += SelectWeapon_performed;
        playerInputActions.Player.Heal.performed += Heal_performed;
        playerInputActions.Player.Exit.performed += Exit_performed; ;
    }

    private void Exit_performed(InputAction.CallbackContext obj) {
        InvokeEventHandler(InteractVariant.ExitPerformed);
    }

    private void Heal_performed(InputAction.CallbackContext obj) {
        InvokeEventHandler(InteractVariant.HealPerformed);
    }

    private void Shoot_canceled(InputAction.CallbackContext context) {
        InvokeEventHandler(InteractVariant.ShootCanceled);
    }

    private void Shoot_performed(InputAction.CallbackContext obj) {
        InvokeEventHandler(InteractVariant.ShootPerformed);

    }

    private void Interact_performed(InputAction.CallbackContext obj) {
        InvokeEventHandler(InteractVariant.Interact);
    }

    private void SelectWeapon_performed(InputAction.CallbackContext context) {
        float scrollInput = context.ReadValue<float>();
        bool scrolledUp = scrollInput > 0f;
        InteractVariant variant = scrolledUp ? InteractVariant.SelectWeaponNext : InteractVariant.SelectWeaponPrevious;
        InvokeEventHandler(variant);
    }

    private void InvokeEventHandler(InteractVariant variant) {
        eventHandler?.Invoke(this, new InteractEventArgs(variant));
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}

public enum InteractVariant {
    Interact, 
    ShootPerformed,
    ShootCanceled,
    SelectWeaponNext,
    SelectWeaponPrevious,
    HealPerformed,
    ExitPerformed,
}

public class InteractEventArgs : EventArgs {
    public InteractVariant variant { get; }

    public InteractEventArgs(InteractVariant v) {
        variant = v;
    }
}


