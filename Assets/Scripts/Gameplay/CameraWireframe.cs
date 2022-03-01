using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWireframe : MonoBehaviour
{
    [SerializeField] private bool active;
    private void Start()
    {
        if (!active)
            Destroy(this);
    }

    void OnPreRender()
    {
        GL.wireframe = true;
    }

    void OnPostRender()
    {
        GL.wireframe = false;
    }
}
