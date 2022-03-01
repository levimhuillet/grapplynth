using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    /// <summary>
    /// Manages Ambiance audio in a scene
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class AmbianceManager : MonoBehaviour, IAudioPlayer {
        private AudioSource m_ambianceSrc;

        private AudioManager.AudioLoopPair m_stashedAudio;
        private AudioData m_currData;
        private Queue<AudioManager.AudioLoopPair> m_audioQueue;

        #region Unity Callbacks

        private void Awake() {
            m_ambianceSrc = this.GetComponent<AudioSource>();
        }

        #endregion

        #region IAudioPlayer

        /// <summary>
        /// For longer sounds
        /// </summary>
        /// <param name="clipID"></param>
        public void PlayAudio(string clipID, bool loop = false) {
            LoadAmbianceAudio(clipID);
            m_ambianceSrc.loop = loop;
            m_ambianceSrc.Play();
        }

        public void PlayAudioWhen(string clipIDToPlay, string clipIDPlayWhen, bool loop = false) {
            // todo: implement this
        }

        public bool IsPlayingAudio() {
            return m_ambianceSrc.isPlaying;
        }

        public void StopAudio() {
            m_ambianceSrc.Stop();
        }

        public void ClearAudio() {
            m_currData = null;
            m_ambianceSrc.clip = null;
        }

        public void ResumeAudio() {
            if (m_ambianceSrc.clip == null) {
                return;
            }
            m_ambianceSrc.Play();
        }

        // Saves the current audio for later
        public void StashAudio() {
            m_stashedAudio = new AudioManager.AudioLoopPair(m_currData, m_ambianceSrc.loop);
        }

        // Saves the current audio for later
        public void ResumeStashedAudio() {
            if (m_stashedAudio.Data == null) {
                ClearAudio();
                return;
            }

            m_currData = m_stashedAudio.Data;
            AudioManager.LoadAudio(m_ambianceSrc, m_stashedAudio.Data);
            m_ambianceSrc.loop = m_stashedAudio.Loop;
            m_ambianceSrc.Play();
        }

        #endregion

        #region Helper Methods

        private void LoadAmbianceAudio(string clipID) {
            var data = GameDB.GetAudioData(clipID);
            m_currData = data;
            AudioManager.LoadAudio(m_ambianceSrc, data);
        }

        #endregion
    }
}
