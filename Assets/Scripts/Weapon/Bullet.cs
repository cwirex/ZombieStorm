using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void OnTriggerEnter(Collider collider) {
        if(collider.GetComponent<Obstacle>() != null) {
            // Hit a wall
            Destroy(gameObject);
        }
        if(collider.TryGetComponent<Enemy>(out Enemy enemy)) {
            // Hit an enemy
            int bulletDamage = 30;
            enemy.Damage(bulletDamage);
            Destroy(gameObject);
        }
    }
}
