using UnityEngine;
using UnityEngine.UI;

namespace Grapplynth {
    public class UINoAdsMenu : MonoBehaviour {

        #region Editor

        [SerializeField]
        private Button m_closeButton;
        [SerializeField]
        private Button[] m_buttonsToEnable;

        #endregion

        #region Unity Callbacks

        private void Awake() {
            m_closeButton.onClick.AddListener(HandleClose);
        }

        #endregion

        #region ButtonHandlers

        private void HandleClose() {
            AudioManager.instance.PlayOneShot("click_default");
            this.gameObject.SetActive(false);
            foreach (Button button in m_buttonsToEnable) {
               button.interactable = true;
            }
        }

        #endregion
    }
}

