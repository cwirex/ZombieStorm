using System.Collections;
using UnityEngine;

public class HealthController : MonoBehaviour, IDamagable, IKnockbackable {
    [SerializeField] private float maxHealth = 1000f;
    [SerializeField] private float knockbackResistance = 100f;
    [SerializeField] private HealthBar healthBar;

    private float health;
    public Rigidbody rb { get; set; }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        health = maxHealth;
        healthBar.SetMaxHealth(health);
    }

    private void updateHealtBar() {
        healthBar.SetHealth(health);
    }

    public void TakeDamage(float damage) {
        health -= damage;
        Debug.Log("Player took damage: " + (int)damage);
        updateHealtBar();

        if (health <= 0f) {
            Die();
        }
    }

    public void TakeDamage(float damage, Vector3 direction) {
        float knockBackBaseForce = 1000f;
        TakeDamage(damage);
        ApplyKnockbackForce(direction, damage * knockBackBaseForce);
    }

    public bool Heal(float healAmount) {
        if(health < maxHealth) {
            health += healAmount;
            health = Mathf.Clamp(health, 0f, maxHealth);
            updateHealtBar();
            return true;
        }
        return false;
        
    }

    public void Die() {
        print("Player.Die()");
        // Time.timeScale = 0f;
    }

    public void ApplyKnockbackForce(Vector3 direction, float force) {
        rb.AddForce(direction * force / knockbackResistance, ForceMode.Impulse);
    }
}