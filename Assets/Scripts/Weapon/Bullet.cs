using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class of a flying Bullet (Instantiated by Weapon)
/// </summary>
public class Bullet : MonoBehaviour
{
    private float BulletDamage;

    public void SetDamage(float damage) {
        BulletDamage = damage;
    }

    private void OnTriggerEnter(Collider collider) {
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
