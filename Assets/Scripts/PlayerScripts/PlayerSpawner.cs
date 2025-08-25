using UnityEngine;
using UnityEngine.AI;
using Assets.Scripts.Player;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Player References")]
    [SerializeField] private Player player;
    [SerializeField] private bool findPlayerAutomatically = true;
    
    [Header("Wave Transition")]
    [SerializeField] private float waveEndDelay = 0.5f;
    [SerializeField] private float countdownDuration = 3f;
    
    [Header("Debug Info")]
    [SerializeField] private bool hasSpawned = false;
    [SerializeField] private bool playerFrozen = false;
    
    public System.Action OnPlayerSpawned;
    public System.Action OnCountdownStarted;
    public System.Action OnCountdownFinished;
    
    public Vector3 SpawnPosition => transform.position;
    
    private void Start()
    {
        if (findPlayerAutomatically && player == null)
        {
            player = FindObjectOfType<Player>();
        }
        
        Debug.Log($"PlayerSpawner ready at position {transform.position}");
    }
    
    public void SpawnPlayer()
    {
        if (player == null)
        {
            Debug.LogError("PlayerSpawner: No player assigned!");
            return;
        }
        
        StartCoroutine(TeleportPlayerSafely());
    }
    
    private System.Collections.IEnumerator TeleportPlayerSafely()
    {
        // Get all components that might block teleportation
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Collider playerCollider = player.GetComponent<Collider>();
        NavMeshAgent navAgent = player.GetComponent<NavMeshAgent>();
        
        // Store original states
        bool wasKinematic = playerRb != null && playerRb.isKinematic;
        bool hadCollider = playerCollider != null && playerCollider.enabled;
        bool hadNavAgent = navAgent != null && navAgent.enabled;
        
        // Disable all movement constraints
        if (playerRb) playerRb.isKinematic = true;
        if (playerCollider) playerCollider.enabled = false;
        if (navAgent) navAgent.enabled = false;
        
        Debug.Log($"Disabled NavAgent: {hadNavAgent}, Rigidbody: {!wasKinematic}, Collider: {hadCollider}");
        
        // Wait a frame to ensure components are disabled
        yield return null;
        
        // Teleport
        player.transform.position = transform.position;
        player.transform.rotation = transform.rotation;
        
        Debug.Log($"Teleported player to: {transform.position}, actual position: {player.transform.position}");
        
        // Wait another frame
        yield return null;
        
        // Restore components
        if (playerRb) playerRb.isKinematic = wasKinematic;
        if (playerCollider && hadCollider) playerCollider.enabled = true;
        if (navAgent && hadNavAgent) 
        {
            navAgent.enabled = true;
            // Warp the NavMeshAgent to the new position
            if (navAgent.isOnNavMesh)
            {
                navAgent.Warp(transform.position);
            }
        }
        
        hasSpawned = true;
        
        Debug.Log($"Player teleported safely to position: {transform.position}");
        OnPlayerSpawned?.Invoke();
    }
    
    public void ResetSpawner()
    {
        hasSpawned = false;
    }
    
    public bool HasSpawnedPlayer()
    {
        return hasSpawned;
    }
    
    // Public methods for external control
    public void SetPlayer(Player newPlayer)
    {
        player = newPlayer;
    }
    
    // Integration with game systems
    public void OnGameStart()
    {
        SpawnPlayer();
    }
    
    public void OnWaveStart()
    {
        // Just enable player controls - teleportation already happened during wave completion
        StartCoroutine(StartWaveCountdown());
    }
    
    public void OnWaveCompleted()
    {
        // Start the wave transition sequence
        StartCoroutine(WaveCompletionSequence());
    }
    
    private System.Collections.IEnumerator WaveCompletionSequence()
    {
        Debug.Log("Wave completed - starting transition sequence");
        
        // Wait 0.5 seconds after last zombie dies
        yield return new WaitForSeconds(waveEndDelay);
        
        // Freeze player
        FreezePlayer();
        
        // Teleport player safely
        yield return StartCoroutine(TeleportPlayerSafely());
        
        Debug.Log("Player teleported - ready for next wave");
    }
    
    private System.Collections.IEnumerator StartWaveCountdown()
    {
        Debug.Log("Starting wave countdown");
        OnCountdownStarted?.Invoke();
        
        // Countdown 3...2...1...
        for (int i = (int)countdownDuration; i > 0; i--)
        {
            Debug.Log($"Wave starting in: {i}");
            yield return new WaitForSeconds(1f);
        }
        
        Debug.Log("Wave started!");
        
        // Unfreeze player
        UnfreezePlayer();
        
        OnCountdownFinished?.Invoke();
    }
    
    public void FreezePlayer()
    {
        if (player == null) return;
        
        playerFrozen = true;
        
        // Disable movement components
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement) playerMovement.enabled = false;
        
        // Disable input
        var gameInput = player.GetComponent<GameInput>();
        if (gameInput) gameInput.enabled = false;
        
        // Stop rigidbody
        var playerRb = player.GetComponent<Rigidbody>();
        if (playerRb)
        {
            playerRb.velocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }
        
        Debug.Log("Player frozen for wave transition");
    }
    
    public void UnfreezePlayer()
    {
        if (player == null) return;
        
        playerFrozen = false;
        
        // Re-enable movement components
        var playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement) playerMovement.enabled = true;
        
        // Re-enable input
        var gameInput = player.GetComponent<GameInput>();
        if (gameInput) gameInput.enabled = true;
        
        Debug.Log("Player unfrozen - wave started");
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        // Draw spawn position - red if player is frozen, green if spawned, yellow if ready
        if (playerFrozen)
            Gizmos.color = Color.red;
        else if (hasSpawned)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.yellow;
            
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Draw direction arrow
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 1f);
    }
    
    private void OnDrawGizmosSelected()
    {
        // More detailed visualization when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, Vector3.one * 0.3f);
        
        // Draw spawn area
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}