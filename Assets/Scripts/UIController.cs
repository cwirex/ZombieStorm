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
        [SerializeField] private Image weaponImage;
        [SerializeField] private TMP_Text medsCounter;
        [SerializeField] private TMP_Text tntsCounter;
        [SerializeField] private TMP_Text ammoCounter;
        [SerializeField] List<Sprite> weaponSprites = new List<Sprite>();
        [SerializeField] private Slider ammoSlider;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private GameObject gameUI;

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

        public void TogglePause() {
            bool isPaused = Time.timeScale == 0f;
            if (isPaused) ResumeGame();
            else PauseGame();
        }

        public void PauseGame() {
            pauseUI.SetActive(true);
            gameUI.SetActive(false);
            Time.timeScale = 0f;

        }

        public void ResumeGame() {
            pauseUI.SetActive(false);
            gameUI.SetActive(true);
            Time.timeScale = 1f;
        }

        public void RestartGame() {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitGame() {
            Application.Quit();
        }
    }
}