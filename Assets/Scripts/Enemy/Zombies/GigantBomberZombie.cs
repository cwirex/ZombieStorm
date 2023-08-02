using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GigantBomberZombie : Enemy
{
    [SerializeField] private float ExplosionRadius = 6f;
    [SerializeField] private float ExplosionForce = 1200f;
    [SerializeField] private float ExplosionDamage = 500f;


    private bool HasExploded = false;
    public override void Die() {
        if(HasExploded) {
            return;
        } HasExploded = true;

        NavMeshAgent navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null) {
            navAgent.enabled = false;
        }
        float explosionDelay = 1.2f;
        StartCoroutine(Explode(explosionDelay));
    }

    private IEnumerator Explode(float delay) {
        yield return new WaitForSeconds(delay);
        List<Explosive> otherExplosives = new List<Explosive>();
        var surroundingObjects = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (var surroundingObject in surroundingObjects) {
            if (surroundingObject.gameObject == gameObject) continue;

            if (surroundingObject.TryGetComponent<Rigidbody>(out var rb)) {    // Is any other RigidBody
                rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 0f, ForceMode.Impulse);
                if (surroundingObject.TryGetComponent<IDamagable>(out var damagable)) {
                    float distance = Vector3.Distance(transform.position, surroundingObject.transform.position);
                    float explosionDamageRatioFromDistance = ExplosionRadius / (1 + distance * distance);
                    float damage = ExplosionDamage * explosionDamageRatioFromDistance;

                    damagable.TakeDamage(damage);
                }
            }

            if (surroundingObject.gameObject.TryGetComponent(out Explosive otherExplosive)) {   // If object is Explosive
                // Calculate distance and Maybe add to chain of explosion:
                float distance = Vector3.Distance(transform.position, otherExplosive.transform.position);
                float chainRange = 0.5f;
                if (distance < ExplosionRadius * chainRange) {
                    otherExplosives.Add(otherExplosive);
                }
            }
        }
        foreach (var explosive in otherExplosives) {
            explosive.TriggerExplosion();
        }
        // TODO display explosion effect
        Destroy(gameObject);
    }
}
