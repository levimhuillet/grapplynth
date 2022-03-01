using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraSortTrigger : MonoBehaviour
{
    [SerializeField] private Vector3 newOffset;

    public void OnTriggerEnter(Collider other)
    {
        Camera.main.GetComponent<ChangeCameraSorting>().SetOffset(newOffset);
    }

    public void OnTriggerExit(Collider other)
    {
        Camera.main.GetComponent<ChangeCameraSorting>().ResetOffset();
    }
}
