using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class Chaser : MonoBehaviour {
        public float x, y, z;
        public float speed = 0.046875f;
        public int rotation;
        public int chaserID;
        private static int numChasers;
        public bool killed;

        private Vector3 startPosition; //used for resetting to beginning of corner

        private void OnEnable() {
            EventManager.OnRestart.AddListener(ResetNumChasers);
            EventManager.OnGameOver.AddListener(HandleOnGameOver);
        }

        private void OnDisable() {
            EventManager.OnRestart.RemoveListener(ResetNumChasers);
            EventManager.OnGameOver.RemoveListener(HandleOnGameOver);
        }

        private void HandleOnGameOver() {
            this.transform.position = startPosition;
            x = startPosition.x;
            y = startPosition.y;
            z = startPosition.z;
            Debug.Log("game Over finished calling");
        }

        public void SetStartPosition(Vector3 start) {
            startPosition = start;
            Debug.Log("start pos: " + start);
        }

        private void OnDestroy() {
            if (killed == false) {
                ResetNumChasers();
            }
        }

        private void ResetNumChasers() {
            numChasers = 0;
        }

        // Start is called before the first frame update
        void Awake() {
            x = transform.position.x;
            y = transform.position.y;
            z = transform.position.z;
            
            gameObject.transform.eulerAngles = new Vector3(0, rotation, 0);
            chaserID = numChasers;
            if (chaserID == 0) {
                numChasers = 0;
                startPosition = new Vector3(x, y, z);
            }
            numChasers++;
        }

        // Update is called once per frame
        void Update() {
            if (GameManager.instance.GameIsPaused) {
                return;
            }
            x += ((rotation + 360) % 180 == 90 ? speed : 0.0f);
            y += 0.0f;
            z += ((rotation + 360) % 180 == 0 ? speed : 0.0f);
            transform.position = new Vector3(x, y, z);
        }
    }
}