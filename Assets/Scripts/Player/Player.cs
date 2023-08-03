using Assets.Scripts.Player;
using Assets.Scripts.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private Camera currentCamera;
    [SerializeField] private LayerMask obstaclesLayer;

    //private Vector3 facingDir = Vector3.zero;
    private PlayerMovement playerMovement;
    private HealthController healthController;
    private InteractController interactController;

    private void Start() {
        playerMovement = GetComponent<PlayerMovement>();
        healthController = GetComponent<HealthController>();
        interactController = GetComponent<InteractController>();
    }

    private void Update() {
        HandleAim();
    }

    private void HandleAim() {
        Ray mouseRay = currentCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(mouseRay, out RaycastHit hitInfo)) {
            Vector3 lookDir = (hitInfo.point - transform.position).normalized;
            Vector3 facingDir = new Vector3(lookDir.x, 0, lookDir.z);
            transform.forward = Vector3.Slerp(transform.forward, facingDir, Time.deltaTime * rotateSpeed);
        }
    }

    internal void EquipWeapon(Weapon weapon) {
        if(interactController == null) {
            interactController = GetComponent<InteractController>();
        }
        interactController.EquipWeapon(weapon);
    }

    
    public bool IsWalking() {
        return playerMovement.IsWalking();
    }

    public void TakeDamage(float damage) {
        healthController.TakeDamage(damage);
    }

    public void TakeDamage(float damage, Vector3 direction) {
        healthController.TakeDamage(damage, direction);
    }
}
