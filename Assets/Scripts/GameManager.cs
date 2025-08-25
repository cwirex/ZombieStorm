using UnityEngine;
using Assets.Scripts.PlayerScripts;

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
        
        // Start in MainMenu only on first load, go to Playing on restart
        if (!hasStartedGameBefore) {
            ChangeState(GameState.MainMenu);
        } else {
            // This is a restart - go directly to playing
            ChangeState(GameState.Playing);
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
                break;
                
            case GameState.Paused:
                Time.timeScale = 0f;
                uiController?.ShowPauseMenu();
                break;
                
            case GameState.GameOver:
                uiController?.ShowGameOver();
                // Don't modify Time.timeScale here - HealthController handles slowdown
                break;
        }
        
        Debug.Log($"Game State changed to: {currentState}");
    }
    
    // Public methods for UI to call
    public void StartGame() {
        hasStartedGameBefore = true; // Mark that game has been started
        ChangeState(GameState.Playing);
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
        // Mark that game has started (so next load goes to Playing)
        hasStartedGameBefore = true;
        
        // Reset time scale before reloading
        Time.timeScale = 1f;
        
        // Reload the scene to restart
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
    
    public void QuitGame() {
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
}