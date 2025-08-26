using System;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }
    
    [Header("Currency Settings")]
    [SerializeField] private int startingCash = 0;
    
    // Current cash and tracking
    private int currentCash = 0;
    private int cashAtWaveStart = 0;
    private int enemiesKilledThisWave = 0;
    private int cashFromEnemiesThisWave = 0;
    private int waveCompletionBonus = 0;
    private int bossBonus = 0;
    
    // Events
    public event Action<int> OnCashChanged;
    
    // Properties
    public int CurrentCash => currentCash;
    public int CashAtWaveStart => cashAtWaveStart;
    public int EnemiesKilledThisWave => enemiesKilledThisWave;
    public int CashFromEnemiesThisWave => cashFromEnemiesThisWave;
    public int WaveCompletionBonus => waveCompletionBonus;
    public int BossBonus => bossBonus;
    public int TotalCashThisWave => cashFromEnemiesThisWave + waveCompletionBonus + bossBonus;
    
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
        currentCash = startingCash;
        cashAtWaveStart = currentCash;
        
        // Subscribe to score events to mirror cash earnings
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += OnScoreEarned;
        }
        
        // Subscribe to wave events for bonuses
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted += OnWaveStarted;
            WaveManager.Instance.OnWaveCompleted += OnWaveCompleted;
        }
        
        OnCashChanged?.Invoke(currentCash);
    }
    
    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= OnScoreEarned;
        }
        
        if (WaveManager.Instance != null)
        {
            WaveManager.Instance.OnWaveStarted -= OnWaveStarted;
            WaveManager.Instance.OnWaveCompleted -= OnWaveCompleted;
        }
    }
    
    private void OnScoreEarned(int newScore)
    {
        // Calculate the score difference to add equivalent cash
        int scoreDiff = newScore - ScoreManager.Instance.GetPreviousScore();
        if (scoreDiff > 0)
        {
            AddCashFromEnemy(scoreDiff);
        }
    }
    
    private void AddCashFromEnemy(int amount)
    {
        currentCash += amount;
        cashFromEnemiesThisWave += amount;
        enemiesKilledThisWave++;
        
        OnCashChanged?.Invoke(currentCash);
    }
    
    private void OnWaveStarted(int waveNumber)
    {
        // Reset wave tracking
        cashAtWaveStart = currentCash;
        enemiesKilledThisWave = 0;
        cashFromEnemiesThisWave = 0;
        waveCompletionBonus = 0;
        bossBonus = 0;
    }
    
    private void OnWaveCompleted(int waveNumber)
    {
        // Calculate wave completion bonus: 100 * waveNumber + 500 * bossLevel
        bool isBoss = IsBossWave(waveNumber);
        int bossLevel = isBoss ? GetBossLevel(waveNumber) : 0;
        
        waveCompletionBonus = 100 * waveNumber;
        bossBonus = 500 * bossLevel;
        
        int totalBonus = waveCompletionBonus + bossBonus;
        currentCash += totalBonus;
        
        Debug.Log($"Wave {waveNumber} completed! Bonus: {totalBonus} (Wave: {waveCompletionBonus}, Boss: {bossBonus})");
        OnCashChanged?.Invoke(currentCash);
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
        // Simple boss level calculation - could be made more sophisticated
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
    
    public bool SpendCash(int amount)
    {
        if (currentCash >= amount)
        {
            currentCash -= amount;
            OnCashChanged?.Invoke(currentCash);
            return true;
        }
        return false;
    }
    
    public void AddCash(int amount)
    {
        currentCash += amount;
        OnCashChanged?.Invoke(currentCash);
    }
    
    // For debugging
    public void ResetCash()
    {
        currentCash = startingCash;
        cashAtWaveStart = currentCash;
        enemiesKilledThisWave = 0;
        cashFromEnemiesThisWave = 0;
        waveCompletionBonus = 0;
        bossBonus = 0;
        OnCashChanged?.Invoke(currentCash);
    }
}