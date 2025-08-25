using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PhoenixZombie : Enemy
{
    [SerializeField] Material respawnMaterial;
    [SerializeField] float respawnDuration = 3f;
    [SerializeField] float Boost = 4f;  // min 2f

    enum State {
        Alive,
        Respawning,
        Respawned
    }
    private State state;
    private float MaxHealth;
    private NavMeshAgent navAgent;
    private Renderer mRenderer;

    protected override void Start() {
        base.Start();
        navAgent = GetComponent<NavMeshAgent>();
        mRenderer = GetComponentInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();
        state = State.Alive;
        MaxHealth = Health;
    }

    public override void Die() {
        if (state == State.Alive) {
            state = State.Respawning;
            float rbFreeze = 999f;
            rb.mass *= rbFreeze;
            StartCoroutine(Respawn());
            ApplyRespawnBoosts();
            rb.mass /= rbFreeze;
        } else if(state == State.Respawned) {
            // Use death animation for final death after resurrection
            StartCoroutine(DeathAnimation());
        }
    }
    private IEnumerator Respawn() {
        if (navAgent != null) {
            navAgent.enabled = false;
        }
        yield return new WaitForSeconds(respawnDuration);
        mRenderer.material = respawnMaterial;
        if (navAgent != null) {
            navAgent.enabled = true;
        }
        state = State.Respawned;
    }

    private void ApplyRespawnBoosts() {
        Health = MaxHealth * Boost;
        Damage *= Boost;
        navAgent.speed = MovementSpeed * Boost / 2f;
        navAgent.acceleration *= Boost;
        navAgent.angularSpeed *= Boost;
        rb.mass *= Boost / 2f;
    }
}
