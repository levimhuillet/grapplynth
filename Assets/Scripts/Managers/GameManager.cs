using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class GameManager : MonoBehaviour {
        public static GameManager instance;

        private SkinData m_currSkinData; // the player's current skin
        private Vector3 m_mostRecentCornerPos; // the most recently passed turn
                                               //(used for second lives)
        [SerializeField]
        private int m_maxKeepRunnings; // how many times the player is allowed to continue through ads
        private int m_numKeepRunnings; // how many times the player has continued through ads

        private bool m_gameIsPaused;

        public bool GameIsPaused {
            get { return m_gameIsPaused; }
        }
        
        #region Unity Callbacks

        private void Awake() {
            if (instance == null) {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (this != instance) {
                Destroy(this.gameObject);
            }
        }

        private void Start() {
            SetCurrSkin("skin-default");

            EventManager.OnRestart.AddListener(ResetKeepRunnings);
            EventManager.OnPause.AddListener(HandleOnPause);
            EventManager.OnResume.AddListener(HandleOnResume);
            EventManager.OnRestart.AddListener(HandleOnRestart);
            EventManager.OnGameOver.AddListener(HandleOnGameOver);
            EventManager.OnNewLife.AddListener(HandleOnNewLife);
            EventManager.OnReturnMain.AddListener(HandleOnReturnMain);
        }

        #endregion

        public SkinData GetCurrSkin() {
            return m_currSkinData;
        }
        public void SetCurrSkin(string id) {
            m_currSkinData = GameDB.GetSkinData(id);
        }

        public Vector3 GetMostRecentCorner() {
            return m_mostRecentCornerPos;
        }
        public void SetMostRecentCorner(Vector3 cornerPos) {
            m_mostRecentCornerPos = cornerPos;
        }

        public void IncrementKeepRunnings() {
            m_numKeepRunnings++;
        }
        public void ResetKeepRunnings() {
            m_numKeepRunnings = 0;
        }
        public bool CanKeepRunning() {
            return m_numKeepRunnings < m_maxKeepRunnings;
        }

        #region EventHandlers

        private void HandleOnPause() {
            m_gameIsPaused = true;
            Time.timeScale = 0;
        }

        private void HandleOnResume() {
            m_gameIsPaused = false;
            Time.timeScale = 1;
        }

        private void HandleOnRestart() {
            m_gameIsPaused = false;
            Time.timeScale = 1;
        }

        private void HandleOnGameOver() {
            m_gameIsPaused = true;
            Time.timeScale = 0;
        }


        private void HandleOnNewLife() {
            m_gameIsPaused = false;
            Time.timeScale = 1;
        }

        private void HandleOnReturnMain() {
            m_gameIsPaused = false;
            Time.timeScale = 1;
        }

        #endregion

    }
}