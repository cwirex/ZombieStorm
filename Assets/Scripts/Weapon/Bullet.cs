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
    }

    protected virtual void OnTriggerEnter(Collider collider) {
        if(collider.TryGetComponent(out Enemy enemy)) {
            // Hit an enemy
            enemy.TakeDamage(BulletDamage);
            Destroy(gameObject);

            if(BulletDamage <= 0) {
                Debug.LogWarning("Bullet damage is <= 0!");
            }
        }
        if (collider.TryGetComponent(out Explosive explosive)){
            explosive.TriggerExplosion();
        }
        if (collider.GetComponent<Obstacle>() != null) {
            // Hit a wall
            Destroy(gameObject);
        }
    }
}
