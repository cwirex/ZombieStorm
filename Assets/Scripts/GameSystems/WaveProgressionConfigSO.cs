using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveProgressionConfig", menuName = "ZombieStorm/Wave Progression Configuration")]
public class WaveProgressionConfigSO : ScriptableObject
{
    [Header("🎛️ DIFFICULTY MODIFIERS - 4 Main Controls")]
    [Space(10)]
    [Range(0.3f, 3f)] public float difficultyCountModifier = 1.2f;
    [Range(0.3f, 3f)] public float difficultySpeedModifier = 1.1f;
    [Range(0.5f, 5f)] public float difficultyBossModifier = 1.8f;
    [Range(1.0f, 1.5f)] public float finalPhaseScalingModifier = 1.1f;
    
    [Header("📊 BASIC SETUP - 2 Parameters")]
    [Space(10)]
    
    [Header("Spawn Speed (seconds between spawns)")]
    [Range(0.3f, 2f)] public float learningPhaseSpeed = 1.0f;
    [Range(0.1f, 1f)] public float advancedPhaseSpeed = 0.5f;
    
    [Header("🎯 Global Zombie Prefabs & Unlock Waves")]
    [Space(5)]
    public GameObject normalZombiePrefab;
    [Range(1, 30)] public int normalZombieUnlockWave = 1;
    
    public GameObject toxicZombiePrefab;
    [Range(1, 30)] public int toxicZombieUnlockWave = 3;
    
    public GameObject suiciderZombiePrefab;
    [Range(1, 30)] public int suiciderZombieUnlockWave = 8;
    
    public GameObject phoenixZombiePrefab;
    [Range(1, 30)] public int phoenixZombieUnlockWave = 13;
    
    public GameObject gigantZombiePrefab;
    [Range(1, 30)] public int gigantZombieUnlockWave = 16;
    
    public GameObject gigantBomberZombiePrefab;
    [Range(1, 30)] public int gigantBomberZombieUnlockWave = 20;
    
    [Header("🧟 Phase Configuration")]
    [Space(10)]
    
    [Header("📚 Learning Phase (Waves 1-6: Center → Single directions)")]
    public PhaseConfig learningPhase;
    
    [Header("⚡ Combination Phase (Waves 8-11: Multiple directions)")]
    public PhaseConfig combinationPhase;
    
    [Header("🔄 Inverted Phase (Waves 13-16: All except one direction)")]
    public PhaseConfig invertedPhase;
    
    [Header("💀 Final Phase (Wave 18+: All directions endless)")]
    public PhaseConfig finalPhase;
    
    [Header("🏆 Boss Phase (Waves 7, 12, 17: Special boss fights)")]
    public PhaseConfig bossPhase;
    
    
    [Header("⚙️ DETAILED CONFIGURATION")]
    [Space(10)]
    public List<int> bossWaveNumbers = new List<int> { 7, 12, 17 };
    public int finalPhaseBossInterval = 5;
    
    public WaveConfig GenerateWaveConfig(int waveNumber, WaveProgressionManager.WavePhase phase, SpawnerArea[] areas, bool isBossWave)
    {
        var config = new WaveConfig
        {
            waveNumber = waveNumber,
            spawnInterval = GetSpawnIntervalForPhase(phase, waveNumber, isBossWave),
            enemyCount = GetEnemyCountForPhase(phase, waveNumber, isBossWave),
            enemyTypes = GetEnemyTypesForPhase(phase, waveNumber, isBossWave)
        };
        
        return config;
    }
    
