using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameInput : MonoBehaviour
{
    private PlayerInputActions playerInputActions;
    public EventHandler<InteractEventArgs> eventHandler;
    
    void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.Shoot.performed += Shoot_performed;
    }

    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        InvokeEventHandler(InteractVariant.Shoot);

    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        InvokeEventHandler(InteractVariant.Interact);
    }

    private void InvokeEventHandler(InteractVariant variant) {
        InteractEventArgs args = new InteractEventArgs(variant);
        eventHandler?.Invoke(this, args);
    }

    public Vector2 GetMovementVectorNormalized() {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        inputVector = inputVector.normalized;

        return inputVector;
    }
}

public enum InteractVariant {
    Interact, Shoot
}

public class InteractEventArgs : EventArgs {
    public InteractVariant variant { get; }

    public InteractEventArgs(InteractVariant v) {
        variant = v;
    }
}


