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
        playerInputActions.Player.SelectWeapon1.performed += SelectWeapon1_performed;
        playerInputActions.Player.SelectWeapon2.performed += SelectWeapon2_performed;
        playerInputActions.Player.SelectWeapon3.performed += SelectWeapon3_performed;
        playerInputActions.Player.SelectWeapon4.performed += SelectWeapon4_performed;
        playerInputActions.Player.SelectWeapon5.performed += SelectWeapon5_performed;
        playerInputActions.Player.SelectWeapon6.performed += SelectWeapon6_performed;
        playerInputActions.Player.SelectWeapon7.performed += SelectWeapon7_performed;
        playerInputActions.Player.SelectWeapon8.performed += SelectWeapon8_performed;
        playerInputActions.Player.Heal.performed += Heal_performed;
        playerInputActions.Player.Exit.performed += Exit_performed; ;
    }
    
    private void OnDestroy() {
        if (playerInputActions != null) {
            playerInputActions.Player.Interact.performed -= Interact_performed;
            playerInputActions.Player.Shoot.performed -= Shoot_performed;
            playerInputActions.Player.Shoot.canceled -= Shoot_canceled;
            playerInputActions.Player.SelectWeapon.performed -= SelectWeapon_performed;
            playerInputActions.Player.SelectWeapon1.performed -= SelectWeapon1_performed;
            playerInputActions.Player.SelectWeapon2.performed -= SelectWeapon2_performed;
            playerInputActions.Player.SelectWeapon3.performed -= SelectWeapon3_performed;
            playerInputActions.Player.SelectWeapon4.performed -= SelectWeapon4_performed;
            playerInputActions.Player.SelectWeapon5.performed -= SelectWeapon5_performed;
            playerInputActions.Player.SelectWeapon6.performed -= SelectWeapon6_performed;
            playerInputActions.Player.SelectWeapon7.performed -= SelectWeapon7_performed;
            playerInputActions.Player.SelectWeapon8.performed -= SelectWeapon8_performed;
            playerInputActions.Player.Heal.performed -= Heal_performed;
            playerInputActions.Player.Exit.performed -= Exit_performed;
            playerInputActions.Dispose();
        }
    }

    private void Exit_performed(InputAction.CallbackContext obj) {
        // If shop is open, close it first
        if (Assets.Scripts.Shop.ShopManager.Instance != null && Assets.Scripts.Shop.ShopManager.Instance.IsShopOpen) {
            Assets.Scripts.Shop.ShopManager.Instance.CloseShop();
            return;
        }
        
        // Handle escape key based on current game state
        if (GameManager.Instance != null) {
            switch (GameManager.Instance.CurrentState) {
                case GameState.Playing:
                    GameManager.Instance.PauseGame();
                    break;
                case GameState.Paused:
                    GameManager.Instance.ResumeGame();
                    break;
                case GameState.MainMenu:
                case GameState.GameOver:
                    // In menu states, escape does nothing or could quit
                    break;
            }
        } else {
            // Fallback to old behavior if GameManager not found
            InvokeEventHandler(InteractVariant.ExitPerformed);
        }
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

    private void SelectWeapon1_performed(InputAction.CallbackContext context) => InvokeEventHandler(InteractVariant.SelectWeapon1);
    private void SelectWeapon2_performed(InputAction.CallbackContext context) => InvokeEventHandler(InteractVariant.SelectWeapon2);
    private void SelectWeapon3_performed(InputAction.CallbackContext context) => InvokeEventHandler(InteractVariant.SelectWeapon3);
    private void SelectWeapon4_performed(InputAction.CallbackContext context) => InvokeEventHandler(InteractVariant.SelectWeapon4);
    private void SelectWeapon5_performed(InputAction.CallbackContext context) => InvokeEventHandler(InteractVariant.SelectWeapon5);
    private void SelectWeapon6_performed(InputAction.CallbackContext context) => InvokeEventHandler(InteractVariant.SelectWeapon6);
    private void SelectWeapon7_performed(InputAction.CallbackContext context) => InvokeEventHandler(InteractVariant.SelectWeapon7);
    private void SelectWeapon8_performed(InputAction.CallbackContext context) => InvokeEventHandler(InteractVariant.SelectWeapon8);

    private void InvokeEventHandler(InteractVariant variant) {
        // Block input when shop is open
        if (Assets.Scripts.Shop.ShopManager.Instance != null && Assets.Scripts.Shop.ShopManager.Instance.IsShopOpen) {
            return;
        }
        
        // Only process gameplay input when actually playing
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) {
            return;
        }
        
        eventHandler?.Invoke(this, new InteractEventArgs(variant));
    }

    public Vector2 GetMovementVectorNormalized() {
        // Block movement when shop is open
        if (Assets.Scripts.Shop.ShopManager.Instance != null && Assets.Scripts.Shop.ShopManager.Instance.IsShopOpen) {
            return Vector2.zero;
        }
        
        // Only allow movement when playing
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Playing) {
            return Vector2.zero;
        }
        
        // Safety check for null playerInputActions
        if (playerInputActions == null) {
            return Vector2.zero;
        }
        
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
    SelectWeapon1,
    SelectWeapon2,
    SelectWeapon3,
    SelectWeapon4,
    SelectWeapon5,
    SelectWeapon6,
    SelectWeapon7,
    SelectWeapon8,
    HealPerformed,
    ExitPerformed,
}

public class InteractEventArgs : EventArgs {
    public InteractVariant variant { get; }

    public InteractEventArgs(InteractVariant v) {
        variant = v;
    }
}


