using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        playerInputActions.Player.SelectWeapon.performed += SelectWeapon_performed;
    }

    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        var args = new InteractEventArgs(InteractVariant.Shoot);
        InvokeEventHandler(args);

    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        var args = new InteractEventArgs(InteractVariant.Interact);
        InvokeEventHandler(args);
    }

    private void SelectWeapon_performed(UnityEngine.InputSystem.InputAction.CallbackContext context) {
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
    Shoot, 
    SelectWeaponNext,
    SelectWeaponPrevious,
}

public class InteractEventArgs : EventArgs {
    public InteractVariant variant { get; }

    public InteractEventArgs(InteractVariant v) {
        variant = v;
    }
}


