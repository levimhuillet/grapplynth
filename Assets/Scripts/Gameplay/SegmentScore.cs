using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class SegmentScore : MonoBehaviour {

        private bool visited = false;
        [SerializeField] int pieceScore;

        // Start is called before the first frame update
        void Start() {
            
        }

        // Update is called once per frame
        void Update() {

        }

        private void OnTriggerEnter(Collider other) {
            if (visited == false) {
                ScoreManager.instance.Points = pieceScore;
                EventManager.OnSegmentScore.Invoke();
                visited = true;
            }
        }
    }
}