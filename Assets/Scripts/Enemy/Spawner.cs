using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnerArea
{
    North,
    East, 
    South,
    West,
    Center
}

public class Spawner : MonoBehaviour
{
    [Header("Spawner Area")]
    [SerializeField] private SpawnerArea spawnerArea = SpawnerArea.North;
    
    [Header("Default Settings (for fallback)")]
    [SerializeField] private int defaultSpawnCount = 3;
    [SerializeField] private float defaultSpawnInterval = 0.6f;
    [SerializeField] private GameObject defaultEnemyPrefab;
    
    [Header("Debug Info")]
    [SerializeField] private bool isSpawning = false;
    [SerializeField] private int currentWaveEnemiesSpawned = 0;
    [SerializeField] private int currentWaveTargetCount = 0;
    
    private WaveConfig currentWaveConfig;
    private Coroutine spawningCoroutine;
    
    // Events for WaveManager
    public System.Action<Enemy> OnEnemySpawned;
    public System.Action OnSpawningComplete;
    
    public SpawnerArea Area => spawnerArea;
    public int TargetEnemyCount => currentWaveTargetCount;
    
    private void Start() {
        // No longer auto-start spawning - wait for WaveManager to trigger
        Debug.Log($"Spawner {gameObject.name} ({spawnerArea}) ready and waiting for wave start");
    }
    
    public void StartWave(WaveConfig waveConfig)
    {
        // Stop any existing spawning
        StopWave();
        
        currentWaveConfig = waveConfig ?? CreateFallbackWaveConfig();
        currentWaveEnemiesSpawned = 0;
        currentWaveTargetCount = currentWaveConfig.enemyCount;
        
        Debug.Log($"Spawner {gameObject.name} starting wave with {currentWaveTargetCount} enemies");
        
        isSpawning = true;
        spawningCoroutine = StartCoroutine(SpawnEnemiesCoroutine());
    }
    
    public void StopWave()
    {
        if (spawningCoroutine != null)
        {
            StopCoroutine(spawningCoroutine);
            spawningCoroutine = null;
        }
        
        isSpawning = false;
        Debug.Log($"Spawner {gameObject.name} stopped spawning");
    }
    
    public bool IsSpawningComplete()
    {
        return !isSpawning && currentWaveEnemiesSpawned >= currentWaveTargetCount;
    }
    
    public int GetRemainingEnemies()
    {
        return Mathf.Max(0, currentWaveTargetCount - currentWaveEnemiesSpawned);
    }
    
    private WaveConfig CreateFallbackWaveConfig()
    {
        return new WaveConfig
        {
            enemyCount = defaultSpawnCount,
            spawnInterval = defaultSpawnInterval,
            enemyTypes = defaultEnemyPrefab != null ? new GameObject[] { defaultEnemyPrefab } : new GameObject[0]
        };
    }
    
    private IEnumerator SpawnEnemiesCoroutine()
    {
        // Calculate the initial delay based on the index of this spawner
        float initialDelay = (currentWaveConfig?.spawnInterval ?? defaultSpawnInterval) * transform.GetSiblingIndex();
        
        // Wait for the initial delay before starting the spawning
        yield return new WaitForSeconds(initialDelay);
        
        while (currentWaveEnemiesSpawned < currentWaveTargetCount && isSpawning)
        {
            GameObject enemyToSpawn = GetEnemyPrefabToSpawn();
            
            if (enemyToSpawn != null)
            {
                GameObject spawnedEnemyGO = Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
                Enemy spawnedEnemy = spawnedEnemyGO.GetComponent<Enemy>();
                
                if (spawnedEnemy != null)
                {
                    OnEnemySpawned?.Invoke(spawnedEnemy);
                    Debug.Log($"Spawner {gameObject.name} spawned {enemyToSpawn.name}");
                }
                else
                {
                    Debug.LogError($"Spawned enemy {enemyToSpawn.name} doesn't have Enemy component!");
                }
                
                currentWaveEnemiesSpawned++;
            }
            else
            {
                Debug.LogWarning($"Spawner {gameObject.name} has no enemy prefab to spawn!");
                break;
            }
            
            yield return new WaitForSeconds(currentWaveConfig?.spawnInterval ?? defaultSpawnInterval);
        }
        
        // Spawning complete
        isSpawning = false;
        Debug.Log($"Spawner {gameObject.name} completed spawning {currentWaveEnemiesSpawned} enemies");
        OnSpawningComplete?.Invoke();
    }
    
    private GameObject GetEnemyPrefabToSpawn()
    {
        // Try to get enemy from current wave config
        if (currentWaveConfig?.enemyTypes != null && currentWaveConfig.enemyTypes.Length > 0)
        {
            return currentWaveConfig.enemyTypes[Random.Range(0, currentWaveConfig.enemyTypes.Length)];
        }
        
        // Fallback to default enemy prefab
        return defaultEnemyPrefab;
    }
}
