using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IDamagable, IKnockbackable
{
    [SerializeField] protected float Health = 100;
    [SerializeField] protected float Damage = 10f;
    [SerializeField] protected float MovementSpeed = 3.5f;
    protected NavMeshAgent agent;
        
    protected float lastTimeAttacked = 0f;
    public Rigidbody rb { get; set; }

    protected virtual void Start() {
        agent = GetComponent<NavMeshAgent>();
        SetMovementSpeed(MovementSpeed);
        rb = GetComponent<Rigidbody>();
    }

    protected virtual void SetMovementSpeed(float speed) {
        agent.speed = speed;
    }

    public virtual void TakeDamage(float damage) {
        Health -= damage;
        if (Health < 0.01)   // float precision 
            Die();
    }

    public virtual void Die() {
        StartCoroutine(DeathAnimation());
    }
    
    protected virtual IEnumerator DeathAnimation() {
        return DeathAnimationWithDuration(0.5f, 3f);
    }
    
    protected IEnumerator DeathAnimationWithDuration(float deathDelay, float fadeOutDuration) {
        // Disable movement and collision
        if (agent != null) {
            agent.enabled = false;
        }
        
        // Disable trigger collider to prevent damage
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider != null && triggerCollider.isTrigger) {
            triggerCollider.enabled = false;
        }
        
        // Freeze rigidbody
        if (rb != null) {
            rb.mass *= 999f;
        }
        
        // Wait for delay before starting fade
        yield return new WaitForSeconds(deathDelay);
        
        // Get the visual renderer
        Renderer renderer = GetComponentInChildren<Renderer>();
        
        if (renderer != null) {
            Color originalColor = renderer.material.color;
            
            float elapsed = 0f;
            while (elapsed < fadeOutDuration) {
                elapsed += Time.deltaTime;
                float progress = elapsed / fadeOutDuration;
                
                // Fade to white
                Color fadedColor = Color.Lerp(originalColor, Color.white, progress);
                renderer.material.color = fadedColor;
                
                yield return null;
            }
        } else {
            // Fallback if no renderer found
            yield return new WaitForSeconds(fadeOutDuration);
        }
        
        // Award score points before destroying
        if (ScoreManager.Instance != null) {
            ScoreManager.Instance.AddScore(this);
        }
        
        Destroy(gameObject);
    }

    protected virtual void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Player player)) {
            float awaitAfterAttack = 0.1f;
            if (Time.time - lastTimeAttacked > awaitAfterAttack) {
                AttackPlayer(player);
                lastTimeAttacked = Time.time;
            }
        }
    }

    protected virtual void AttackPlayer(Player player) {
        if(player.gameObject.TryGetComponent(out IDamagable damagable)) {
            // Calculate direction (from which hit came from)
            Vector3 direction = player.transform.position - transform.position;
            direction.Normalize();
            damagable.TakeDamage(Damage, direction);
        } else {
            Debug.LogWarning("Couldn't get Player's IDamagle component to apply damage from Enemy");
        }
    }

    public virtual void TakeDamage(float damage, Vector3 direction) {
        float knockBackBaseForce = 100f;
        ApplyKnockbackForce(direction, knockBackBaseForce * damage / (MovementSpeed*MovementSpeed));
        TakeDamage(damage);
    }

    public void ApplyKnockbackForce(Vector3 direction, float force) {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
