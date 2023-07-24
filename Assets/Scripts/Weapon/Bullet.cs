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
        if(collider.GetComponent<Obstacle>() != null) {
            // Hit a wall
            Destroy(gameObject);
        }
        if(collider.TryGetComponent<Enemy>(out Enemy enemy)) {
            // Hit an enemy
            enemy.Damage(BulletDamage);
            Destroy(gameObject);

            if(BulletDamage == 0) {
                Debug.LogWarning("Bullet damage is 0!");
            }
        }
    }
}
