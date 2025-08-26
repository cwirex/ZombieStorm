using Assets.Scripts.Player;
using Assets.Scripts.Weapon;
using Assets.Scripts.PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.PlayerScripts {
    public class UIController : MonoBehaviour {
        [Header("Game UI Elements")]
        [SerializeField] private Image weaponImage;
        [SerializeField] private TMP_Text medsCounter;
        [SerializeField] private TMP_Text tntsCounter;
        [SerializeField] private TMP_Text ammoCounter;
        [SerializeField] private TMP_Text scoreCounter;
        [SerializeField] List<Sprite> weaponSprites = new List<Sprite>();
        [SerializeField] private Slider ammoSlider;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private GameObject gameUI;
        
        [Header("Wave UI Elements")]
        [SerializeField] private TMP_Text waveDisplay;
        [SerializeField] private TMP_Text enemiesLeftDisplay;
        [SerializeField] private WaveCountdownTimer dialCountdownTimer;
        
        [Header("Menu UI Elements")]
        [SerializeField] private TMP_Text menuTitle;
        [SerializeField] private Button playButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;
        
        [Header("Score UI Elements")]
        [SerializeField] private TMP_Text finalScoreText;
        [SerializeField] private TMP_Text leaderboardText;
        
        private void Start() {
            // Subscribe to cash events for gameplay display
            if (CurrencyManager.Instance != null) {
                CurrencyManager.Instance.OnCashChanged += UpdateCashDisplay;
            }
            
            // Subscribe to score events only for final display (we'll handle this in ShowGameOver)
            // ScoreManager events are handled per UI state now
            
            // Subscribe to wave events
            if (WaveManager.Instance != null) {
                WaveManager.Instance.OnWaveStarted += UpdateWaveDisplay;
                WaveManager.Instance.OnWaveStateChanged += OnWaveStateChanged;
            }
            
            // Start updating enemies left counter periodically
            StartCoroutine(UpdateEnemiesLeftPeriodically());
            
            // Subscribe to dial countdown timer events
            if (dialCountdownTimer != null) {
                dialCountdownTimer.OnCountdownTick += OnDialCountdownTick;
                dialCountdownTimer.OnCountdownComplete += OnDialCountdownComplete;
            }
            
            // Initialize wave display if WaveManager is already started
            if (WaveManager.Instance != null && WaveManager.Instance.CurrentWaveNumber > 0) {
                UpdateWaveDisplay(WaveManager.Instance.CurrentWaveNumber);
            }
        }
        
        private void OnDestroy() {
            // Unsubscribe from events to prevent memory leaks
            if (CurrencyManager.Instance != null) {
                CurrencyManager.Instance.OnCashChanged -= UpdateCashDisplay;
            }
            
            if (WaveManager.Instance != null) {
                WaveManager.Instance.OnWaveStarted -= UpdateWaveDisplay;
                WaveManager.Instance.OnWaveStateChanged -= OnWaveStateChanged;
            }
            
            if (dialCountdownTimer != null) {
                dialCountdownTimer.OnCountdownTick -= OnDialCountdownTick;
                dialCountdownTimer.OnCountdownComplete -= OnDialCountdownComplete;
            }
        }
        
        private void UpdateCashDisplay(int cash) {
            SetCashCounter(cash);
        }
        
        private void UpdateWaveDisplay(int waveNumber) {
            if (waveDisplay != null) {
                string waveText = GetWaveDescription(waveNumber);
                waveDisplay.text = waveText;
            }
            
            // Update enemies left counter
            UpdateEnemiesLeftDisplay();
        }
        
        private string GetWaveDescription(int waveNumber) {
            if (WaveProgressionManager.Instance != null) {
                var progression = WaveProgressionManager.Instance.GetWaveProgression(waveNumber);
                
                // Check if current wave is boss
                if (progression.isBossWave) {
                    return $"Wave {waveNumber} - BOSS";
                }
                
                // Check if next wave is boss
                var nextProgression = WaveProgressionManager.Instance.GetWaveProgression(waveNumber + 1);
                if (nextProgression.isBossWave) {
                    return $"Wave {waveNumber}, next BOSS";
                }
                
                // Regular wave
                return $"Wave {waveNumber}";
            }
            
            // Fallback
            return $"Wave {waveNumber}";
        }
        
        private void UpdateEnemiesLeftDisplay() {
            if (enemiesLeftDisplay != null && WaveManager.Instance != null) {
                int enemiesLeft = WaveManager.Instance.ActiveEnemyCount;
                enemiesLeftDisplay.text = $"Enemies left: {enemiesLeft}";
                enemiesLeftDisplay.gameObject.SetActive(true);
            }
        }
        
        private void OnWaveStateChanged(WaveState waveState) {
            // Handle different wave states for UI updates
            switch (waveState) {
                case WaveState.Preparing:
                    // Hide enemies left counter during preparation
                    if (enemiesLeftDisplay != null) {
                        enemiesLeftDisplay.gameObject.SetActive(false);
                    }
                    break;
                case WaveState.Active:
                    // Show enemies left counter when wave starts
                    if (enemiesLeftDisplay != null) {
                        enemiesLeftDisplay.gameObject.SetActive(true);
                    }
                    break;
                case WaveState.Transition:
                    // Show countdown for next wave, hide enemies left counter
                    if (enemiesLeftDisplay != null) {
                        enemiesLeftDisplay.gameObject.SetActive(false);
                    }
                    StartDialCountdown();
                    break;
            }
        }
        
        private IEnumerator UpdateEnemiesLeftPeriodically() {
            while (true) {
                yield return new WaitForSeconds(0.5f); // Update twice per second
                
                // Only update if we're in an active wave state
                if (WaveManager.Instance != null && 
                    (WaveManager.Instance.CurrentWaveState == WaveState.Active || 
                     WaveManager.Instance.CurrentWaveState == WaveState.Cleanup)) {
                    UpdateEnemiesLeftDisplay();
                }
            }
        }
        
        
        private void StartDialCountdown() {
            if (dialCountdownTimer != null) {
                dialCountdownTimer.SetCountdownDuration(4f); // Updated to 4 seconds
                dialCountdownTimer.StartCountdown();
                
                // Freeze player during dial countdown
                FreezePlayer(true);
            }
        }
        
        
        private void FreezePlayer(bool freeze) {
            // Find the player and disable/enable movement
            var playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null) {
                playerMovement.enabled = !freeze;
            }
            
            // Also disable/enable weapon firing
            var weaponManager = FindObjectOfType<WeaponManager>();
            if (weaponManager != null) {
                weaponManager.enabled = !freeze;
            }
            
            // Disable/enable player input
            var gameInput = FindObjectOfType<GameInput>();
            if (gameInput != null) {
                gameInput.enabled = !freeze;
            }
        }
        
        private void OnDialCountdownTick(int secondsRemaining) {
            // Could display remaining seconds somewhere if needed
            Debug.Log($"Wave countdown: {secondsRemaining}");
        }
        
        private void OnDialCountdownComplete() {
            // Unfreeze player when countdown completes
            FreezePlayer(false);
        }
        
        private void StartInitialWaveCountdown() {
            if (dialCountdownTimer != null) {
                // Show wave 1 display during initial countdown
                if (waveDisplay != null) {
                    waveDisplay.text = GetWaveDescription(1);
                }
                
                // Start countdown and trigger first wave when complete
                dialCountdownTimer.SetCountdownDuration(4f);
                dialCountdownTimer.OnCountdownComplete += StartFirstWaveAfterCountdown;
                dialCountdownTimer.StartCountdown();
                
                // Freeze player during initial countdown
                FreezePlayer(true);
                
                Debug.Log("Initial wave countdown started");
            }
        }
        
        private void StartFirstWaveAfterCountdown() {
            // Unsubscribe from this one-time event
            if (dialCountdownTimer != null) {
                dialCountdownTimer.OnCountdownComplete -= StartFirstWaveAfterCountdown;
            }
            
            // Trigger first wave
            if (WaveManager.Instance != null) {
                WaveManager.Instance.StartWave(1);
            }
            
            Debug.Log("First wave started after initial countdown");
        }

        public void SetMedsCounter(int counter) { medsCounter.text = counter.ToString();}

        public void SetTntsCounter(int counter) {  tntsCounter.text = counter.ToString();}

        public void SetAmmoCounter(string ammoString) { ammoCounter.text = ammoString; }
        
        public void SetScoreCounter(int score) { 
            if (scoreCounter != null) {
                scoreCounter.text = score.ToString();
            }
        }
        
        public void SetCashCounter(int cash) { 
            if (scoreCounter != null) {
                scoreCounter.text = $"${cash}";
            }
        }

        public void SetAmmoSlider(float fillAmount) { ammoSlider.value = fillAmount; }
        
        // Public method to set wave display (can be called from external systems)
        public void SetWaveDisplay(int waveNumber) {
            UpdateWaveDisplay(waveNumber);
        }

        public void SetWeaponIcon(EWeapons weaponId) {
            int idx = (int)weaponId;
            if(idx < weaponSprites.Count) {
                Sprite weaponSprite = weaponSprites[idx];
                weaponImage.sprite = weaponSprite;
            } else {
                Debug.LogError("Weapon Icon Index out of range!");
            }
            

        }

        internal void UpdateItemCounter(Item item) {
            print(item);
            if(item.GetType() == typeof(Medkit)) {
                SetMedsCounter(item.Amount);
            } else if(item.GetType() == typeof(TNT)) {
                SetTntsCounter(item.Amount);
            }
        }

        internal void UpdateAmmoCounter(int ammoInMagazine, int ammoLeft, int magazineCapacity) {
            SetAmmoCounter($"{ammoInMagazine}/{ammoLeft}");
            SetAmmoSlider((float)ammoInMagazine / (float)magazineCapacity);
        }

        // Methods called by GameManager for different states
        public void ShowMainMenu() {
            pauseUI.SetActive(true);
            gameUI.SetActive(false);
            
            if (menuTitle) menuTitle.text = "ZombieStorm";
            SetButtonVisibility(playButton: true, resumeButton: false, restartButton: false, quitButton: true);
        }
        
        public void ShowGameplay() {
            pauseUI.SetActive(false);
            gameUI.SetActive(true);
            
            // Ensure time scale is correct for gameplay
            Time.timeScale = 1f;
            
            // Update wave display if wave is active
            if (WaveManager.Instance != null && WaveManager.Instance.CurrentWaveNumber > 0) {
                UpdateWaveDisplay(WaveManager.Instance.CurrentWaveNumber);
            }
            
            // Display current cash instead of score during gameplay
            if (CurrencyManager.Instance != null) {
                UpdateCashDisplay(CurrencyManager.Instance.CurrentCash);
            }
            
            // Start initial countdown for first wave if no wave has started yet
            if (WaveManager.Instance != null && WaveManager.Instance.CurrentWaveNumber == 0) {
                StartInitialWaveCountdown();
            }
        }
        
        public void ShowPauseMenu() {
            pauseUI.SetActive(true);
            gameUI.SetActive(false);
            
            if (menuTitle) menuTitle.text = "Game Paused";
            
            // Display current cash and cash receipt instead of score/leaderboard
            DisplayCurrentCash();
            DisplayCashReceipt();
            
            SetButtonVisibility(playButton: false, resumeButton: true, restartButton: true, quitButton: true);
        }
        
        public void ShowGameOver() {
            pauseUI.SetActive(true);
            gameUI.SetActive(false);
            
            if (menuTitle) menuTitle.text = "Game Over!";
            
            // Display final SCORE and leaderboard (not cash)
            DisplayFinalScore();
            DisplayLeaderboard();
            
            SetButtonVisibility(playButton: false, resumeButton: false, restartButton: true, quitButton: true);
        }
        
        private void DisplayFinalScore() {
            if (finalScoreText != null && ScoreManager.Instance != null) {
                int currentScore = ScoreManager.Instance.CurrentScore;
                bool isTopScore = ScoreManager.Instance.IsNewTopScore(currentScore);
                
                string scoreText = $"Score: {currentScore}";
                if (isTopScore) {
                    scoreText += "\n<color=yellow>NEW HIGH SCORE!</color>";
                }
                
                finalScoreText.text = scoreText;
            }
        }
        
        private void DisplayLeaderboard() {
            if (leaderboardText != null && ScoreManager.Instance != null) {
                var leaderboard = ScoreManager.Instance.LoadLeaderboard();
                int currentScore = ScoreManager.Instance.CurrentScore;
                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<color=yellow>TOP SCORES</color>");
                sb.AppendLine("═══════════════");
                
                if (leaderboard.Count == 0) {
                    sb.AppendLine("No scores yet!");
                } else {
                    for (int i = 0; i < leaderboard.Count; i++) {
                        string prefix = leaderboard[i].score == currentScore ? "> " : "";
                        sb.AppendLine($"{prefix}{leaderboard[i].score}");
                    }
                }
                
                leaderboardText.text = sb.ToString();
            }
        }
        
        private void DisplayCurrentCash() {
            if (finalScoreText != null && CurrencyManager.Instance != null) {
                int currentCash = CurrencyManager.Instance.CurrentCash;
                finalScoreText.text = $"Cash: ${currentCash}";
            }
        }
        
        private void DisplayCashReceipt() {
            if (leaderboardText != null && CurrencyManager.Instance != null) {
                var currency = CurrencyManager.Instance;
                
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<color=green>PAYOUT:</color>");
                sb.AppendLine("═══════════════");
                
                if (currency.EnemiesKilledThisWave > 0) {
                    sb.AppendLine($"Enemies killed: +${currency.CashFromEnemiesThisWave}");
                }
                
                if (currency.WaveCompletionBonus > 0) {
                    sb.AppendLine($"Wave finished: +${currency.WaveCompletionBonus}");
                }
                
                if (currency.BossBonus > 0) {
                    sb.AppendLine($"Boss bonus: +${currency.BossBonus}");
                }
                
                sb.AppendLine("───────────────");
                sb.AppendLine($"<color=yellow>Total: +${currency.TotalCashThisWave}</color>");
                
                leaderboardText.text = sb.ToString();
            }
        }
        
        private void SetButtonVisibility(bool playButton, bool resumeButton, bool restartButton, bool quitButton) {
            if (this.playButton) this.playButton.gameObject.SetActive(playButton);
            if (this.resumeButton) this.resumeButton.gameObject.SetActive(resumeButton);
            if (this.restartButton) this.restartButton.gameObject.SetActive(restartButton);
            if (this.quitButton) this.quitButton.gameObject.SetActive(quitButton);
        }

        // Button callback methods - these will be connected to UI buttons
        public void OnPlayButtonClicked() {
            GameManager.Instance?.StartGame();
        }
        
        public void OnResumeButtonClicked() {
            GameManager.Instance?.ResumeGame();
        }
        
        public void OnRestartButtonClicked() {
            GameManager.Instance?.RestartGame();
        }
        
        public void OnQuitButtonClicked() {
            GameManager.Instance?.QuitGame();
        }
        
        // Legacy methods for backward compatibility
        public void TogglePause() {
            if (GameManager.Instance?.CurrentState == GameState.Playing) {
                GameManager.Instance.PauseGame();
            } else if (GameManager.Instance?.CurrentState == GameState.Paused) {
                GameManager.Instance.ResumeGame();
            }
        }

        [System.Obsolete("Use GameManager.Instance.PauseGame() instead")]
        public void PauseGame() {
            GameManager.Instance?.PauseGame();
        }

        [System.Obsolete("Use GameManager.Instance.ResumeGame() instead")]
        public void ResumeGame() {
            GameManager.Instance?.ResumeGame();
        }

        [System.Obsolete("Use GameManager.Instance.RestartGame() instead")]
        public void RestartGame() {
            GameManager.Instance?.RestartGame();
        }

        [System.Obsolete("Use GameManager.Instance.QuitGame() instead")]
        public void QuitGame() {
            GameManager.Instance?.QuitGame();
        }
    }
}