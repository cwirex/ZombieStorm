using Assets.Scripts.Weapon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 200f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;

    [SerializeField] private GameInput gameInput;
    [SerializeField] private Camera currentCamera;
    [SerializeField] private WeaponManager weapon;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private LayerMask obstaclesLayer;

    private Weapon currentWeapon;
    private float health;
    private bool isWalking = false;
    private Vector3 facingDir = Vector3.zero;
    private Rigidbody rb;

    private void Start() {
        health = maxHealth;
        gameInput.eventHandler += OnPlayerInteract;
        healthBar.SetMaxHealth(maxHealth);
        rb = GetComponent<Rigidbody>();
    }

    private void Update() {
        HandleMovement();
        HandleAim();
    }

    private void OnPlayerInteract(object sender, InteractEventArgs args) {
        if(args.variant == InteractVariant.Shoot) {
            currentWeapon.Shoot();

        } else if(args.variant == InteractVariant.Interact) {
            print("Interact!");

        }
        
    }
    private void HandleAim() {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hitInfo)) {
            Vector3 lookDir = (hitInfo.point - transform.position).normalized;
            facingDir = new Vector3(lookDir.x, 0, lookDir.z);
            transform.forward = Vector3.Slerp(transform.forward, facingDir, Time.deltaTime * rotateSpeed);
        }
    }
    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        isWalking = inputVector != Vector2.zero;

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);


        if (!isPlayerColliding(moveDir)) {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        } else {
            // Attempt to move on X or Z only
            Vector3 moveDirX = new Vector3(moveDir.x, 0f, 0f);
            Vector3 moveDirZ = new Vector3(0f, 0f, moveDir.z);
            if (!isPlayerColliding(moveDirX)) {
                transform.position += moveDirX * moveSpeed * Time.deltaTime;
            } else if (!isPlayerColliding(moveDirZ)) {
                transform.position += moveDirZ * moveSpeed * Time.deltaTime;
            }
        }
    }
    private bool isPlayerColliding(Vector3 moveDir) {
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHight = 2f;
        return Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHight, playerRadius, moveDir, moveDistance, obstaclesLayer);
    }
    internal void EquipWeapon(Weapon weapon) {
        currentWeapon = weapon;
    }
    internal void TakeDamage(float damage) {
        health -= damage;
        if(health < 0) {

            health = 0f;
        }
        healthBar.SetHealth(health);
    }
    private void ApplyKnockbackForce(Vector3 direction, float damage) {
        float knockbackPower = 10f;
        float knockback = damage * knockbackPower;
        rb.AddForce(direction * knockback, ForceMode.Impulse);
    }
    public bool IsWalking() {
        return isWalking;
    }

    void IDamagable.TakeDamage(float damage) {
        health -= damage;
        healthBar.SetHealth(health);
        if (health <= 0f) {
            Die();
        }
    }
    internal void TakeDamage(float damage, Vector3 direction) {
        ApplyKnockbackForce(direction, damage);
        TakeDamage(damage);
    }

    public void Die() {
        print("Player.Die()");
    }
}
