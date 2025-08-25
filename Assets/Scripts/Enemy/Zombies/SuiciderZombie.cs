using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuiciderZombie : Enemy
{
    [SerializeField] private float ExplosionRadius = 3f;
    [SerializeField] private float ExplosionForce = 500f;
    
    protected override IEnumerator DeathAnimation() {
        // Faster death animation (half duration)
        return DeathAnimationWithDuration(0.25f, 1.5f);
    }

    /// <summary>
    /// Explode on contact with player.
    /// Damage all objects around.
    /// Don't trigger other explosions.
    /// </summary>
    /// <param name="player">Ignored in this use case</param>
    protected override void AttackPlayer(Player player) {
        
        var surroundingObjects = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (var surroundingObject in surroundingObjects) {
            if (surroundingObject.TryGetComponent<Rigidbody>(out var rb)) {    // Is any other RigidBody
                rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 0f, ForceMode.Impulse);
                if (surroundingObject.TryGetComponent<IDamagable>(out var damagable)) {
                    float distance = Vector3.Distance(transform.position, surroundingObject.transform.position);
                    float explosionDamageRatioFromDistance = ExplosionRadius / (1 + distance * distance);
                    float damage = Damage * explosionDamageRatioFromDistance;

                    damagable.TakeDamage(damage);
                }
            }

        }

        var controller = FindObjectOfType<ExplosionController>();
        controller?.MakeSmallExplosion(transform.position);

        Destroy(gameObject);
    }
}
