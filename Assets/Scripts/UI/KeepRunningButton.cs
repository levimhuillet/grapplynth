using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Grapplynth
{
    public class KeepRunningButton : MonoBehaviour
    {
        private Button m_button;

        private void Awake() {
            m_button = this.GetComponent<Button>();

            RevalButton();

            EventManager.OnRestart.AddListener(HandleOnRestart);
            EventManager.OnReturnMain.AddListener(HandleOnReturnMain);
            EventManager.OnStart.AddListener(HandleOnStart);
        }

        private void OnClicked() {
            EventManager.OnNewLife.Invoke();
        }

        private void RevalButton() {
            m_button.onClick.RemoveAllListeners();
            m_button.onClick.AddListener(OnClicked);

            m_button.interactable = GameManager.instance.CanKeepRunning();
        }

        private void HandleOnStart() {
            RevalButton();
        }

        private void HandleOnReturnMain() {
            RevalButton();
        }


        private void HandleOnRestart() {
            RevalButton();
        }
    }
}