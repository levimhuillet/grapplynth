using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class BarCheckpoint : MonoBehaviour {

        private bool visited = false;

        // Start is called before the first frame update
        void Start() {
            
        }

        // Update is called once per frame
        void Update() {

        }

        private void OnTriggerExit(Collider other) {
            if (visited == false) {
                EventManager.OnBarScore.Invoke();
                visited = true;
            }
        }
    }
}