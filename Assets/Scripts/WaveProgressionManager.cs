using System.Collections.Generic;
using UnityEngine;

public class WaveProgressionManager : MonoBehaviour
{
    public static WaveProgressionManager Instance { get; private set; }
    
    [Header("Wave Progression Configuration")]
    [SerializeField] private bool useCustomProgression = true;
    [SerializeField] private WaveProgressionConfigSO progressionConfig;
    [SerializeField] private int finalPhaseStartWave = 18;
    
    [Header("Boss Wave Configuration")]
    [SerializeField] private List<int> bossWaves = new List<int> { 7, 12, 17 };
    [SerializeField] private bool isBossEvery5WavesAfterFinal = true;
    
    [Header("Debug Info")]
    [SerializeField] private WavePhase currentPhase;
    [SerializeField] private string currentWaveDescription;
    
    public WaveProgressionConfigSO ProgressionConfig => progressionConfig;
    
    public enum WavePhase
    {
        Learning,       // Waves 1-6: Center -> Single directions
        Combination,    // Waves 8-11: Multiple directions
        Inverted,       // Waves 13-16: Everything except one
        Final,          // Wave 18+: All directions endless
        Boss            // Boss waves: 7, 12, 17, then every 5
    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public WaveProgressionData GetWaveProgression(int waveNumber)
    {
        var progression = new WaveProgressionData();
        progression.waveNumber = waveNumber;
        
        // Use boss waves from config if available
        var configBossWaves = progressionConfig != null ? progressionConfig.bossWaveNumbers : bossWaves;
        
        // Check if this is a boss wave
        if (IsBossWave(waveNumber, configBossWaves))
        {
            progression.phase = WavePhase.Boss;
            progression.areas = GetBossWaveAreas(waveNumber);
            progression.isBossWave = true;
            progression.description = GetBossWaveDescription(waveNumber);
        }
        else
        {
            // Regular wave progression
            progression = GetRegularWaveProgression(waveNumber);
        }
        
        currentPhase = progression.phase;
        currentWaveDescription = progression.description;
        
        return progression;
    }
    
    public WaveConfig GenerateWaveConfig(int waveNumber)
    {
        if (progressionConfig != null)
        {
            var progression = GetWaveProgression(waveNumber);
            return progressionConfig.GenerateWaveConfig(waveNumber, progression.phase, progression.areas, progression.isBossWave);
        }
        
        // Fallback to basic generation
        return null;
    }
    
    private WaveProgressionData GetRegularWaveProgression(int waveNumber)
    {
        var progression = new WaveProgressionData();
        progression.waveNumber = waveNumber;
        progression.isBossWave = false;
        
        if (waveNumber <= 2)
        {
            // Phase 1: Learning - Center waves
            progression.phase = WavePhase.Learning;
            progression.areas = new SpawnerArea[] { SpawnerArea.Center };
            progression.description = $"Learning Phase - Center spawn";
        }
        else if (waveNumber <= 6)
        {
            // Phase 1: Learning - Single directions
            progression.phase = WavePhase.Learning;
            progression.areas = GetLearningPhaseAreas(waveNumber);
            progression.description = $"Learning Phase - {string.Join(", ", progression.areas)}";
        }
        else if (waveNumber >= 8 && waveNumber <= 11)
        {
            // Phase 2: Combination
            progression.phase = WavePhase.Combination;
            progression.areas = GetCombinationPhaseAreas(waveNumber);
            progression.description = $"Combination Phase - {string.Join(", ", progression.areas)}";
        }
        else if (waveNumber >= 13 && waveNumber <= 16)
        {
            // Phase 3: Inverted
            progression.phase = WavePhase.Inverted;
            progression.areas = GetInvertedPhaseAreas(waveNumber);
            progression.description = $"Inverted Phase - All except one direction";
        }
        else if (waveNumber >= finalPhaseStartWave)
        {
            // Phase 4: Final
            progression.phase = WavePhase.Final;
            progression.areas = new SpawnerArea[] { SpawnerArea.North, SpawnerArea.East, SpawnerArea.South, SpawnerArea.West };
            progression.description = $"Final Phase - All directions (Wave {waveNumber})";
        }
        else
        {
            // Fallback for waves between phases
            progression.phase = WavePhase.Learning;
            progression.areas = new SpawnerArea[] { SpawnerArea.North };
            progression.description = $"Transition wave";
        }
        
        return progression;
    }
    
