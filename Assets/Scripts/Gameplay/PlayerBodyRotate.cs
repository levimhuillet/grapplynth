using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
public class PlayerBodyRotate : MonoBehaviour
{
    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform headTransform;
    [SerializeField] private GrappleGun leftGrappleGun;
    [SerializeField] private GrappleGun rightGrappleGun;

    private Rigidbody rb;

    private Quaternion desiredBodyRotation;
    private float bodyRotationSpeed = 5f;

    private Vector3 targetBodyTurnLeft = new Vector3(5, 0, 20);
    private Vector3 targetBodyTurnRight = new Vector3(5, 0, -20);
    private Vector3 targetBodyTurnBoth = new Vector3(20, 0, 0);
    private bool leftGrappling = false;
    private bool rightGrappling = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }  

    // Update is called once per frame
    void Update()
    {
        //TurnBodyFromHooks();
        TurnBodyFromVelocity();
    }

    public void TurnBodyFromHooks()
    {
        leftGrappling = leftGrappleGun.IsGrappling;
        rightGrappling = rightGrappleGun.IsGrappling;

        if (leftGrappling)
        {
            if(rightGrappling)
            {
                //Both are out
                desiredBodyRotation = Quaternion.Euler(targetBodyTurnBoth);
            }
            else
            {
                //Only Left is out
                desiredBodyRotation = Quaternion.Euler(targetBodyTurnLeft);
            }
        }
        else
        {
            if (rightGrappling)
            {
                //Only Right is out
                desiredBodyRotation = Quaternion.Euler(targetBodyTurnRight);
            }
            else
            {
                //Neither are out
                desiredBodyRotation = Quaternion.Euler(Vector3.zero);
            }
        }

        bodyTransform.localRotation = Quaternion.Lerp(bodyTransform.localRotation, desiredBodyRotation, Time.deltaTime * bodyRotationSpeed);
    }

    public void TurnBodyFromVelocity()
    {
        Vector3 currVelocity = rb.velocity;
        Vector3 rotatedVelocity = Quaternion.Euler(0,-transform.rotation.eulerAngles.y,0) * currVelocity; //Rotate velocity vector to be relative to the player's facing direction

        //Debug.Log("rotation: " + transform.rotation.eulerAngles.y + ", velocity: " + currVelocity + ", rotatedVelocity: " + rotatedVelocity);

        float velX = rotatedVelocity.x * 2;
        float velZ = rotatedVelocity.z;

        velX = Mathf.Clamp(velX, -25, 25);
        velZ = Mathf.Clamp(velZ, -15, 15);

        Quaternion desiredRotation = Quaternion.Euler(velZ, 0, -velX);

        bodyTransform.localRotation = Quaternion.Lerp(bodyTransform.localRotation, desiredRotation, Time.deltaTime * bodyRotationSpeed);
    }
}
}