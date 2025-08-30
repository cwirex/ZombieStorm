using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Weapon;

/// <summary>
/// Class of a flying Bullet (Instantiated by Weapon)
/// </summary>
public class Bullet : MonoBehaviour
{
    protected float BulletDamage;
    protected Weapon sourceWeapon; // Reference to the weapon that fired this bullet

    public virtual void SetDamage(float damage) {
        BulletDamage = damage;
        if (BulletDamage <= 0) {
            Debug.LogWarning("Bullet damage is <= 0!");
        }
    }
    
    /// <summary>
    /// Sets the weapon that fired this bullet for kill tracking
    /// </summary>
    public virtual void SetSourceWeapon(Weapon weapon) {
        sourceWeapon = weapon;
    }

    protected virtual void OnTriggerEnter(Collider collider) {
        if(collider.TryGetComponent(out Enemy enemy)) {
            float enemyHealthBefore = enemy.CurrentHealth;
            
            if(gameObject.TryGetComponent(out Rigidbody rb)){
                Vector3 direction = rb.velocity.normalized;
                enemy.TakeDamage(BulletDamage, direction);
            } else {
                enemy.TakeDamage(BulletDamage);
            }
            
            // Check if enemy was killed and notify source weapon for ammo regeneration
            if (enemyHealthBefore > 0 && enemy.CurrentHealth <= 0 && sourceWeapon != null) {
                sourceWeapon.OnEnemyKilled();
            }
            
            Destroy(gameObject);
        }
        else if (collider.TryGetComponent(out Explosive explosive)){
            float delay = 0.1f;
            explosive.TriggerExplosion(delay);
            Destroy(gameObject);
        }
        else if (collider.GetComponent<Obstacle>() != null) {
            // Hit a wall
            Destroy(gameObject);
        }
    }
}
