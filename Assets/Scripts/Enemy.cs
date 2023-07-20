using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [SerializeField] private int health = 100;
    [SerializeField] private float damage = 10f;

    public void Damage(int damage) {
        health -= damage;
        if (health < 0) Kill();
    }

    public void Kill() {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();

            player.TakeDamage(damage, direction);
        }
    }
}
