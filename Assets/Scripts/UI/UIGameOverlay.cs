using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Grapplynth {
    public class UIGameOverlay : MonoBehaviour {

        #region Editor

        [SerializeField]
        private Button m_pauseButton;
        [SerializeField]
        private GameObject m_pauseMenu;
        [SerializeField]
        private GameObject m_gameOverMenu;
        [SerializeField]
        private Text m_overlayScoreText;

        #endregion

        #region Unity Callbacks

        private void OnEnable() {
            m_pauseButton.onClick.AddListener(HandlePause);
            EventManager.OnPause.AddListener(HandleOnPause);
            EventManager.OnResume.AddListener(HandleOnResume);
            EventManager.OnRestart.AddListener(HandleOnRestart);
            EventManager.OnGameOver.AddListener(HandleOnGameOver);
            EventManager.OnNewLife.AddListener(HandleOnNewLife);

            EventManager.OnTurnCorner.AddListener(UpdateScoreText);

            EventManager.OnScoreChanged.AddListener(UpdateScoreText);
        }

        private void OnDisable () {
            m_pauseButton.onClick.RemoveListener(HandlePause);
            EventManager.OnPause.RemoveListener(HandleOnPause);
            EventManager.OnResume.RemoveListener(HandleOnResume);
            EventManager.OnRestart.RemoveListener(HandleOnRestart);
            EventManager.OnGameOver.RemoveListener(HandleOnGameOver);

            EventManager.OnTurnCorner.RemoveListener(UpdateScoreText);

            EventManager.OnScoreChanged.RemoveListener(UpdateScoreText);
        }

        private void Start() {
            UpdateScoreText();
        }

        #endregion

        #region Button Handlers

        private void HandlePause() {
            EventManager.OnPause.Invoke();
        }

        #endregion

        #region Event Handlers

        private void HandleOnPause() {
            AudioManager.instance.PlayOneShot("click_default");

            AudioManager.instance.PauseAudio();

            m_pauseMenu.SetActive(true);
            m_pauseButton.interactable = false;
        }

        private void HandleOnResume() {
            AudioManager.instance.PlayOneShot("click_default");
            AudioManager.instance.UnPauseAudio();

            m_pauseMenu.SetActive(false);
            m_pauseButton.interactable = true;
        }

        private void HandleOnRestart() {
            AudioManager.instance.PlayOneShot("click_play");

            m_pauseMenu.SetActive(false);
            m_pauseButton.interactable = true;

            m_gameOverMenu.SetActive(false);

            SceneManager.LoadScene(1);
        }

        private void HandleOnGameOver() {
            m_gameOverMenu.SetActive(true);
            m_pauseButton.interactable = false;
        }

        public void HandleOnNewLife() {
            AudioManager.instance.PlayOneShot("click_play");
            m_pauseMenu.SetActive(false);
            m_pauseButton.interactable = true;

            m_gameOverMenu.SetActive(false);
        }


        #endregion

        private void UpdateScoreText() {
            m_overlayScoreText.text = "Score: " + ScoreManager.instance.CurrScore;
        }
    }
}