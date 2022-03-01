using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class PlayerPrefsManager : MonoBehaviour {
        public static PlayerPrefsManager instance;

        private static string HIGH_SCORE_KEY = "highScore";
        private static string CURR_SKIN_KEY = "currSkin";
        private static string BLUE_SKIN_UNLOCKED_KEY = "blueSkinUnlocked";
        private static string GREEN_SKIN_UNLOCKED_KEY = "greenSkinUnlocked";
        private static string RED_SKIN_UNLOCKED_KEY = "redSkinUnlocked";

        private void OnEnable() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (this != instance) {
                Destroy(this.gameObject);
                return;
            }
        }

        private void Start() {
            LoadPrefs();
        }

        private void OnApplicationQuit() {
            SavePrefs();
        }

        private void SavePrefs() {
            // High Score
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, ScoreManager.instance.HighScore);

            // Unlocked Skins
            int blueUnlocked = 0;
            if (GameDB.instance.IsSkinUnlocked("skin-blue")) {
                blueUnlocked = 1;
            }
            PlayerPrefs.SetInt(BLUE_SKIN_UNLOCKED_KEY, blueUnlocked);

            int greenUnlocked = 0;
            if (GameDB.instance.IsSkinUnlocked("skin-green")) {
                greenUnlocked = 1;
            }
            PlayerPrefs.SetInt(GREEN_SKIN_UNLOCKED_KEY, greenUnlocked);

            int redUnlocked = 0;
            if (GameDB.instance.IsSkinUnlocked("skin-red")) {
                redUnlocked = 1;
            }
            PlayerPrefs.SetInt(RED_SKIN_UNLOCKED_KEY, redUnlocked);

            // Current Skin
            PlayerPrefs.SetString(CURR_SKIN_KEY, GameManager.instance.GetCurrSkin().ID);

            PlayerPrefs.Save();
        }
        private void LoadPrefs() {
            // High Score
            int score = PlayerPrefs.GetInt(HIGH_SCORE_KEY, -1);
            ScoreManager.instance.LoadHighScore(score);

            // Unlocked Skins
            if (PlayerPrefs.GetInt(BLUE_SKIN_UNLOCKED_KEY, 0) == 1) {
                GameDB.instance.UnlockSkin("skin-blue");
            }
            if (PlayerPrefs.GetInt(GREEN_SKIN_UNLOCKED_KEY, 0) == 1) {
                GameDB.instance.UnlockSkin("skin-green");
            }
            if (PlayerPrefs.GetInt(RED_SKIN_UNLOCKED_KEY, 0) == 1) {
                GameDB.instance.UnlockSkin("skin-red");
            }

            // Current Skin
            string currID = PlayerPrefs.GetString(CURR_SKIN_KEY, "skin-default");
            GameManager.instance.SetCurrSkin(currID);
        }
        public void ResetPrefs() {
            // High Score
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, -1);

            // Unlocked Skins
            PlayerPrefs.SetInt(BLUE_SKIN_UNLOCKED_KEY, 0);
            PlayerPrefs.SetInt(GREEN_SKIN_UNLOCKED_KEY, 0);
            PlayerPrefs.SetInt(RED_SKIN_UNLOCKED_KEY, 0);

            // Current Skin
            GameManager.instance.SetCurrSkin("skin-default");

            PlayerPrefs.Save();
        }
    }
}