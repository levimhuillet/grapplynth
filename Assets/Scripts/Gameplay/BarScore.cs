using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class BarScore : MonoBehaviour {

        private bool visited = false;
        private List<GameObject> checkpoints = new List<GameObject>();

        // Start is called before the first frame update
        void Start() {
            
        }

        // Update is called once per frame
        void Update() {
            if (visited == false) {                
                //foreach (GameObject checkpoint in checkpoints) {
                //    BarCheckpoint check = checkpoint.gameObject.GetComponent<BarCheckpoint>();
                //}
                //EventManager.OnBarScore.Invoke();
                visited = true;
            }
        }

        private void OnTriggerExit(Collider other) {

        }
    }
}