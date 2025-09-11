using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    public static PlayerSpawnManager Instance { get; private set; }
    
    [Header("Spawn Configuration")]
    [SerializeField] private Transform centerSpawnPoint;
    [SerializeField] private Vector3 centerPosition = Vector3.zero;
    [SerializeField] private bool useTransformPosition = true;
    
    [Header("Area Spawn Points")]
    [SerializeField] private Transform northSpawnPoint;
    [SerializeField] private Transform southSpawnPoint;
    [SerializeField] private Transform eastSpawnPoint;
    [SerializeField] private Transform westSpawnPoint;
    
    private Vector3 originalCenterPosition; // Store the original center position permanently
    
    [Header("Spawn Area Validation")]
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private LayerMask obstacleLayerMask = -1;
    
    [Header("Player References")]
    [SerializeField] private Player player;
    [SerializeField] private bool findPlayerAutomatically = true;
    
    private Vector3 validatedSpawnPosition;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Set center position and store it permanently
        if (useTransformPosition && centerSpawnPoint != null)
        {
            centerPosition = centerSpawnPoint.position;
            originalCenterPosition = centerSpawnPoint.position;
        }
        else if (useTransformPosition)
        {
            centerPosition = transform.position;
            originalCenterPosition = transform.position;
        }
        else
        {
            originalCenterPosition = centerPosition; // Use the manually set position
        }
        
        validatedSpawnPosition = originalCenterPosition;
        
        Debug.Log($"PlayerSpawnManager: Center position set to {originalCenterPosition}");
    }
    
    private void Start()
    {
        // Find player if needed
        if (findPlayerAutomatically && player == null)
        {
            player = FindObjectOfType<Player>();
        }
        
        // Subscribe to game events
        if (GameManager.Instance != null)
        {
        }
        
        // Subscribe to wave events
        var waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnWaveCompleted += OnWaveCompleted; // Teleport immediately when wave ends
            waveManager.OnWaveStarted += OnWaveStarted;
        }
        
        // Spawn player at center on game start
        SpawnPlayerAtCenter();
    }
    
    public void SpawnPlayerAtCenter()
    {
        SpawnPlayerAtArea(SpawnerArea.Center);
    }
    
    public void SpawnPlayerAtArea(SpawnerArea area)
    {
        if (player == null)
        {
            Debug.LogError("PlayerSpawnManager: No player assigned!");
            return;
        }
        
        Vector3 spawnPos = GetSpawnPositionForArea(area);
        Quaternion spawnRot = GetRotationForArea(area);
        
        StartCoroutine(TeleportPlayerSafely(spawnPos, spawnRot, area));
    }
    
    private System.Collections.IEnumerator TeleportPlayerSafely(Vector3 targetPosition, Quaternion targetRotation, SpawnerArea area)
    {
        // Get all components that might block teleportation
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        Collider playerCollider = player.GetComponent<Collider>();
        UnityEngine.AI.NavMeshAgent navAgent = player.GetComponent<UnityEngine.AI.NavMeshAgent>();
        
        // Store original states
        bool wasKinematic = playerRb != null && playerRb.isKinematic;
        bool hadCollider = playerCollider != null && playerCollider.enabled;
        bool hadNavAgent = navAgent != null && navAgent.enabled;
        
        // Disable all movement constraints
        if (playerRb) playerRb.isKinematic = true;
        if (playerCollider) playerCollider.enabled = false;
        if (navAgent) navAgent.enabled = false;
        
        Debug.Log($"Teleporting to {area}: Disabled NavAgent: {hadNavAgent}, Rigidbody: {!wasKinematic}, Collider: {hadCollider}");
        
        // Wait a frame to ensure components are disabled
        yield return null;
        
        // Teleport
        player.transform.position = targetPosition;
        player.transform.rotation = targetRotation;
        
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
                navAgent.Warp(targetPosition);
            }
        }
        
        Debug.Log($"Player safely teleported to {area} area: {targetPosition}");
    }
    
    private Vector3 GetSpawnPositionForArea(SpawnerArea area)
    {
        Transform spawnPoint = area switch
        {
            SpawnerArea.North => northSpawnPoint,
            SpawnerArea.South => southSpawnPoint,
            SpawnerArea.East => eastSpawnPoint,
            SpawnerArea.West => westSpawnPoint,
            _ => centerSpawnPoint
        };
        
        // Use exact spawn point position, no validation needed
        if (spawnPoint != null)
        {
            return spawnPoint.position;
        }
        
        // Fallback to center if spawn point not assigned
        Debug.LogWarning($"No spawn point assigned for {area}, using center");
        return originalCenterPosition;
    }
    
    private Quaternion GetRotationForArea(SpawnerArea area)
    {
        return area switch
        {
            SpawnerArea.North => Quaternion.LookRotation(Vector3.back),    // Face south towards center
            SpawnerArea.South => Quaternion.LookRotation(Vector3.forward), // Face north towards center
            SpawnerArea.East => Quaternion.LookRotation(Vector3.left),     // Face west towards center
            SpawnerArea.West => Quaternion.LookRotation(Vector3.right),    // Face east towards center
            _ => Quaternion.identity // Face north from center
        };
    }
    
    private Vector3 FindValidSpawnPosition(Vector3 targetPosition)
    {
        
        // Check if spawn position is clear
        if (IsPositionClear(targetPosition))
        {
            validatedSpawnPosition = targetPosition;
            return targetPosition;
        }
        
        // Try to find a clear position nearby
        for (int attempts = 0; attempts < 8; attempts++)
        {
            float angle = attempts * 45f * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * spawnRadius, 
                0, 
                Mathf.Sin(angle) * spawnRadius
            );
            
            Vector3 testPosition = originalCenterPosition + offset;
            
            if (IsPositionClear(testPosition))
            {
                validatedSpawnPosition = testPosition;
                return testPosition;
            }
        }
        
        // Fallback to original center position
        Debug.LogWarning("PlayerSpawnManager: Could not find clear spawn position, using center anyway");
        validatedSpawnPosition = originalCenterPosition;
        return originalCenterPosition;
    }
    
    private bool IsPositionClear(Vector3 position)
    {
        // Check for obstacles using a sphere cast
        return !Physics.CheckSphere(position + Vector3.up * 0.5f, 0.5f, obstacleLayerMask);
    }
    
    private void ValidatePlayerInCenter()
    {
        if (player == null) return;
        
        float distanceFromCenter = Vector3.Distance(player.transform.position, originalCenterPosition);
        
        if (distanceFromCenter > spawnRadius * 2f)
        {
            Debug.LogWarning($"Player is {distanceFromCenter:F1} units from center - may not be in center area");
        }
    }
    
    // Public methods for external control
    public void RespawnPlayerAtCenter()
    {
        SpawnPlayerAtCenter();
    }
    
    public void SetCenterPosition(Vector3 newCenterPosition)
    {
        centerPosition = newCenterPosition;
        originalCenterPosition = newCenterPosition;
        validatedSpawnPosition = originalCenterPosition;
    }
    
    public void SetCenterPosition(Transform newCenterTransform)
    {
        if (newCenterTransform != null)
        {
            centerSpawnPoint = newCenterTransform;
            centerPosition = newCenterTransform.position;
            originalCenterPosition = newCenterTransform.position;
            validatedSpawnPosition = originalCenterPosition;
        }
    }
    
    // Getters
    public Vector3 CenterPosition => originalCenterPosition;
    public Vector3 ValidatedSpawnPosition => validatedSpawnPosition;
    public bool IsPlayerInCenter()
    {
        if (player == null) return false;
        
        float distanceFromCenter = Vector3.Distance(player.transform.position, originalCenterPosition);
        return distanceFromCenter <= spawnRadius;
    }
    
    // Integration with other systems
    public void OnGameRestart()
    {
        SpawnPlayerAtCenter();
    }
    
    private void OnWaveCompleted(int waveNumber)
    {
        // Player teleportation now happens when pressing space to start next wave
        // No longer auto-teleporting at wave end
        Debug.Log($"Wave {waveNumber} completed - player will be teleported when starting next wave");
        // SpawnPlayerAtCenter(); // Commented out - teleportation moved to space press
    }
    
    private void OnWaveStarted(int waveNumber)
    {
        // Wave started - player should already be in center from wave completion
    }
    
    public void OnWaveStart(int waveNumber)
    {
        // Legacy method - kept for compatibility
        OnWaveStarted(waveNumber);
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        // Draw center position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(originalCenterPosition, 0.5f);
        
        // Draw spawn radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(originalCenterPosition, spawnRadius);
        
        // Draw validated spawn position if different
        if (validatedSpawnPosition != originalCenterPosition)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(validatedSpawnPosition, 0.3f);
            Gizmos.DrawLine(originalCenterPosition, validatedSpawnPosition);
        }
        
        // Draw connection lines to walls (if WallManager exists)
        if (Application.isPlaying && WallManager.Instance != null)
        {
            Gizmos.color = Color.red;
            var northWall = WallManager.Instance.GetWallByPosition(WallPosition.NorthWall);
            var eastWall = WallManager.Instance.GetWallByPosition(WallPosition.EastWall);
            var southWall = WallManager.Instance.GetWallByPosition(WallPosition.SouthWall);
            var westWall = WallManager.Instance.GetWallByPosition(WallPosition.WestWall);
            
            if (northWall != null) Gizmos.DrawLine(originalCenterPosition, northWall.transform.position);
            if (eastWall != null) Gizmos.DrawLine(originalCenterPosition, eastWall.transform.position);
            if (southWall != null) Gizmos.DrawLine(originalCenterPosition, southWall.transform.position);
            if (westWall != null) Gizmos.DrawLine(originalCenterPosition, westWall.transform.position);
        }
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from wave events
        var waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnWaveCompleted -= OnWaveCompleted;
            waveManager.OnWaveStarted -= OnWaveStarted;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // More detailed gizmos when selected
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(originalCenterPosition, Vector3.one * 0.2f);
        
        // Show spawn validation area
        Gizmos.color = Color.cyan;
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 pos = originalCenterPosition + new Vector3(Mathf.Cos(angle) * spawnRadius, 0, Mathf.Sin(angle) * spawnRadius);
            Gizmos.DrawWireSphere(pos, 0.2f);
        }
    }
}