using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.PlayerScripts {
    public class UIController : MonoBehaviour {
        [SerializeField] Image weaponImage;
        [SerializeField] TMP_Text medsCounter;
        [SerializeField] TMP_Text tntsCounter;
        [SerializeField] List<Sprite> weaponSprites = new List<Sprite>();

        public void SetMedsCounter(int counter) { medsCounter.text = counter.ToString();}

        public void SetTntsCounter(int counter) {  tntsCounter.text = counter.ToString();}

        public void SetWeaponIcon(EWeapons weaponId) {
            int idx = (int)weaponId;
            if(idx < weaponSprites.Count) {
                Sprite weaponSprite = weaponSprites[idx];
                weaponImage.sprite = weaponSprite;
            } else {
                Debug.LogError("Weapon Icon Index out of range!");
            }
            

        }
        // Use this for initialization
        void Start() {
            
        }

        // Update is called once per frame
        void Update() {

        }
    }
}