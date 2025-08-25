using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour
{
    public static WallManager Instance { get; private set; }
    
    [Header("Wall Management")]
    [SerializeField] private List<Wall> walls = new List<Wall>();
    [SerializeField] private bool controlWallsWithWaves = true;
    [SerializeField] private bool openWallsForActiveSpawners = true;
    
    [Header("Wave Integration")]
    [SerializeField] private bool closeWallsBetweenWaves = false;
    [SerializeField] private float wallCloseDelay = 2f;
    
    // Wall organization by position
    private Dictionary<WallPosition, Wall> wallsByPosition = new Dictionary<WallPosition, Wall>();
    private Dictionary<SpawnerArea, Wall> wallsByArea = new Dictionary<SpawnerArea, Wall>();
    
    // Events
    public System.Action<Wall> OnWallDestroyed;
    public System.Action<SpawnerArea> OnAreaOpened;
    public System.Action<SpawnerArea> OnAreaClosed;
    
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
    }
    
    private void Start()
    {
        // Find all walls if not assigned
        if (walls.Count == 0)
        {
            walls.AddRange(FindObjectsOfType<Wall>());
            Debug.Log($"WallManager found {walls.Count} walls in scene");
        }
        
        OrganizeWalls();
        
        // Subscribe to WaveManager events if available
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted += OnWaveStarted;
            WaveManager.Instance.OnWaveCompleted += OnWaveCompleted;
            WaveManager.Instance.OnWaveStateChanged += OnWaveStateChanged;
            WaveManager.Instance.OnWaveAreasActivated += OnWaveAreasActivated; // NEW: Listen for active areas
            Debug.Log("WallManager connected to WaveManager");
        }
        
        // Initialize wall states
        InitializeWallStates();
    }
    
    private void OrganizeWalls()
    {
        wallsByPosition.Clear();
        wallsByArea.Clear();
        
        foreach (var wall in walls)
        {
            if (wall != null)
            {
                wallsByPosition[wall.Position] = wall;
                wallsByArea[wall.ConnectedArea] = wall;
                
                // Subscribe to wall events
                wall.OnWallDestroyed += HandleWallDestroyed;
                wall.OnWallStateChanged += OnWallStateChanged;
                
                Debug.Log($"Organized wall {wall.Position} connecting to {wall.ConnectedArea}");
            }
        }
    }
    
    private void InitializeWallStates()
    {
        if (controlWallsWithWaves)
        {
            // Start with all walls closed, will open based on wave configuration
            SetAllWalls(true);
        }
    }
    
    public void RegisterWall(Wall wall)
    {
        if (!walls.Contains(wall))
        {
            walls.Add(wall);
            wallsByPosition[wall.Position] = wall;
            wallsByArea[wall.ConnectedArea] = wall;
            
            wall.OnWallDestroyed += HandleWallDestroyed;
            wall.OnWallStateChanged += OnWallStateChanged;
            
            Debug.Log($"Registered wall {wall.Position}");
        }
    }
    
    public void UnregisterWall(Wall wall)
    {
        walls.Remove(wall);
        wallsByPosition.Remove(wall.Position);
        wallsByArea.Remove(wall.ConnectedArea);
        
        wall.OnWallDestroyed -= HandleWallDestroyed;
        wall.OnWallStateChanged -= OnWallStateChanged;
    }
    
    // Wave event handlers
    private void OnWaveStarted(int waveNumber)
    {
        if (!controlWallsWithWaves) return;
        
        Debug.Log($"WallManager: Wave {waveNumber} started");
    }
    
    private void OnWaveAreasActivated(SpawnerArea[] activeAreas)
    {
        if (!controlWallsWithWaves || !openWallsForActiveSpawners) return;
        
        Debug.Log($"WallManager: Configuring walls for active areas: {string.Join(", ", activeAreas)}");
        
        ConfigureWallsForAreas(activeAreas);
    }
    
    private void OnWaveCompleted(int waveNumber)
    {
        Debug.Log($"WallManager: Wave {waveNumber} completed");
        
        if (closeWallsBetweenWaves)
        {
            StartCoroutine(CloseWallsAfterDelay());
        }
    }
    
    private void OnWaveStateChanged(WaveState waveState)
    {
        switch (waveState)
        {
            case WaveState.Preparing:
                if (openWallsForActiveSpawners)
                    ConfigureWallsForActiveSpawners();
                break;
                
            case WaveState.Complete:
                // Optional: Close all walls when wave completes
                break;
        }
    }
    
    private void ConfigureWallsForAreas(SpawnerArea[] activeAreas)
    {
        // First, close all walls
        SetAllWalls(true);
        
        // Then open walls for areas that should spawn enemies
        Debug.Log($"Opening walls for areas: {string.Join(", ", activeAreas)}");
        
        foreach (var area in activeAreas)
        {
            if (area != SpawnerArea.Center) // Center doesn't need walls opened
            {
                SetWallForArea(area, false); // false = open wall
                Debug.Log($"Opened wall for {area}");
            }
        }
    }
    
    private void ConfigureWallsForActiveSpawners()
    {
        // Legacy method - now just calls the new method with fallback logic
        if (WaveManager.Instance == null) return;
        
        SpawnerArea[] activeAreas = GetActiveAreasForCurrentWave();
        ConfigureWallsForAreas(activeAreas);
    }
    
    private SpawnerArea[] GetActiveAreasForCurrentWave()
    {
        // Fallback logic - only used if new system fails
        if (WaveManager.Instance == null) return new SpawnerArea[0];
        
        int waveNumber = WaveManager.Instance.CurrentWaveNumber;
        
        switch (waveNumber % 6)
        {
            case 1: return new[] { SpawnerArea.North };
            case 2: return new[] { SpawnerArea.South };
            case 3: return new[] { SpawnerArea.North, SpawnerArea.South };
            case 4: return new[] { SpawnerArea.East, SpawnerArea.West };
            case 5: return new[] { SpawnerArea.North, SpawnerArea.East, SpawnerArea.South, SpawnerArea.West };
            case 0: return new[] { SpawnerArea.Center };
            default: return new[] { SpawnerArea.North, SpawnerArea.South };
        }
    }
    
    private System.Collections.IEnumerator CloseWallsAfterDelay()
    {
        yield return new WaitForSeconds(wallCloseDelay);
        SetAllWalls(true);
        Debug.Log("All walls closed after wave completion");
    }
    
    // Wall control methods
    public void SetWallForArea(SpawnerArea area, bool active)
    {
        if (wallsByArea.ContainsKey(area))
        {
            wallsByArea[area].SetWallActive(active);
            
            if (active)
                OnAreaClosed?.Invoke(area);
            else
                OnAreaOpened?.Invoke(area);
        }
    }
    
    public void SetWallByPosition(WallPosition position, bool active)
    {
        if (wallsByPosition.ContainsKey(position))
        {
            wallsByPosition[position].SetWallActive(active);
        }
    }
    
    public void SetAllWalls(bool active)
    {
        foreach (var wall in walls)
        {
            if (wall != null)
            {
                wall.SetWallActive(active);
            }
        }
        
        Debug.Log($"All walls set to {(active ? "CLOSED" : "OPEN")}");
    }
    
    public void OpenAllWalls()
    {
        SetAllWalls(false);
    }
    
    public void CloseAllWalls()
    {
        SetAllWalls(true);
    }
    
    // Wall state queries
    public bool IsAreaOpen(SpawnerArea area)
    {
        if (area == SpawnerArea.Center) return true; // Center is always accessible
        
        return wallsByArea.ContainsKey(area) && !wallsByArea[area].IsActive;
    }
    
    public bool IsWallDestroyed(WallPosition position)
    {
        return wallsByPosition.ContainsKey(position) && !wallsByPosition[position].IsActive;
    }
    
    public Wall GetWallForArea(SpawnerArea area)
    {
        wallsByArea.TryGetValue(area, out Wall wall);
        return wall;
    }
    
    public Wall GetWallByPosition(WallPosition position)
    {
        wallsByPosition.TryGetValue(position, out Wall wall);
        return wall;
    }
    
    // Public methods for Wall to call
    public void NotifyWallDestroyed(Wall wall)
    {
        Debug.Log($"WallManager: Wall {wall.Position} destroyed, {wall.ConnectedArea} area opened!");
        OnWallDestroyed?.Invoke(wall);
        OnAreaOpened?.Invoke(wall.ConnectedArea);
    }
    
    // Event handlers
    private void HandleWallDestroyed(Wall wall)
    {
        NotifyWallDestroyed(wall);
    }
    
    private void OnWallStateChanged(Wall wall, bool isActive)
    {
        if (isActive)
            OnAreaClosed?.Invoke(wall.ConnectedArea);
        else
            OnAreaOpened?.Invoke(wall.ConnectedArea);
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from WaveManager events
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted -= OnWaveStarted;
            WaveManager.Instance.OnWaveCompleted -= OnWaveCompleted;
            WaveManager.Instance.OnWaveStateChanged -= OnWaveStateChanged;
        }
    }
    
    // Debug methods
    [ContextMenu("Open All Walls")]
    private void DebugOpenAllWalls()
    {
        OpenAllWalls();
    }
    
    [ContextMenu("Close All Walls")]
    private void DebugCloseAllWalls()
    {
        CloseAllWalls();
    }
}