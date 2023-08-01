using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] protected float Health = 100;
    [SerializeField] protected float Damage = 10f;
    [SerializeField] protected float MovementSpeed = 3.5f;
    protected NavMeshAgent agent;

    protected virtual void Start() {
        agent = GetComponent<NavMeshAgent>();
        SetMovementSpeed(MovementSpeed);
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
            AttackPlayer(player);
        }
    }

    protected virtual void AttackPlayer(Player player) {
        // Calculate direction (from which hit came from)
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();

        player.TakeDamage(Damage, direction);
    }
}
