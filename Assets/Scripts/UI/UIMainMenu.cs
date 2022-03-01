using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Grapplynth {
    public class UIMainMenu : MonoBehaviour {

        #region Editor

        [SerializeField]
        private Button m_playGameButton;
        [SerializeField]
        private Button m_settingsButton;
        [SerializeField]
        private Button m_quitButton;
        [SerializeField]
        private Button m_resetButton;
        [SerializeField]
        private Button m_adButton;
        [SerializeField]
        private Button m_selectSkinButton;
        [SerializeField]
        private GameObject m_settingsMenu;
        [SerializeField]
        private GameObject m_skinSelectionMenu;
        [SerializeField]
        private GameObject m_noAdsMenu;
        [SerializeField]
        private GameObject m_adsRewardMenu;


        #endregion

        #region Unity Callbacks

        private void OnEnable() {
            m_playGameButton.onClick.AddListener(HandlePlayGame);
            m_settingsButton.onClick.AddListener(HandleSettings);
            m_quitButton.onClick.AddListener(HandleQuit);
            m_resetButton.onClick.AddListener(HandleResetProgress);
            m_selectSkinButton.onClick.AddListener(HandleSelectSkin);

            EventManager.OnNoAds.AddListener(HandleOnNoAds);
            EventManager.OnAdReward.AddListener(HandleOnAdReward);
        }

        private void OnDisable() {
            m_playGameButton.onClick.RemoveAllListeners();
            m_settingsButton.onClick.RemoveAllListeners();
            m_quitButton.onClick.RemoveAllListeners();
            m_resetButton.onClick.RemoveListener(HandleResetProgress);
            m_selectSkinButton.onClick.RemoveListener(HandleSelectSkin);

            EventManager.OnNoAds.RemoveListener(HandleOnNoAds);
            EventManager.OnAdReward.RemoveListener(HandleOnAdReward);
        }

        private void Start() {
            AudioManager.instance.PlayAudio("title", true);
        }

        #endregion

        #region ButtonHandlers

        private void HandlePlayGame() {
            AudioManager.instance.PlayOneShot("click_play");
            GameManager.instance.ResetKeepRunnings();
            AudioManager.instance.PlayAudio("labyrinth", true);
            SceneManager.LoadScene("Labyrinth");
            EventManager.OnStart.Invoke();
        }
        private void HandleSettings() {
            AudioManager.instance.PlayOneShot("click_default");

            m_settingsMenu.SetActive(true);
        }
        private void HandleQuit() {
            AudioManager.instance.PlayOneShot("click_quit");

            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private void HandleResetProgress() {
            ScoreManager.instance.LoadHighScore(-1);
            GameDB.instance.InitSkinData();
            GameManager.instance.SetCurrSkin("skin-default");
            PlayerPrefsManager.instance.ResetPrefs();

            AudioManager.instance.PlayOneShot("click_default");
        }

        private void HandleSelectSkin() {
            AudioManager.instance.PlayOneShot("click_default");

            m_skinSelectionMenu.SetActive(true);
        }

        #endregion

        #region AdHandlers

        private void HandleOnNoAds() {
            m_adButton.interactable = false;
            m_selectSkinButton.interactable = false;
            m_noAdsMenu.SetActive(true);
        }

        private void HandleOnAdReward() {
            m_adButton.interactable = false;
            m_selectSkinButton.interactable = false;
            m_adsRewardMenu.SetActive(true);
        }

        #endregion
    }
}