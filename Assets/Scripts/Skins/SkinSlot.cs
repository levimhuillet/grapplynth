using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Grapplynth {
    public class SkinSlot : MonoBehaviour {
        [SerializeField]
        private string m_skinID;
        [SerializeField]
        private Button m_button;
        [SerializeField]
        private Image m_baseImage;
        [SerializeField]
        private GameObject m_lockedImage;

        public string SkinID {
            get { return m_skinID; }
        }
        public Button Button {
            get { return m_button; }
        }
        public Image BaseImage {
            get { return m_baseImage; }
        }
        public GameObject LockedImage {
            get { return m_lockedImage; }
        }

        private void OnEnable() {
            m_button.onClick.AddListener(HandleSelectSkin);
        }

        private void OnDisable() {
            m_button.onClick.RemoveListener(HandleSelectSkin);
        }

        private void HandleSelectSkin() {
            GameManager.instance.SetCurrSkin(m_skinID);
            UISkinSelectionMenu.instance.SelectSlotColor(m_skinID);
        }
    }
}