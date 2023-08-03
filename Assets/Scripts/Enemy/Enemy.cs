using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IDamagable, IKnockbackable
{
    [SerializeField] protected float Health = 100;
    [SerializeField] protected float Damage = 10f;
    [SerializeField] protected float MovementSpeed = 3.5f;
    protected NavMeshAgent agent;
        
    protected float lastTimeAttacked = 0f;
    public Rigidbody rb { get; set; }

    protected virtual void Start() {
        agent = GetComponent<NavMeshAgent>();
        SetMovementSpeed(MovementSpeed);
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void SetMovementSpeed(float speed) {
        agent.speed = speed;
    }

    public virtual void TakeDamage(float damage) {
        Health -= damage;
        if (Health < 0.01)   // float precision 
            Die();
    }

    public virtual void Die() {
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            float awaitAfterAttack = 1f;
            if (Time.time - lastTimeAttacked > awaitAfterAttack) {
                AttackPlayer(player);
                lastTimeAttacked = Time.time;
            }
        }
        
    }

    protected virtual void AttackPlayer(Player player) {
        if(player.gameObject.TryGetComponent(out IDamagable damagable)) {
            // Calculate direction (from which hit came from)
            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();
            damagable.TakeDamage(Damage, direction);
        } else {
            Debug.LogWarning("Couldn't get Player's IDamagle component to apply damage from Enemy");
        }
    }

    public virtual void TakeDamage(float damage, Vector3 direction) {
        float knockBackBaseForce = 100f;
        ApplyKnockbackForce(direction, knockBackBaseForce * damage / (MovementSpeed*MovementSpeed));
        TakeDamage(damage);
    }

    public void ApplyKnockbackForce(Vector3 direction, float force) {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
