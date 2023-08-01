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
    }

    private void Shoot_canceled(InputAction.CallbackContext context) {
        var args = new InteractEventArgs(InteractVariant.ShootCanceled);
        InvokeEventHandler(args);
    }

    private void Shoot_performed(InputAction.CallbackContext obj) {
        var args = new InteractEventArgs(InteractVariant.ShootPerformed);
        InvokeEventHandler(args);

    }

    private void Interact_performed(InputAction.CallbackContext obj) {
        var args = new InteractEventArgs(InteractVariant.Interact);
        InvokeEventHandler(args);
    }

    private void SelectWeapon_performed(InputAction.CallbackContext context) {
        float scrollInput = context.ReadValue<float>();
        bool scrolledUp = scrollInput > 0f;
        InteractVariant variant = scrolledUp ? InteractVariant.SelectWeaponNext : InteractVariant.SelectWeaponPrevious;
        var args = new InteractEventArgs(variant);
        InvokeEventHandler(args);
    }

    private void InvokeEventHandler(InteractEventArgs args) {
        eventHandler?.Invoke(this, args);
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
}

public class InteractEventArgs : EventArgs {
    public InteractVariant variant { get; }

    public InteractEventArgs(InteractVariant v) {
        variant = v;
    }
}


