using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreEntry
{
    public int score;
    public string date;
    
    public ScoreEntry(int score, string date)
    {
        this.score = score;
        this.date = date;
    }
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    [SerializeField] private int currentScore = 0;
    private int previousScore = 0;
    private const int MAX_LEADERBOARD_ENTRIES = 10;
    private const string LEADERBOARD_KEY = "ZombieStorm_Leaderboard";
    
    public int CurrentScore => currentScore;
    
    public int GetPreviousScore() => previousScore;
    
    // Score values for different enemy types
    private readonly Dictionary<System.Type, int> scoreValues = new()
    {
        { typeof(NormalZombie), 10 },
        { typeof(GigantZombie), 25 },
        { typeof(GigantBomberZombie), 35 },
        { typeof(Toxic), 20 },
        { typeof(PhoenixZombie), 40 },
        { typeof(SuiciderZombie), 15 }
    };
    
    // Events
    public event System.Action<int> OnScoreChanged;
    public event System.Action<int> OnGameFinished; // Fired when game ends with final score
    
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
        ResetScore();
        
        // Subscribe to wave events for bonuses
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveCompleted += OnWaveCompleted;
        }
    }
    
    private void OnDestroy()
    {
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveCompleted -= OnWaveCompleted;
        }
    }
    
    public void ResetScore()
    {
        currentScore = 0;
        previousScore = 0;
        OnScoreChanged?.Invoke(currentScore);
    }
    
    public void AddScore(Enemy enemy)
    {
        previousScore = currentScore;
        System.Type enemyType = enemy.GetType();
        
        if (scoreValues.TryGetValue(enemyType, out int points))
        {
            currentScore += points;
            OnScoreChanged?.Invoke(currentScore);
            Debug.Log($"Score added: +{points} for {enemyType.Name}. Total: {currentScore}");
        }
        else
        {
            // Fallback for unknown enemy types
            int defaultPoints = 10;
            currentScore += defaultPoints;
            OnScoreChanged?.Invoke(currentScore);
            Debug.LogWarning($"Unknown enemy type {enemyType.Name}, awarded default {defaultPoints} points");
        }
    }
    
    private void OnWaveCompleted(int waveNumber)
    {
        // Calculate wave completion bonus: same as cash (100 * waveNumber + 500 * bossLevel)
        bool isBoss = IsBossWave(waveNumber);
        int bossLevel = isBoss ? GetBossLevel(waveNumber) : 0;
        
        int waveBonus = 100 * waveNumber;
        int bossBonus = 500 * bossLevel;
        int totalBonus = waveBonus + bossBonus;
        
        previousScore = currentScore;
        currentScore += totalBonus;
        
        Debug.Log($"Wave {waveNumber} score bonus: {totalBonus} (Wave: {waveBonus}, Boss: {bossBonus})");
        OnScoreChanged?.Invoke(currentScore);
    }
    
    private bool IsBossWave(int waveNumber)
    {
        if (WaveProgressionManager.Instance != null)
        {
            var progression = WaveProgressionManager.Instance.GetWaveProgression(waveNumber);
            return progression.isBossWave;
        }
        return false;
    }
    
    private int GetBossLevel(int waveNumber)
    {
        // Same logic as CurrencyManager
        if (waveNumber == 7) return 1;      // First boss
        if (waveNumber == 12) return 2;     // Second boss  
        if (waveNumber == 17) return 3;     // Third boss
        
        // Final phase bosses scale with wave number
        if (waveNumber >= 18)
        {
            return 3 + ((waveNumber - 18) / 5) + 1;
        }
        
        return 1; // Default boss level
    }
    
    public void GameFinished()
    {
        OnGameFinished?.Invoke(currentScore);
        SaveScoreToLeaderboard(currentScore);
    }
    
    private void SaveScoreToLeaderboard(int score)
    {
        List<ScoreEntry> leaderboard = LoadLeaderboard();
        
        // Add new score with current date/time
        string currentDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        leaderboard.Add(new ScoreEntry(score, currentDate));
        
        // Sort by score (highest first)
        leaderboard.Sort((a, b) => b.score.CompareTo(a.score));
        
        // Keep only top entries
        if (leaderboard.Count > MAX_LEADERBOARD_ENTRIES)
        {
            leaderboard = leaderboard.GetRange(0, MAX_LEADERBOARD_ENTRIES);
        }
        
        // Save to PlayerPrefs
        string json = JsonUtility.ToJson(new SerializableList<ScoreEntry>(leaderboard));
        PlayerPrefs.SetString(LEADERBOARD_KEY, json);
        PlayerPrefs.Save();
        
        Debug.Log($"Score {score} saved to leaderboard");
    }
    
    public List<ScoreEntry> LoadLeaderboard()
    {
        string json = PlayerPrefs.GetString(LEADERBOARD_KEY, "");
        
        if (string.IsNullOrEmpty(json))
        {
            return new List<ScoreEntry>();
        }
        
        try
        {
            var wrapper = JsonUtility.FromJson<SerializableList<ScoreEntry>>(json);
            return wrapper?.items ?? new List<ScoreEntry>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading leaderboard: {e.Message}");
            return new List<ScoreEntry>();
        }
    }
    
    public bool IsNewHighScore(int score)
    {
        List<ScoreEntry> leaderboard = LoadLeaderboard();
        
        if (leaderboard.Count == 0)
            return true; // First score is always a high score
            
        if (leaderboard.Count < MAX_LEADERBOARD_ENTRIES)
            return true; // Leaderboard not full, so it's a high score
            
        // Check if score beats the lowest score on leaderboard
        return score > leaderboard[leaderboard.Count - 1].score;
    }
    
    public bool IsNewTopScore(int score)
    {
        List<ScoreEntry> leaderboard = LoadLeaderboard();
        
        if (leaderboard.Count == 0)
            return true; // First score is always the top score
            
        // Check if score beats the current highest score
        return score > leaderboard[0].score;
    }
}

// Helper class for JSON serialization of Lists
[System.Serializable]
public class SerializableList<T>
{
    public List<T> items;
    
    public SerializableList(List<T> items)
    {
        this.items = items;
    }
}