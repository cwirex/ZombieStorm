using Assets.Scripts.Player;
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
        [SerializeField] List<Sprite> weaponSprites = new List<Sprite>();
        [SerializeField] private Slider ammoSlider;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private GameObject gameUI;
        
        [Header("Menu UI Elements")]
        [SerializeField] private TMP_Text menuTitle;
        [SerializeField] private Button playButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitButton;

        public void SetMedsCounter(int counter) { medsCounter.text = counter.ToString();}

        public void SetTntsCounter(int counter) {  tntsCounter.text = counter.ToString();}

        public void SetAmmoCounter(string ammoString) { ammoCounter.text = ammoString; }

        public void SetAmmoSlider(float fillAmount) { ammoSlider.value = fillAmount; }

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
        }
        
        public void ShowPauseMenu() {
            pauseUI.SetActive(true);
            gameUI.SetActive(false);
            
            if (menuTitle) menuTitle.text = "Game Paused";
            SetButtonVisibility(playButton: false, resumeButton: true, restartButton: true, quitButton: true);
        }
        
        public void ShowGameOver() {
            pauseUI.SetActive(true);
            gameUI.SetActive(false);
            
            if (menuTitle) menuTitle.text = "Game Over!";
            SetButtonVisibility(playButton: false, resumeButton: false, restartButton: true, quitButton: true);
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