    private GameObject[] GetEnemyTypesForPhase(WaveProgressionManager.WavePhase phase, int waveNumber, bool isBossWave)
    {
        List<GameObject> enemyTypes = new List<GameObject>();
        
        // Get the appropriate phase config
        PhaseConfig phaseConfig = null;
        
        if (isBossWave)
        {
            phaseConfig = bossPhase;
        }
        else
        {
            switch (phase)
            {
                case WaveProgressionManager.WavePhase.Learning:
                    phaseConfig = learningPhase;
                    break;
                case WaveProgressionManager.WavePhase.Combination:
                    phaseConfig = combinationPhase;
                    break;
                case WaveProgressionManager.WavePhase.Inverted:
                    phaseConfig = invertedPhase;
                    break;
                case WaveProgressionManager.WavePhase.Final:
                    phaseConfig = finalPhase;
                    break;
            }
        }
        
        if (phaseConfig != null)
        {
            enemyTypes.AddRange(phaseConfig.GetActiveEnemyPrefabs(this, waveNumber));
        }
        
        return enemyTypes.ToArray();
    }
    
    private int GetEnemyCountForPhase(WaveProgressionManager.WavePhase phase, int waveNumber, bool isBossWave)
    {
        int baseCount = 0;
        
        // Get the appropriate phase config
        PhaseConfig phaseConfig = null;
        
        if (isBossWave)
        {
            // Boss waves use their own fixed count (not incremental)
            phaseConfig = bossPhase;
            baseCount = phaseConfig != null ? phaseConfig.enemyCountInc * 3 : 15; // Boss = 3x increment
        }
        else
        {
            // Calculate cumulative enemy count across all previous waves
            int totalEnemies = 0;
            
            // Add enemies from Learning Phase (Waves 1-6)
            if (waveNumber >= 1)
            {
                int learningWaves = Mathf.Min(waveNumber, 6);
                for (int i = 1; i <= learningWaves; i++)
                {
                    totalEnemies += learningPhase.enemyCountInc;
                }
            }
            
            // Add enemies from Combination Phase (Waves 8-11, skip wave 7 boss)
            if (waveNumber >= 8)
            {
                int combinationStart = 8;
                int combinationEnd = Mathf.Min(waveNumber, 11);
                for (int i = combinationStart; i <= combinationEnd; i++)
                {
                    totalEnemies += combinationPhase.enemyCountInc;
                }
            }
            
            // Add enemies from Inverted Phase (Waves 13-16, skip wave 12 boss)  
            if (waveNumber >= 13)
            {
                int invertedStart = 13;
                int invertedEnd = Mathf.Min(waveNumber, 16);
                for (int i = invertedStart; i <= invertedEnd; i++)
                {
                    totalEnemies += invertedPhase.enemyCountInc;
                }
            }
            
            // Add enemies from Final Phase (Wave 18+, skip wave 17 boss)
            if (waveNumber >= 18)
            {
                int finalWaves = waveNumber - 17; // How many final phase waves
                totalEnemies += finalWaves * finalPhase.enemyCountInc;
            }
            
            baseCount = totalEnemies;
        }
        
        // Apply difficulty modifiers
        float finalModifier = difficultyCountModifier;
        
        // Apply exponential scaling for final phase
        if (phase == WaveProgressionManager.WavePhase.Final && waveNumber >= 18)
        {
            int wavesIntoFinalPhase = waveNumber - 18;
            float exponentialScaling = Mathf.Pow(finalPhaseScalingModifier, wavesIntoFinalPhase);
            finalModifier *= exponentialScaling;
            Debug.Log($"Final Phase Wave {waveNumber}: Exponential scaling = {exponentialScaling:F2}x (base modifier = {finalPhaseScalingModifier})");
        }
        
        if (isBossWave)
        {
            finalModifier *= difficultyBossModifier; // Boss waves get extra modifier
            
            // Boss waves in final phase also get exponential scaling
            if (phase == WaveProgressionManager.WavePhase.Final && waveNumber >= 18)
            {
                int wavesIntoFinalPhase = waveNumber - 18;
                float bossExponentialScaling = Mathf.Pow(finalPhaseScalingModifier, wavesIntoFinalPhase * 0.5f); // Half rate for bosses
                finalModifier *= bossExponentialScaling;
            }
        }
        
        return Mathf.RoundToInt(baseCount * finalModifier);
    }
    
