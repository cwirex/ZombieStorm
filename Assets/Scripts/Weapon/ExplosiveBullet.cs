using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Audio;
using System;

/// <summary>
/// Class of a flying Bullet (Instantiated by Weapon)
/// </summary>
public class ExplosiveBullet : Bullet
{
    [SerializeField] private float ExplosionRadius = 2.5f;
    [SerializeField] private float ExplosionForce = 2000f;

    override protected void OnTriggerEnter(Collider collider) {
        if (collider.TryGetComponent(out Enemy _) || collider.GetComponent<Obstacle>() != null || collider.GetComponent<Explosive>() != null) {
            // Explode
            List<Explosive> otherExplosives = new();
            var surroundingObjects = Physics.OverlapSphere(transform.position, ExplosionRadius);
            foreach (var surroundingObject in surroundingObjects) {
                if (surroundingObject.gameObject == gameObject) continue;

                if (surroundingObject.TryGetComponent<Rigidbody>(out var rb)) {    // Is any other RigidBody
                    rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 0f, ForceMode.Impulse);
                    if (surroundingObject.TryGetComponent<IDamagable>(out var damagable)) {
                        float distance = Vector3.Distance(transform.position, surroundingObject.transform.position);
                        float damageMultiplier = Mathf.Clamp01((ExplosionRadius - (distance - .8f)) / ExplosionRadius); // 1 at center (+1m), 0 at edge
                        damageMultiplier = ((damageMultiplier + 1f) * (damageMultiplier + 1f)) - 1f; // Quadratic curve for damage falloff  
                        float damage = BulletDamage * damageMultiplier;

                        float finalDamage = damage * damageMultiplier; // More damage closer to center

                        damagable.TakeDamage(damage);
                    }
                }

                if (surroundingObject.gameObject.TryGetComponent(out Explosive otherExplosive)) {   // If object is Explosive
                    // Calculate distance and Maybe add to chain of explosion:
                    float distance = Vector3.Distance(transform.position, otherExplosive.transform.position);
                    float chainRange = 0.8f;
                    if (distance < ExplosionRadius * chainRange) {
                        otherExplosives.Add(otherExplosive);
                    }
                }
            }
            foreach (var explosive in otherExplosives) {
                explosive.TriggerExplosion();
            }
            
            // Play RPG explosion sound
            SoundEvents.TriggerRPGExplosion();
            
            var controller = FindObjectOfType<ExplosionController>();
            controller?.MakeSmallExplosion(transform.position);
            Destroy(gameObject);
        }
    }
}
