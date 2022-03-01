using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform cameraTargetPosition;
    [SerializeField] private Transform playerTransform;

    private LayerMask whatIsGrapplable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;
        if (Physics.Linecast(playerTransform.position, cameraTargetPosition.position, out hitInfo, whatIsGrapplable))
        {
            transform.position = hitInfo.point;
        }
        else
        {
            transform.position = cameraTargetPosition.position;
        }
        transform.rotation = playerTransform.rotation;
    }

    public void LoadVarsFromPlayerController(Grapplynth.PlayerController playerController)
    {
        whatIsGrapplable = playerController.WhatIsGrapplable;
    }
}
