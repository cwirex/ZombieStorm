using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class of a flying Bullet (Instantiated by Weapon)
/// </summary>
public class ToxicBullet : Bullet
{
    override protected void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out Player player)) {
            if (gameObject.TryGetComponent(out Rigidbody rb)) {
                Vector3 direction = rb.velocity.normalized;
                player.TakeDamage(BulletDamage, direction);
            } else {
                player.TakeDamage(BulletDamage);
            }

            Destroy(gameObject);
        }
        if (collider.GetComponent<Obstacle>() != null) {
            // Hit a wall
            Destroy(gameObject);
        }
    }
}
