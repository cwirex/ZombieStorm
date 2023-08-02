using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class of a flying Bullet (Instantiated by Weapon)
/// </summary>
public class Bullet : MonoBehaviour
{
    protected float BulletDamage;

    public virtual void SetDamage(float damage) {
        BulletDamage = damage;
        if (BulletDamage <= 0) {
            Debug.LogWarning("Bullet damage is <= 0!");
        }
    }

    protected virtual void OnTriggerEnter(Collider collider) {
        if(collider.TryGetComponent(out Enemy enemy)) {
            if(gameObject.TryGetComponent(out Rigidbody rb)){
                Vector3 direction = rb.velocity.normalized;
                enemy.TakeDamage(BulletDamage, direction);
            } else {
                enemy.TakeDamage(BulletDamage);
            }
            
            Destroy(gameObject);
        }
        if (collider.TryGetComponent(out Explosive explosive)){
            float delay = 0.1f;
            explosive.TriggerExplosion(delay);
        }
        if (collider.GetComponent<Obstacle>() != null) {
            // Hit a wall
            Destroy(gameObject);
        }
    }
}