    private SpawnerArea[] GetLearningPhaseAreas(int waveNumber)
    {
        switch (waveNumber)
        {
            case 3: return new SpawnerArea[] { SpawnerArea.North };
            case 4: return new SpawnerArea[] { SpawnerArea.West };
            case 5: return new SpawnerArea[] { SpawnerArea.South };
            case 6: return new SpawnerArea[] { SpawnerArea.East };
            default: return new SpawnerArea[] { SpawnerArea.North };
        }
    }
    
    private SpawnerArea[] GetCombinationPhaseAreas(int waveNumber)
    {
        switch (waveNumber)
        {
            case 8: return new SpawnerArea[] { SpawnerArea.North, SpawnerArea.West };
            case 9: return new SpawnerArea[] { SpawnerArea.South, SpawnerArea.East };
            case 10: return new SpawnerArea[] { SpawnerArea.North, SpawnerArea.South };
            case 11: return new SpawnerArea[] { SpawnerArea.West, SpawnerArea.East };
            default: return new SpawnerArea[] { SpawnerArea.North, SpawnerArea.South };
        }
    }
    
    private SpawnerArea[] GetInvertedPhaseAreas(int waveNumber)
    {
        switch (waveNumber)
        {
            case 13: return new SpawnerArea[] { SpawnerArea.West, SpawnerArea.South, SpawnerArea.East }; // All except North
            case 14: return new SpawnerArea[] { SpawnerArea.North, SpawnerArea.South, SpawnerArea.East }; // All except West
            case 15: return new SpawnerArea[] { SpawnerArea.North, SpawnerArea.West, SpawnerArea.East }; // All except South
            case 16: return new SpawnerArea[] { SpawnerArea.North, SpawnerArea.West, SpawnerArea.South }; // All except East
            default: return new SpawnerArea[] { SpawnerArea.North, SpawnerArea.East, SpawnerArea.South, SpawnerArea.West };
        }
    }
    
    private bool IsBossWave(int waveNumber, List<int> bossWaveList = null)
    {
        var wavesToCheck = bossWaveList ?? bossWaves;
        
        // Check predefined boss waves
        if (wavesToCheck.Contains(waveNumber))
            return true;
            
        // Check if it's a boss wave in final phase
        var finalBossInterval = progressionConfig?.finalPhaseBossInterval ?? 5;
        if (isBossEvery5WavesAfterFinal && waveNumber >= finalPhaseStartWave)
        {
            return (waveNumber - finalPhaseStartWave) % finalBossInterval == (finalBossInterval - 1);
        }
        
        return false;
    }
    
    private SpawnerArea[] GetBossWaveAreas(int waveNumber)
    {
        if (waveNumber == 7 || waveNumber == 17)
        {
            // First and third boss: Center only
            return new SpawnerArea[] { SpawnerArea.Center };
        }
        else
        {
            // Second boss and final phase bosses: All directions
            return new SpawnerArea[] { SpawnerArea.North, SpawnerArea.East, SpawnerArea.South, SpawnerArea.West };
        }
    }
    
    private string GetBossWaveDescription(int waveNumber)
    {
        if (waveNumber == 7)
            return "🏆 FIRST BOSS - Center Arena";
        else if (waveNumber == 12)
            return "🏆 SECOND BOSS - All Directions";
        else if (waveNumber == 17)
            return "🏆 THIRD BOSS - Final Center Fight";
        else
            return "🏆 BOSS WAVE - Survive the Onslaught";
    }
    
    public WavePhase GetCurrentPhase()
    {
        return currentPhase;
    }
    
    public string GetCurrentWaveDescription()
    {
        return currentWaveDescription;
    }
    
    public bool IsInFinalPhase(int waveNumber)
    {
        return waveNumber >= finalPhaseStartWave;
    }
}

[System.Serializable]
public class WaveProgressionData
{
    public int waveNumber;
    public WaveProgressionManager.WavePhase phase;
    public SpawnerArea[] areas;
    public bool isBossWave;
    public string description;
    
    public bool IsAreaActive(SpawnerArea area)
    {
        return System.Array.Exists(areas, a => a == area);
    }
}