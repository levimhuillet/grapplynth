using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SegmentScript : MonoBehaviour
{
    public int width;   // x
    public int height;  // y
    public int length;  // z
    public int deltay;  // change in height
    public int segmentID;      // segment ID
    public int generatorID;      // segment ID
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnTriggerEnter(Collider collider) {
        //GenerateLevel generatelevel = FindGameObjectOfType(GenerateLevel);
        //generateLevel.GetCurrentSegment(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