    private float GetSpawnIntervalForPhase(WaveProgressionManager.WavePhase phase, int waveNumber, bool isBossWave)
    {
        float baseInterval;
        
        if (phase == WaveProgressionManager.WavePhase.Learning)
        {
            baseInterval = learningPhaseSpeed - (waveNumber * 0.02f);
        }
        else
        {
            // All advanced phases use the same speed
            baseInterval = advancedPhaseSpeed - (waveNumber * 0.03f);
        }
        
        // Apply difficulty modifiers
        float speedModifier = difficultySpeedModifier;
        
        // Apply exponential scaling for final phase speed
        if (phase == WaveProgressionManager.WavePhase.Final && waveNumber >= 18)
        {
            int wavesIntoFinalPhase = waveNumber - 18;
            float exponentialSpeedScaling = Mathf.Pow(finalPhaseScalingModifier, wavesIntoFinalPhase * 0.7f); // 70% of full scaling for speed
            speedModifier *= exponentialSpeedScaling;
            Debug.Log($"Final Phase Wave {waveNumber}: Speed scaling = {exponentialSpeedScaling:F2}x");
        }
        
        if (isBossWave)
        {
            speedModifier *= difficultyBossModifier; // Boss waves get extra speed
        }
        
        // Lower interval = faster spawning, so divide by modifier
        float finalInterval = baseInterval / speedModifier;
        
        return Mathf.Max(0.05f, finalInterval); // Minimum 0.05s for ultra-fast spawning
    }
}

[System.Serializable]
public class PhaseConfig
{
    [Header("📊 Phase Settings")]
    [Range(1, 20)] public int enemyCountInc = 4;
    [Range(1, 3)] public int spawners = 1;
    
    [Header("🧟 The 6 Zombie Types (0 = disabled, higher = more likely)")]
    [Space(5)]
    
    [Range(0, 10)] public int normalZombieAmount = 10;
    [Range(0, 10)] public int toxicZombieAmount = 0;
    [Range(0, 10)] public int suiciderZombieAmount = 0;
    [Range(0, 10)] public int phoenixZombieAmount = 0;
    [Range(0, 10)] public int gigantZombieAmount = 0;
    [Range(0, 10)] public int gigantBomberZombieAmount = 0;
    
    public List<GameObject> GetActiveEnemyPrefabs(WaveProgressionConfigSO config, int currentWave)
    {
        List<GameObject> activeEnemies = new List<GameObject>();
        
        // Add enemies based on their amounts using global prefabs, but only if unlocked
        if (currentWave >= config.normalZombieUnlockWave)
            AddEnemyByAmount(activeEnemies, config.normalZombiePrefab, normalZombieAmount);
            
        if (currentWave >= config.toxicZombieUnlockWave)
            AddEnemyByAmount(activeEnemies, config.toxicZombiePrefab, toxicZombieAmount);
            
        if (currentWave >= config.suiciderZombieUnlockWave)
            AddEnemyByAmount(activeEnemies, config.suiciderZombiePrefab, suiciderZombieAmount);
            
        if (currentWave >= config.phoenixZombieUnlockWave)
            AddEnemyByAmount(activeEnemies, config.phoenixZombiePrefab, phoenixZombieAmount);
            
        if (currentWave >= config.gigantZombieUnlockWave)
            AddEnemyByAmount(activeEnemies, config.gigantZombiePrefab, gigantZombieAmount);
            
        if (currentWave >= config.gigantBomberZombieUnlockWave)
            AddEnemyByAmount(activeEnemies, config.gigantBomberZombiePrefab, gigantBomberZombieAmount);
        
        return activeEnemies;
    }
    
    private void AddEnemyByAmount(List<GameObject> list, GameObject prefab, int amount)
    {
        if (prefab != null && amount > 0)
        {
            for (int i = 0; i < amount; i++)
            {
                list.Add(prefab);
            }
        }
    }
    
    public bool HasAnyEnemies()
    {
        return normalZombieAmount > 0 || toxicZombieAmount > 0 || suiciderZombieAmount > 0 || 
               phoenixZombieAmount > 0 || gigantZombieAmount > 0 || gigantBomberZombieAmount > 0;
    }
}

