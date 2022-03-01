using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeCameraSorting : MonoBehaviour
{
    private Vector3 oldOffset;
    private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        RecalcSortingAxis();
    }

    public void SetOffset(Vector3 newOffset)
    {
        oldOffset = offset;
        offset = newOffset;
    }

    public void ResetOffset()
    {
        offset = oldOffset;
    }

    public void RecalcSortingAxis()
    {
        GraphicsSettings.transparencySortAxis = transform.forward + offset;
    }
}
