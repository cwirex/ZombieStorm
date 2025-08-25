using System.Collections;
using UnityEngine;
using Assets.Scripts.Player;

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
    
    public bool HealByPercentage(float percentage) {
        if(health < maxHealth) {
            float healAmount = maxHealth * percentage;
            health += healAmount;
            health = Mathf.Clamp(health, 0f, maxHealth);
            updateHealtBar();
            return true;
        }
        return false;
    }

    public void Die() {
        Debug.Log("Player died!");
        StartCoroutine(PlayerDeathAnimation());
    }
    
    private IEnumerator PlayerDeathAnimation() {
        // Disable player movement and interaction immediately
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null) {
            playerMovement.enabled = false;
        }
        
        InteractController interactController = GetComponent<InteractController>();
        if (interactController != null) {
            interactController.enabled = false;
        }
        
        // Start slowdown immediately
        StartCoroutine(SlowDownTime());
        
        // Wait  before showing game over
        yield return new WaitForSeconds(0.2f);
        
        // Trigger game over through GameManager
        if (GameManager.Instance != null) {
            GameManager.Instance.GameOver();
        }
    }
    
    private IEnumerator SlowDownTime() {
        float startTimeScale = 0.4f;
        float endTimeScale = 0f;
        float duration = 2.2f;
        float elapsed = 0f;
        
        while (elapsed < duration) {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time so it continues even as timeScale changes
            float progress = elapsed / duration;
            Time.timeScale = Mathf.Lerp(startTimeScale, endTimeScale, progress);
            yield return null;
        }
        
        Time.timeScale = 0f;
    }

    public void ApplyKnockbackForce(Vector3 direction, float force) {
        rb.AddForce(direction * force / knockbackResistance, ForceMode.Impulse);
    }
}