using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class GameDB : MonoBehaviour {
        [SerializeField]
        private AudioData[] m_audioData;
        [SerializeField]
        private SkinData[] m_skinData;
        [SerializeField]
        private Color m_skinSlotSelected;
        [SerializeField]
        private Color m_skinSlotUnselected;

        private Dictionary<string, AudioData> m_audioMap;
        private Dictionary<string, SkinData> m_skinMap;

        private Dictionary<string, bool> m_skinsUnlocked;
        private List<string> m_skinsToUnlock;

        public int textSeed;
        public int gameSeed;
        public int currentGenID;

        public static GameDB instance;

        private void OnEnable() {
            gameSeed = textSeed;
            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                Destroy(this.gameObject);
            }

            InitSkinData();
        }

        public static AudioData GetAudioData(string id) {
            // initialize the map if it does not exist
            if (instance.m_audioMap == null) {
                instance.m_audioMap = new Dictionary<string, AudioData>();
                foreach (AudioData data in instance.m_audioData) {
                    instance.m_audioMap.Add(data.ID, data);
                }
            }
            if (instance.m_audioMap.ContainsKey(id)) {
                return instance.m_audioMap[id];
            }
            else {
                throw new KeyNotFoundException(string.Format("No Audio " +
                    "with id `{0}' is in the database", id
                ));
            }
        }

        public void InitSkinData() {
            instance.m_skinsUnlocked = new Dictionary<string, bool>();
            instance.m_skinsToUnlock = new List<string>();
            foreach (SkinData sd in m_skinData) {
                instance.m_skinsUnlocked.Add(sd.ID, sd.StartUnlocked);

                if (!sd.StartUnlocked) {
                    instance.m_skinsToUnlock.Add(sd.ID);
                }
            }
        }

        public static SkinData GetSkinData(string id) {
            // initialize the map if it does not exist
            if (instance.m_skinMap == null) {
                instance.m_skinMap = new Dictionary<string, SkinData>();
                foreach (SkinData skin in instance.m_skinData) {
                    instance.m_skinMap.Add(skin.ID, skin);
                }
            }
            if (instance.m_skinMap.ContainsKey(id)) {
                return instance.m_skinMap[id];
            }
            else {
                throw new KeyNotFoundException(string.Format("No Skin " +
                    "with id " + id + " is in the database"
                ));
            }
        }

        public bool IsSkinUnlocked(string id) {
            return m_skinsUnlocked[id];
        }
        public void UnlockSkin(string id) {
            m_skinsUnlocked[id] = true;
        }

        public Color GetSlotColor(bool selected) {
            if (selected) {
                return new Color(m_skinSlotSelected.r, m_skinSlotSelected.g, m_skinSlotSelected.b);
            }
            else {
                return new Color(m_skinSlotUnselected.r, m_skinSlotUnselected.g, m_skinSlotUnselected.b);
            }
        }

        public string RandomlyUnlockNewSkin() {
            if (m_skinsToUnlock.Count == 0) {
                return null;
            }

            int index = Random.Range(0, m_skinsToUnlock.Count);
            string unlockedID = m_skinsToUnlock[index];
            m_skinsToUnlock.RemoveAt(index);

            UnlockSkin(unlockedID);

            return unlockedID;
        }
    }
}