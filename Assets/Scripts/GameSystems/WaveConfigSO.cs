using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfig", menuName = "ZombieStorm/Wave Configuration")]
public class WaveConfigSO : ScriptableObject
{
    [Header("Wave Info")]
    public int waveNumber = 1;
    public string waveName = "Wave 1";
    
    [Header("Spawn Areas")]
    public SpawnerArea[] activeAreas = { SpawnerArea.North };
    
    [Header("Enemy Composition")]
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();
    
    [Header("Area-Specific Configuration")]
    public bool useAreaSpecificSettings = false;
    public List<AreaSpawnData> areaSpecificSpawns = new List<AreaSpawnData>();
    
    [Header("Spawn Parameters")]
    [Range(0.1f, 5f)]
    public float baseSpawnInterval = 0.6f;
    [Range(0f, 2f)]
    public float spawnIntervalVariation = 0.2f;
    
    [Header("Difficulty Modifiers")]
    [Range(0.5f, 3f)]
    public float healthMultiplier = 1f;
    [Range(0.5f, 3f)]
    public float damageMultiplier = 1f;
    [Range(0.5f, 2f)]
    public float speedMultiplier = 1f;
    
    [Header("Rewards")]
    public int baseMoneyReward = 100;
    public int waveCompletionBonus = 50;
    
    [Header("Special Wave Properties")]
    public bool isBossWave = false;
    public bool isHordeWave = false;
    public string specialWaveDescription = "";
    
    public int GetTotalEnemyCount()
    {
        int total = 0;
        foreach (var enemyData in enemies)
        {
            total += enemyData.count;
        }
        return total;
    }
    
    public GameObject GetRandomEnemyType()
    {
        if (enemies.Count == 0) return null;
        
        // Create a weighted list based on spawn weights
        List<GameObject> weightedEnemies = new List<GameObject>();
        
        foreach (var enemyData in enemies)
        {
            for (int i = 0; i < enemyData.spawnWeight; i++)
            {
                if (enemyData.enemyPrefab != null)
                {
                    weightedEnemies.Add(enemyData.enemyPrefab);
                }
            }
        }
        
        if (weightedEnemies.Count == 0) return null;
        
        return weightedEnemies[Random.Range(0, weightedEnemies.Count)];
    }
    
    public float GetRandomSpawnInterval()
    {
        float variation = Random.Range(-spawnIntervalVariation, spawnIntervalVariation);
        return Mathf.Max(0.1f, baseSpawnInterval + variation);
    }
    
    public bool IsAreaActive(SpawnerArea area)
    {
        return System.Array.Exists(activeAreas, a => a == area);
    }
    
    public List<EnemySpawnData> GetEnemiesForArea(SpawnerArea area)
    {
        if (useAreaSpecificSettings)
        {
            var areaData = areaSpecificSpawns.Find(a => a.area == area);
            if (areaData != null && areaData.enemies.Count > 0)
            {
                return areaData.enemies;
            }
        }
        
        // Fallback to general enemy list
        return enemies;
    }
    
    public int GetEnemyCountForArea(SpawnerArea area)
    {
        if (useAreaSpecificSettings)
        {
            var areaData = areaSpecificSpawns.Find(a => a.area == area);
            if (areaData != null)
            {
                return areaData.GetTotalEnemyCount();
            }
        }
        
        // Distribute general enemy count across active areas
        return Mathf.CeilToInt((float)GetTotalEnemyCount() / activeAreas.Length);
    }
}

[System.Serializable]
public class EnemySpawnData
{
    [Header("Enemy Type")]
    public GameObject enemyPrefab;
    public string enemyName = "";
    
    [Header("Spawn Configuration")]
    [Range(1, 50)]
    public int count = 1;
    [Range(1, 10)]
    public int spawnWeight = 1; // Higher weight = more likely to spawn
    
    [Header("Individual Modifiers")]
    [Range(0.1f, 5f)]
    public float healthModifier = 1f;
    [Range(0.1f, 5f)]
    public float damageModifier = 1f;
    [Range(0.1f, 3f)]
    public float speedModifier = 1f;
}

[System.Serializable]
public class AreaSpawnData
{
    [Header("Area Configuration")]
    public SpawnerArea area = SpawnerArea.North;
    public string areaDescription = "";
    
    [Header("Area-Specific Enemies")]
    public List<EnemySpawnData> enemies = new List<EnemySpawnData>();
    
    [Header("Area-Specific Parameters")]
    [Range(0.1f, 5f)]
    public float spawnIntervalModifier = 1f;
    [Range(0.1f, 3f)]
    public float difficultyModifier = 1f;
    
    public int GetTotalEnemyCount()
    {
        int total = 0;
        foreach (var enemyData in enemies)
        {
            total += enemyData.count;
        }
        return total;
    }
}