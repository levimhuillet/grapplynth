using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Grapplynth {
    public class UISettingsMenu : MonoBehaviour {

        #region Editor

        [SerializeField]
        private Button m_closeButton;
        [SerializeField]
        private InputField m_seedInput;

        #endregion

        #region Unity Callbacks

        private void Awake() {
            m_closeButton.onClick.AddListener(HandleClose);
            GameDB gameDB = GameObject.Find("GameDB").GetComponent<GameDB>();
            m_seedInput.text = gameDB.textSeed.ToString();
        }

        #endregion

        #region ButtonHandlers

        private void HandleClose() {
            AudioManager.instance.PlayOneShot("click_default");
            GameDB gameDB = GameObject.Find("GameDB").GetComponent<GameDB>();
            gameDB.gameSeed = ( (m_seedInput.text == null || m_seedInput.text.Length == 0 || int.TryParse(m_seedInput.text, out gameDB.textSeed) == false || gameDB.textSeed == 0) ? Random.Range(-2000000000, 2000000000) : (int.Parse(m_seedInput.text)) );
            this.gameObject.SetActive(false);
        }

        #endregion
    }
}

