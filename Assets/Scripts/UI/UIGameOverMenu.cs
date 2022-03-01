using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Grapplynth {
    public class UIGameOverMenu : MonoBehaviour {

        #region Editor

        [SerializeField]
        private Button m_mainMenuButton;
        [SerializeField]
        private Button m_playAgainButton;
        [SerializeField]
        private Text m_scoreText;
        [SerializeField]
        private Text m_highScoreText;
        [SerializeField]
        private GameObject m_noAdsMenu;
        [SerializeField]
        private GameObject m_adsRewardMenu;
        [SerializeField]
        private Button m_keepRunningButton;

        #endregion

        #region Unity Callbacks

        private void OnEnable() {
            AudioManager.instance.PauseAudio();

            m_mainMenuButton.onClick.AddListener(HandleMainMenu);
            m_playAgainButton.onClick.AddListener(HandlePlayAgain);

            UpdateScoreText();

            EventManager.OnNoAds.AddListener(HandleOnNoAds);
            EventManager.OnAdReward.AddListener(HandleOnAdReward);

            m_keepRunningButton.interactable = GameManager.instance.CanKeepRunning();
        }

        private void OnDisable() {
            m_mainMenuButton.onClick.RemoveAllListeners();
            m_playAgainButton.onClick.RemoveAllListeners();

            m_highScoreText.gameObject.SetActive(false);

            EventManager.OnNoAds.RemoveListener(HandleOnNoAds);
            EventManager.OnAdReward.RemoveListener(HandleOnAdReward);
        }

        #endregion

        #region ButtonHandlers

        private void HandleMainMenu() {
            AudioManager.instance.PlayOneShot("click_default");

            EventManager.OnReturnMain.Invoke();

            SceneManager.LoadScene("MainMenu");
        }

        private void HandlePlayAgain() {
            AudioManager.instance.PlayOneShot("click_play");

            AudioManager.instance.PlayAudio("labyrinth", true);

            EventManager.OnRestart.Invoke();
        }

        #endregion

        private void UpdateScoreText() {
            m_scoreText.text = "Score: " + ScoreManager.instance.CurrScore;

            if (ScoreManager.instance.WasHighScore) {
                m_highScoreText.gameObject.SetActive(true);
            }
        }

        #region AdHandlers

        private void HandleOnNoAds() {
            m_noAdsMenu.SetActive(true);
        }

        private void HandleOnAdReward() {
            m_adsRewardMenu.SetActive(true);
        }

        #endregion
    }
}
