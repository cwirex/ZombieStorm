using UnityEngine;
using Assets.Scripts.PlayerScripts;
using Assets.Scripts.Audio;

public enum GameState {
    MainMenu,    // Game hasn't started yet
    Playing,     // Normal gameplay
    Paused,      // Player pressed escape
    GameOver     // Player died
}

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private GameState currentState = GameState.MainMenu;
    private UIController uiController;
    private PlayerSpawner playerSpawner;
    private static bool hasStartedGameBefore = false; // Track if we've played before
    
    public GameState CurrentState => currentState;
    
    private void Awake() {
        // Singleton pattern - but allow recreation on scene reload
        if (Instance == null) {
            Instance = this;
            // Don't use DontDestroyOnLoad - let it recreate with scene
        } else {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start() {
        uiController = FindObjectOfType<UIController>();
        playerSpawner = FindObjectOfType<PlayerSpawner>();
        
        // Subscribe to WaveManager events if it exists
        var waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnWaveStarted += OnWaveStarted;
            waveManager.OnWaveCompleted += OnWaveCompleted;
            Debug.Log("GameManager connected to WaveManager events");
        }
        
        // Start in MainMenu only on first load, go to Playing on restart
        if (!hasStartedGameBefore) {
            // Play game initialization sound on first run
            SoundEvents.TriggerGameInitialized();
            ChangeState(GameState.MainMenu);
        } else {
            // This is a restart - go directly to playing and spawn player
            ChangeState(GameState.Playing);
            SpawnPlayerAtStart();
        }
    }
    
    public void ChangeState(GameState newState) {
        currentState = newState;
        
        // Ensure we have a valid UI controller reference
        if (uiController == null) {
            uiController = FindObjectOfType<UIController>();
        }
        
        switch (currentState) {
            case GameState.MainMenu:
                Time.timeScale = 0f;
                uiController?.ShowMainMenu();
                break;
                
            case GameState.Playing:
                Time.timeScale = 1f;
                uiController?.ShowGameplay();
                
                // Stop ghost manifestation sound when game starts
                if (SoundManager.Instance != null)
                {
                    SoundManager.Instance.StopGameInitializationSound();
                }
                break;
                
            case GameState.Paused:
                Time.timeScale = 0f;
                uiController?.ShowPauseMenu();
                break;
                
            case GameState.GameOver:
                uiController?.ShowGameOver();
                
                // Start game over ambient music
                SoundEvents.TriggerGameOverStart();
                
                // Don't modify Time.timeScale here - HealthController handles slowdown
                break;
        }
        
        Debug.Log($"Game State changed to: {currentState}");
    }
    
    // Public methods for UI to call
    public void StartGame() {
        hasStartedGameBefore = true; // Mark that game has been started
        ChangeState(GameState.Playing);
        SpawnPlayerAtStart();
    }
    
    public void PauseGame() {
        if (currentState == GameState.Playing) {
            ChangeState(GameState.Paused);
        }
    }
    
    public void ResumeGame() {
        if (currentState == GameState.Paused) {
            ChangeState(GameState.Playing);
        }
    }
    
    public void GameOver() {
        // Save the current score to leaderboard before changing state
        if (ScoreManager.Instance != null) {
            ScoreManager.Instance.GameFinished();
        }
        
        ChangeState(GameState.GameOver);
    }
    
    
    public void RestartGame() {
        // Stop game over ambient music
        SoundEvents.TriggerGameOverEnd();
        
        // Reset persistent game state before scene reload
        ResetGameState();
        
        // Mark that game has started (so next load goes to Playing)
        hasStartedGameBefore = true;
        
        // Reset time scale before reloading
        Time.timeScale = 1f;
        
        // Reload the scene to restart
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    private void ResetGameState() {
        // Reset weapon levels and ownership (player keeps only pistol at level 1)
        if (Assets.Scripts.Shop.WeaponLevelTracker.Instance != null) {
            Assets.Scripts.Shop.WeaponLevelTracker.Instance.ResetAllWeapons();
        }
        
        // Reset score to 0
        if (ScoreManager.Instance != null) {
            ScoreManager.Instance.ResetScore();
        }
        
        // Reset wave progress
        if (WaveManager.Instance != null) {
            WaveManager.Instance.ResetWaveProgress();
        }
        
        // Reset currency to starting amount (3000)
        if (CurrencyManager.Instance != null) {
            CurrencyManager.Instance.ResetCash();
        }
    }
    
    // Wave management event handlers
    private void OnWaveStarted(int waveNumber)
    {
        Debug.Log($"GameManager: Wave {waveNumber} started");
        // Ensure we're in Playing state when wave starts
        if (currentState != GameState.Playing)
        {
            ChangeState(GameState.Playing);
        }
        // Spawn player at start of each wave
        SpawnPlayerForWave();
    }
    
    private void OnWaveCompleted(int waveNumber)
    {
        Debug.Log($"GameManager: Wave {waveNumber} completed");
        // Note: Player teleportation now happens when pressing space to start next wave
        // No longer auto-teleporting at wave end
    }
    
    private void SpawnPlayerAtStart()
    {
        if (playerSpawner != null)
        {
            playerSpawner.OnGameStart();
        }
        else
        {
            Debug.LogWarning("GameManager: No PlayerSpawner found!");
        }
    }
    
    private void SpawnPlayerForWave()
    {
        if (playerSpawner != null)
        {
            playerSpawner.OnWaveStart();
        }
        else
        {
            Debug.LogWarning("GameManager: No PlayerSpawner found for wave start!");
        }
    }
    
    private void OnWaveStateChanged(object waveState)
    {
        // Future: Handle different wave states (preparing, active, cleanup, etc.)
    }
    
    public void QuitGame() {
        // Stop game over ambient music
        SoundEvents.TriggerGameOverEnd();
        
        Debug.Log("Quit Game requested");
        
        #if UNITY_EDITOR
            // In Unity Editor, stop play mode
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_WEBGL
            // WebGL can't quit, so just show a message or redirect
            Debug.Log("WebGL build - cannot quit application");
        #else
            // In standalone builds, quit normally
            Application.Quit();
        #endif
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from WaveManager events
        var waveManager = FindObjectOfType<WaveManager>();
        if (waveManager != null)
        {
            waveManager.OnWaveStarted -= OnWaveStarted;
            waveManager.OnWaveCompleted -= OnWaveCompleted;
        }
    }
}