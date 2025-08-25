using UnityEngine;
using UnityEngine.AI;

public enum WallPosition
{
    NorthWall,    // Separates Center from North
    EastWall,     // Separates Center from East  
    SouthWall,    // Separates Center from South
    WestWall      // Separates Center from West
}

public class Wall : MonoBehaviour
{
    [Header("Wall Configuration")]
    [SerializeField] private WallPosition wallPosition = WallPosition.NorthWall;
    [SerializeField] private SpawnerArea connectedSpawnArea = SpawnerArea.North;
    
    [Header("Wall Properties")]
    [SerializeField] private bool isDestructible = true;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool startsActive = true;
    
    [Header("Visual Components")]
    [SerializeField] private GameObject wallMesh;
    [SerializeField] private Collider wallCollider;
    
    [Header("Navigation Components")]
    [SerializeField] private NavMeshObstacle navMeshObstacle;
    
    private float currentHealth;
    private bool isActive;
    
    // Events
    public System.Action<Wall> OnWallDestroyed;
    public System.Action<Wall, bool> OnWallStateChanged;
    
    public WallPosition Position => wallPosition;
    public SpawnerArea ConnectedArea => connectedSpawnArea;
    public bool IsActive => isActive;
    public float HealthPercentage => currentHealth / maxHealth;
    
    private void Awake()
    {
        // Auto-assign connected area based on wall position if not set
        if (connectedSpawnArea == SpawnerArea.Center)
        {
            connectedSpawnArea = GetAreaFromWallPosition(wallPosition);
        }
        
        // Auto-find NavMeshObstacle if not assigned
        if (navMeshObstacle == null)
        {
            navMeshObstacle = GetComponent<NavMeshObstacle>();
        }
        
        currentHealth = maxHealth;
        SetWallActive(startsActive);
    }
    
    private void Start()
    {
        // Register with WallManager if it exists
        WallManager.Instance?.RegisterWall(this);
        
        Debug.Log($"Wall {wallPosition} initialized, connecting Center to {connectedSpawnArea}");
    }
    
    private SpawnerArea GetAreaFromWallPosition(WallPosition position)
    {
        switch (position)
        {
            case WallPosition.NorthWall: return SpawnerArea.North;
            case WallPosition.EastWall: return SpawnerArea.East;
            case WallPosition.SouthWall: return SpawnerArea.South;
            case WallPosition.WestWall: return SpawnerArea.West;
            default: return SpawnerArea.North;
        }
    }
    
    public void SetWallActive(bool active)
    {
        isActive = active;
        
        // Control visual and collision components
        if (wallMesh != null)
            wallMesh.SetActive(active);
        
        if (wallCollider != null)
            wallCollider.enabled = active;
        
        // Control NavMesh obstacle (critical for AI navigation!)
        if (navMeshObstacle != null)
        {
            navMeshObstacle.enabled = active;
            // Force NavMesh update for immediate AI path recalculation
            if (navMeshObstacle.carving)
            {
                navMeshObstacle.enabled = false; // Disable first
                navMeshObstacle.enabled = active; // Then set to desired state
            }
        }
        
        OnWallStateChanged?.Invoke(this, active);
        
        Debug.Log($"Wall {wallPosition} set to {(active ? "ACTIVE (blocking AI)" : "INACTIVE (AI can pass)")}");
    }
    
    public void TakeDamage(float damage)
    {
        if (!isActive || !isDestructible) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"Wall {wallPosition} took {damage} damage. Health: {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            DestroyWall();
        }
    }
    
    public void DestroyWall()
    {
        if (!isActive) return;
        
        Debug.Log($"Wall {wallPosition} destroyed! {connectedSpawnArea} area is now accessible!");
        
        SetWallActive(false);
        OnWallDestroyed?.Invoke(this);
        
        // Notify WallManager
        WallManager.Instance?.NotifyWallDestroyed(this);
    }
    
    public void RepairWall()
    {
        currentHealth = maxHealth;
        SetWallActive(true);
        
        Debug.Log($"Wall {wallPosition} repaired and reactivated");
    }
    
    // For wave-based wall control
    public void SetWallForWave(bool shouldBeActive)
    {
        SetWallActive(shouldBeActive);
    }
    
    private void OnDestroy()
    {
        // Unregister from WallManager
        WallManager.Instance?.UnregisterWall(this);
    }
    
    // Debug visualization
    private void OnDrawGizmos()
    {
        Gizmos.color = isActive ? Color.red : Color.gray;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        
        // Draw connection line to center
        Gizmos.color = Color.yellow;
        Vector3 centerPos = Vector3.zero; // Assume center is at origin
        Gizmos.DrawLine(transform.position, centerPos);
    }
}