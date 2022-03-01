using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Grapplynth {
    public class UIPauseMenu : MonoBehaviour {

        #region Editor

        [SerializeField]
        private Button m_resumeButton;
        [SerializeField]
        private Button m_restartButton;
        [SerializeField]
        private Button m_settingsButton;
        [SerializeField]
        private Button m_abandonButton;
        [SerializeField]
        private GameObject m_settingsMenu;

        #endregion

        #region Unity Callbacks

        private void OnEnable() {
            m_resumeButton.onClick.AddListener(HandleResume);
            m_restartButton.onClick.AddListener(HandleRestart);
            m_settingsButton.onClick.AddListener(HandleSettings);
            m_abandonButton.onClick.AddListener(HandleAbandon);
        }

        private void OnDisable() {
            m_resumeButton.onClick.RemoveListener(HandleResume);
            m_restartButton.onClick.RemoveListener(HandleRestart);
            m_settingsButton.onClick.RemoveListener(HandleSettings);
            m_abandonButton.onClick.RemoveListener(HandleAbandon);
        }

        #endregion

        #region ButtonHandlers

        private void HandleResume() {
            EventManager.OnResume.Invoke();
        }

        private void HandleRestart() {
            AudioManager.instance.PlayAudio("labyrinth", true);

            EventManager.OnRestart.Invoke();
        }

        private void HandleSettings() {
            AudioManager.instance.PlayOneShot("click_default");

            m_settingsMenu.SetActive(true);
        }

        private void HandleAbandon() {
            AudioManager.instance.PlayOneShot("click_default");

            EventManager.OnGameOver.Invoke();
        }

        #endregion
    }
}