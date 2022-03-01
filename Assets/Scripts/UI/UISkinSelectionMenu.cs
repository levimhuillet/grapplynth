using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Grapplynth {
    public class UISkinSelectionMenu : MonoBehaviour {

        #region Editor

        [SerializeField]
        private Button m_closeButton;
        [SerializeField]
        private SkinSlot[] m_slots;

        #endregion

        public static UISkinSelectionMenu instance;

        #region Unity Callbacks

        private void Awake() {
            if (instance == null) {
                instance = this;
            }
            else if (this != instance) {
                Destroy(this.gameObject);
            }
        }

        private void OnEnable() {
            if (m_closeButton != null) {
                m_closeButton.onClick.AddListener(HandleClose);
            }

            foreach(SkinSlot slot in m_slots) {
                bool isUnlocked = GameDB.instance.IsSkinUnlocked(slot.SkinID);
                slot.Button.interactable = isUnlocked;
                slot.LockedImage.SetActive(!isUnlocked);

                SelectSlotColor(GameManager.instance.GetCurrSkin().ID);
            }
        }

        private void OnDisable() {
            if (m_closeButton != null) {
                m_closeButton.onClick.RemoveListener(HandleClose);
            }
        }

        #endregion

        #region ButtonHandlers

        private void HandleClose() {
            AudioManager.instance.PlayOneShot("click_default");
            this.gameObject.SetActive(false);
        }

        #endregion

        public void SelectSlotColor(string selectedID) {
            foreach(SkinSlot slot in m_slots) {
                slot.BaseImage.color = GameDB.instance.GetSlotColor(slot.SkinID == selectedID);
            }
        }
    }
}