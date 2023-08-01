using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    [SerializeField] private float ExplosionRadius = 5f;
    [SerializeField] private float ExplosionForce = 1000f;
    [SerializeField] private float DamageForceRatio = 0.7f;
    [SerializeField] private float chainExplosionDelay = 0.25f;

    private bool HasExploded = false;
    public void TriggerExplosion(float delay=0f) {
        if (!HasExploded) {
            if (delay == 0f) {
                Explode();
            } else {
                StartCoroutine(TriggerExplosionAfter(delay));
            }
        }
    }

    private IEnumerator TriggerExplosionAfter(float delay) {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    private void Explode() {
        if (HasExploded) return;
        HasExploded = true;

        List<Explosive> otherExplosives = new List<Explosive>();

        var surroundingObjects = Physics.OverlapSphere(transform.position, ExplosionRadius);
        foreach (var surroundingObject in surroundingObjects) {
            if (surroundingObject.gameObject.TryGetComponent(out Explosive otherExplosive)) {   // If object is Explosive
                // Calculate distance and Maybe add to chain of explosion:
                float distance = Vector3.Distance(transform.position, otherExplosive.transform.position);
                float chainRange = 0.707f;
                if (distance < ExplosionRadius * chainRange) {
                    otherExplosives.Add(otherExplosive);
                }
            } else if(surroundingObject.TryGetComponent<Rigidbody>(out var rb)) {    // Is any other RigidBody
                rb.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, 0f, ForceMode.Impulse);
                if(surroundingObject.TryGetComponent<IDamagable>(out var damagable)) {
                    float distance = Vector3.Distance(transform.position, surroundingObject.transform.position);
                    float damage = DamageForceRatio * ExplosionForce / (distance * distance);
                    damagable.TakeDamage(damage);
                }
            }
            
        }
        // TODO display explosion effect

        foreach (var explosive in otherExplosives) {
            if(explosive.gameObject != gameObject) {
                explosive.TriggerExplosion(chainExplosionDelay);
            }
        }
        Destroy(gameObject);

    }
}
