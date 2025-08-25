using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum WaveState
{
    Preparing,
    Active,
    Cleanup,
    Complete,
    Transition
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    
    [SerializeField] private int currentWaveNumber = 0;
    [SerializeField] private WaveState currentWaveState = WaveState.Preparing;
    [SerializeField] private List<Spawner> allSpawners = new List<Spawner>();
    
    [Header("Wave Progression")]
    [SerializeField] private bool useWaveProgression = true;
    [SerializeField] private WaveProgressionConfigSO waveProgressionConfig;
    [SerializeField] private List<WaveConfigSO> waveConfigs = new List<WaveConfigSO>();
    
    // Organize spawners by area for easy access
    private Dictionary<SpawnerArea, List<Spawner>> spawnersByArea = new Dictionary<SpawnerArea, List<Spawner>>();
    
    private int activeEnemyCount = 0;
    private int totalEnemiesSpawned = 0;
    private bool allSpawnersComplete = false;
    
    // Events
    public System.Action<int> OnWaveStarted;
    public System.Action<int> OnWaveCompleted;
    public System.Action<WaveState> OnWaveStateChanged;
    public System.Action<SpawnerArea[]> OnWaveAreasActivated; // NEW: Tell which areas are active
    
    public int CurrentWaveNumber => currentWaveNumber;
    public WaveState CurrentWaveState => currentWaveState;
    public int ActiveEnemyCount => activeEnemyCount;
    
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
        // Find all spawners in the scene if not assigned
        if (allSpawners.Count == 0)
        {
            allSpawners.AddRange(FindObjectsOfType<Spawner>());
            Debug.Log($"Found {allSpawners.Count} spawners in scene");
        }
        
        // Organize spawners by area
        OrganizeSpawnersByArea();
        
        // Subscribe to spawner events
        foreach (var spawner in allSpawners)
        {
            spawner.OnEnemySpawned += OnEnemySpawned;
            spawner.OnSpawningComplete += OnSpawnerComplete;
        }
        
        // Start first wave after a short delay
        StartCoroutine(StartFirstWaveDelayed());
    }
    
    private void OrganizeSpawnersByArea()
    {
        spawnersByArea.Clear();
        
        // Initialize all area lists
        foreach (SpawnerArea area in System.Enum.GetValues(typeof(SpawnerArea)))
        {
            spawnersByArea[area] = new List<Spawner>();
        }
        
        // Group spawners by their assigned area
        foreach (var spawner in allSpawners)
        {
            spawnersByArea[spawner.Area].Add(spawner);
            Debug.Log($"Assigned spawner {spawner.gameObject.name} to area {spawner.Area}");
        }
        
        // Log spawner distribution
        foreach (var kvp in spawnersByArea)
        {
            Debug.Log($"Area {kvp.Key}: {kvp.Value.Count} spawners");
        }
    }
    
    private IEnumerator StartFirstWaveDelayed()
    {
        yield return new WaitForSeconds(1f);
        StartWave(1);
    }
    
    public void StartWave(int waveNumber)
    {
        currentWaveNumber = waveNumber;
        activeEnemyCount = 0;
        totalEnemiesSpawned = 0;
        allSpawnersComplete = false;
        
        ChangeWaveState(WaveState.Preparing);
        
        Debug.Log($"Starting Wave {currentWaveNumber}");
        
        // Create basic wave config (will be replaced with ScriptableObject later)
        var waveConfig = CreateBasicWaveConfig(waveNumber);
        
        // Get areas for this wave and notify WallManager
        SpawnerArea[] areasToSpawn = GetAreasForWave(waveNumber);
        OnWaveAreasActivated?.Invoke(areasToSpawn);
        
        // Start spawners based on wave configuration
        StartSpawnersForWave(waveConfig);
        
        ChangeWaveState(WaveState.Active);
        OnWaveStarted?.Invoke(currentWaveNumber);
    }
    
    private void StartSpawnersForWave(WaveConfig waveConfig)
    {
        SpawnerArea[] areasToSpawn = GetAreasForWave(waveConfig.waveNumber);
        
        Debug.Log($"Wave {waveConfig.waveNumber}: Spawning from areas: {string.Join(", ", areasToSpawn)}");
        
        foreach (var area in areasToSpawn)
        {
            if (spawnersByArea.ContainsKey(area) && spawnersByArea[area].Count > 0)
            {
                // Get number of spawners to use for this area from progression config
                int spawnersToUse = GetSpawnersCountForArea(area, waveConfig.waveNumber);
                
                // Select closest spawners to center
                var selectedSpawners = SelectClosestSpawners(spawnersByArea[area], spawnersToUse);
                
                // Split enemy count across selected spawners
                int enemiesPerSpawner = selectedSpawners.Count > 0 ? 
                    Mathf.CeilToInt((float)waveConfig.enemyCount / selectedSpawners.Count) : waveConfig.enemyCount;
                
                Debug.Log($"Area {area}: Using {selectedSpawners.Count}/{spawnersByArea[area].Count} spawners, {enemiesPerSpawner} enemies each");
                
                // Create modified wave config for each spawner
                foreach (var spawner in selectedSpawners)
                {
                    var spawnerConfig = CreateSpawnerWaveConfig(waveConfig, enemiesPerSpawner);
                    spawner.StartWave(spawnerConfig);
                }
            }
            else
            {
                Debug.LogWarning($"No spawners available for area {area}");
            }
        }
    }
    
    private List<Spawner> SelectClosestSpawners(List<Spawner> availableSpawners, int count)
    {
        if (count <= 0) return new List<Spawner>();
        if (count >= availableSpawners.Count) return new List<Spawner>(availableSpawners);
        
        // Find center position (use PlayerSpawner if available, otherwise Vector3.zero)
        Vector3 centerPos = Vector3.zero;
        var playerSpawner = FindObjectOfType<PlayerSpawner>();
        if (playerSpawner != null)
        {
            centerPos = playerSpawner.SpawnPosition;
        }
        
        // Sort spawners by distance to center
        var sortedSpawners = availableSpawners
            .OrderBy(spawner => Vector3.Distance(spawner.transform.position, centerPos))
            .Take(count)
            .ToList();
            
        return sortedSpawners;
    }
    
    private int GetSpawnersCountForArea(SpawnerArea area, int waveNumber)
    {
        // Try to get from progression config
        if (useWaveProgression && waveProgressionConfig != null && WaveProgressionManager.Instance != null)
        {
            var progression = WaveProgressionManager.Instance.GetWaveProgression(waveNumber);
            
            if (progression.isBossWave)
            {
                return waveProgressionConfig.bossPhase.spawners;
            }
            else
            {
                switch (progression.phase)
                {
                    case WaveProgressionManager.WavePhase.Learning:
                        return waveProgressionConfig.learningPhase.spawners;
                    case WaveProgressionManager.WavePhase.Combination:
                        return waveProgressionConfig.combinationPhase.spawners;
                    case WaveProgressionManager.WavePhase.Inverted:
                        return waveProgressionConfig.invertedPhase.spawners;
                    case WaveProgressionManager.WavePhase.Final:
                        return waveProgressionConfig.finalPhase.spawners;
                    default:
                        return 1;
                }
            }
        }
        
        // Fallback to using 1 spawner
        return 1;
    }
    
    private WaveConfig CreateSpawnerWaveConfig(WaveConfig originalConfig, int enemyCount)
    {
        return new WaveConfig
        {
            waveNumber = originalConfig.waveNumber,
            enemyCount = enemyCount,
            spawnInterval = originalConfig.spawnInterval,
            enemyTypes = originalConfig.enemyTypes
        };
    }
    
    private SpawnerArea[] GetAreasForWave(int waveNumber)
    {
        if (useWaveProgression && WaveProgressionManager.Instance != null)
        {
            var progression = WaveProgressionManager.Instance.GetWaveProgression(waveNumber);
            Debug.Log($"Wave {waveNumber}: {progression.description}");
            return progression.areas;
        }
        
        // Fallback to old system if WaveProgressionManager is not available
        Debug.LogWarning("WaveProgressionManager not found - using old wave system");
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
    
    // Method to start spawning from specific areas (for manual control)
    public void StartSpawnersInAreas(SpawnerArea[] areas, WaveConfig waveConfig)
    {
        foreach (var area in areas)
        {
            if (spawnersByArea.ContainsKey(area))
            {
                foreach (var spawner in spawnersByArea[area])
                {
                    spawner.StartWave(waveConfig);
                }
            }
        }
    }
    
    // Method to stop spawning in specific areas
    public void StopSpawnersInAreas(SpawnerArea[] areas)
    {
        foreach (var area in areas)
        {
            if (spawnersByArea.ContainsKey(area))
            {
                foreach (var spawner in spawnersByArea[area])
                {
                    spawner.StopWave();
                }
            }
        }
    }
    
    private WaveConfig CreateBasicWaveConfig(int waveNumber)
    {
        // Try to use WaveProgressionConfigSO first
        if (useWaveProgression && waveProgressionConfig != null && WaveProgressionManager.Instance != null)
        {
            var progression = WaveProgressionManager.Instance.GetWaveProgression(waveNumber);
            var generatedConfig = waveProgressionConfig.GenerateWaveConfig(waveNumber, progression.phase, progression.areas, progression.isBossWave);
            
            if (generatedConfig != null)
            {
                Debug.Log($"Using WaveProgressionConfig for wave {waveNumber}: {generatedConfig.enemyCount} enemies");
                return generatedConfig;
            }
        }
        
        // Try to find a WaveConfigSO for this wave
        WaveConfigSO waveConfigSO = GetWaveConfigSO(waveNumber);
        
        if (waveConfigSO != null)
        {
            return CreateWaveConfigFromSO(waveConfigSO);
        }
        
        // Final fallback to basic wave scaling
        Debug.LogWarning($"Using fallback wave config for wave {waveNumber}");
        int enemyCount = GetScaledEnemyCount(waveNumber);
        float spawnInterval = GetScaledSpawnInterval(waveNumber);
        
        return new WaveConfig
        {
            waveNumber = waveNumber,
            enemyCount = enemyCount,
            spawnInterval = spawnInterval
        };
    }
    
    private WaveConfigSO GetWaveConfigSO(int waveNumber)
    {
        // First try to find exact wave number match
        var exactMatch = waveConfigs.Find(config => config.waveNumber == waveNumber);
        if (exactMatch != null) return exactMatch;
        
        // Then try to find by boss wave
        if (WaveProgressionManager.Instance != null)
        {
            var progression = WaveProgressionManager.Instance.GetWaveProgression(waveNumber);
            if (progression.isBossWave)
            {
                var bossConfig = waveConfigs.Find(config => config.isBossWave);
                if (bossConfig != null) return bossConfig;
            }
        }
        
        // Fallback to a general config
        return waveConfigs.Count > 0 ? waveConfigs[0] : null;
    }
    
    private WaveConfig CreateWaveConfigFromSO(WaveConfigSO so)
    {
        var config = new WaveConfig
        {
            waveNumber = so.waveNumber,
            enemyCount = so.GetTotalEnemyCount(),
            spawnInterval = so.GetRandomSpawnInterval(),
            enemyTypes = GetEnemyTypesFromSO(so)
        };
        
        Debug.Log($"Created wave config from SO: {so.waveName} - {config.enemyCount} enemies");
        return config;
    }
    
    private GameObject[] GetEnemyTypesFromSO(WaveConfigSO so)
    {
        List<GameObject> enemyTypes = new List<GameObject>();
        
        foreach (var enemyData in so.enemies)
        {
            if (enemyData.enemyPrefab != null)
            {
                enemyTypes.Add(enemyData.enemyPrefab);
            }
        }
        
        return enemyTypes.ToArray();
    }
    
    private int GetScaledEnemyCount(int waveNumber)
    {
        // Check if it's a boss wave for extra enemies
        if (WaveProgressionManager.Instance != null)
        {
            var progression = WaveProgressionManager.Instance.GetWaveProgression(waveNumber);
            if (progression.isBossWave)
            {
                return 5 + (waveNumber * 3); // Boss waves have more enemies
            }
            
            if (progression.phase == WaveProgressionManager.WavePhase.Final)
            {
                return 8 + (waveNumber * 2); // Final phase scales faster
            }
        }
        
        // Regular scaling
        return 3 + (waveNumber * 2);
    }
    
    private float GetScaledSpawnInterval(int waveNumber)
    {
        // Boss waves spawn faster
        if (WaveProgressionManager.Instance != null)
        {
            var progression = WaveProgressionManager.Instance.GetWaveProgression(waveNumber);
            if (progression.isBossWave)
            {
                return Mathf.Max(0.2f, 0.4f - (waveNumber * 0.02f));
            }
        }
        
        // Regular scaling
        return Mathf.Max(0.3f, 0.6f - (waveNumber * 0.05f));
    }
    
    public void OnEnemySpawned(Enemy enemy)
    {
        activeEnemyCount++;
        totalEnemiesSpawned++;
        
        // Subscribe to enemy death event
        StartCoroutine(SubscribeToEnemyDeath(enemy));
        
        Debug.Log($"Enemy spawned. Active: {activeEnemyCount}, Total spawned: {totalEnemiesSpawned}");
    }
    
    private IEnumerator SubscribeToEnemyDeath(Enemy enemy)
    {
        // Wait a frame to ensure enemy is fully initialized
        yield return null;
        
        // We'll subscribe to enemy destruction instead since death animation destroys the object
        StartCoroutine(WaitForEnemyDestruction(enemy));
    }
    
    private IEnumerator WaitForEnemyDestruction(Enemy enemy)
    {
        // Wait until the enemy GameObject is destroyed
        while (enemy != null)
        {
            yield return null;
        }
        
        OnEnemyDeath();
    }
    
    public void OnEnemyDeath()
    {
        activeEnemyCount = Mathf.Max(0, activeEnemyCount - 1);
        Debug.Log($"Enemy died. Active enemies: {activeEnemyCount}");
        
        CheckWaveCompletion();
    }
    
    public void OnSpawnerComplete()
    {
        // Check if all active spawners are complete
        bool allComplete = true;
        int activeSpawnersCount = 0;
        
        foreach (var spawner in allSpawners)
        {
            // Only check spawners that were actually started this wave (have target enemies > 0)
            if (spawner.GetRemainingEnemies() >= 0 && spawner.TargetEnemyCount > 0)
            {
                activeSpawnersCount++;
                if (!spawner.IsSpawningComplete())
                {
                    allComplete = false;
                    Debug.Log($"Spawner {spawner.name} still spawning: {spawner.GetRemainingEnemies()} enemies left");
                    break;
                }
            }
        }
        
        if (allComplete && activeSpawnersCount > 0)
        {
            allSpawnersComplete = true;
            Debug.Log($"All {activeSpawnersCount} active spawners completed spawning");
            CheckWaveCompletion();
        }
        else if (activeSpawnersCount == 0)
        {
            Debug.LogWarning("No active spawners found for this wave!");
        }
    }
    
    private void CheckWaveCompletion()
    {
        Debug.Log($"CheckWaveCompletion: State={currentWaveState}, SpawnersComplete={allSpawnersComplete}, ActiveEnemies={activeEnemyCount}");
        
        if ((currentWaveState == WaveState.Active || currentWaveState == WaveState.Cleanup) && allSpawnersComplete && activeEnemyCount == 0)
        {
            CompleteWave();
        }
        else if (allSpawnersComplete && activeEnemyCount > 0 && currentWaveState == WaveState.Active)
        {
            ChangeWaveState(WaveState.Cleanup);
            Debug.Log("Wave in cleanup phase - waiting for remaining enemies to die");
        }
    }
    
    private void CompleteWave()
    {
        ChangeWaveState(WaveState.Complete);
        Debug.Log($"Wave {currentWaveNumber} completed!");
        
        // Immediately close all doors when wave completes
        CloseAllDoorsForTransition();
        
        OnWaveCompleted?.Invoke(currentWaveNumber);
        
        // Start next wave after a delay
        StartCoroutine(StartNextWaveDelayed());
    }
    
    private void CloseAllDoorsForTransition()
    {
        if (WallManager.Instance != null)
        {
            WallManager.Instance.CloseAllWalls();
            Debug.Log("All doors closed for wave transition");
        }
    }
    
    private IEnumerator StartNextWaveDelayed()
    {
        ChangeWaveState(WaveState.Transition);
        
        // Wait between waves
        float delayBetweenWaves = 3f;
        Debug.Log($"Next wave starting in {delayBetweenWaves} seconds...");
        yield return new WaitForSeconds(delayBetweenWaves);
        
        StartWave(currentWaveNumber + 1);
    }
    
    private void ChangeWaveState(WaveState newState)
    {
        currentWaveState = newState;
        OnWaveStateChanged?.Invoke(currentWaveState);
        Debug.Log($"Wave state changed to: {currentWaveState}");
    }
    
    public int GetActiveEnemyCount()
    {
        // Fallback method using FindObjectsOfType if our counter gets out of sync
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        int foundCount = enemies.Length;
        
        if (foundCount != activeEnemyCount)
        {
            Debug.LogWarning($"Enemy count mismatch! Tracked: {activeEnemyCount}, Found: {foundCount}. Correcting...");
            activeEnemyCount = foundCount;
        }
        
        return activeEnemyCount;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from spawner events
        foreach (var spawner in allSpawners)
        {
            if (spawner != null)
            {
                spawner.OnEnemySpawned -= OnEnemySpawned;
                spawner.OnSpawningComplete -= OnSpawnerComplete;
            }
        }
    }
}

// Simple wave configuration class (will be expanded into ScriptableObject)
[System.Serializable]
public class WaveConfig
{
    public int waveNumber;
    public int enemyCount;
    public float spawnInterval;
    public GameObject[] enemyTypes; // Will be populated later
}