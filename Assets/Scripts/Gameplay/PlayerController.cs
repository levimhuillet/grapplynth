using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth
{
    public class PlayerController : MonoBehaviour
    {
        private Rigidbody playerRB;
        [Header("Settings")]
        [SerializeField] private float gravityStrength;

        [SerializeField] private LayerMask whatIsGrapplable;
        public LayerMask WhatIsGrapplable => whatIsGrapplable;

        [SerializeField] private bool canHookThroughWalls;
        public bool CanHookThroughWalls => canHookThroughWalls;

        [SerializeField] private float maxGrappleDistance;
        public float MaxGrappleDistance => maxGrappleDistance;

        [Header("References")]
        [SerializeField] private GrappleGun leftGrapple;
        [SerializeField] private GrappleGun rightGrapple;
        [SerializeField] private CameraFollow cameraFollow;

        private bool touchingLeft;
        private bool touchingRight;
        int leftTouchFingerID;
        int rightTouchFingerID;

        // Start is called before the first frame update
        void Start()
        {
            playerRB = GetComponent<Rigidbody>();

            leftGrapple.LoadVarsFromPlayerController(this);
            rightGrapple.LoadVarsFromPlayerController(this);
            cameraFollow.LoadVarsFromPlayerController(this);

            touchingLeft = false;
            touchingRight = false;

            EventManager.OnNewLife.AddListener(HandleOnNewLife);

            GameManager.instance.SetMostRecentCorner(this.transform.position);
        }

        private void OnDestroy() {
            EventManager.OnNewLife.RemoveListener(HandleOnNewLife);
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.isEditor)
            {
                //On computer, use mouse input for grappling
                if (Input.GetMouseButtonDown(0))
                {
                    leftGrapple.StartGrappleMouse();
                }
                else
                {
                    if (Input.GetMouseButtonUp(0))
                        leftGrapple.EndGrapple();
                }

                if (Input.GetMouseButtonDown(1))
                {
                    rightGrapple.StartGrappleMouse();
                }
                else
                {
                    if (Input.GetMouseButtonUp(1))
                        rightGrapple.EndGrapple();
                }
            }
            else
            {
                //On phone (probably), use touch input
                CheckForNewTouches();

                //Check if any fingers have just been lifted up
                foreach (Touch touch in Input.touches)
                {
                    if ((touch.phase == TouchPhase.Ended) || (touch.phase == TouchPhase.Canceled)) //Check if any touches just ended
                    {
                        if (touchingLeft)
                        {
                            //Check if left was lifted
                            if (touch.fingerId == leftTouchFingerID)
                            {
                                //Left grapple finger was just lifted
                                leftGrapple.EndGrapple();
                                touchingLeft = false;
                                continue;
                            }
                        }

                        if (touchingRight)
                        {
                            if (touch.fingerId == rightTouchFingerID)
                            {
                                //Right grapple finger was just lifted
                                rightGrapple.EndGrapple();
                                touchingRight = false;
                                continue;
                            }
                        }

                    }
                }
            }

        }

        private void CheckForNewTouches()
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began) //Iterate over new touches
                {
                    if (touch.position.x <= (Screen.width/2))
                    {
                        //Left side of screen, try to launch left grapple

                        if (touchingLeft == false) //Able to shoot left grapple
                        {
                            leftGrapple.StartGrappleTouch(touch);
                            leftTouchFingerID = touch.fingerId;
                            touchingLeft = true;
                        }
                        else
                        {
                            if (touchingRight == false) //If left grapple is out, try to shoot right instead
                            {
                                rightGrapple.StartGrappleTouch(touch);
                                rightTouchFingerID = touch.fingerId;
                                touchingRight = true;
                            }
                        }
                    }
                    else
                    {
                        //Right side of screen, try to launch right grapple

                        if (touchingRight == false) //Try shooting right grapple first
                        {
                            rightGrapple.StartGrappleTouch(touch);
                            rightTouchFingerID = touch.fingerId;
                            touchingRight = true;
                        }
                        else
                        {
                            if (touchingLeft == false) //Right is out, try shooting left instead
                            {
                                leftGrapple.StartGrappleTouch(touch);
                                leftTouchFingerID = touch.fingerId;
                                touchingLeft = true;
                            }
                        }
                    }

                }
            }
        }

        private void FixedUpdate()
        {
            //Apply gravity
            playerRB.velocity -= new Vector3(0, gravityStrength, 0);
            // automatically kill if player falls below 1000 units
            if (playerRB.transform.position.y < -1000) {
                EventManager.OnGameOver.Invoke();
            }
        }

        // add velocity once
        public void OnCollisionEnter(Collision col)
        {
            switch (col.gameObject.layer) {
                // landing on spikes: game over
                case 11:
                    EventManager.OnGameOver.Invoke();
                    break;
                // landing on trampoline: bounce up
                case 13:
                    // set velocity components based on trampoline attributes
                    Trampoline trampoline = col.gameObject.GetComponent<Trampoline>();
                    Vector3 newVelocity = new Vector3(0,0,0);
                    newVelocity.x = (trampoline.strengthX * Mathf.Cos((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f)) + (trampoline.strengthZ * Mathf.Sin((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f));
                    if (Mathf.Abs(newVelocity.x) < 0.0001) {
                        newVelocity.x = playerRB.velocity.x;
                    }
                    newVelocity.y = (trampoline.strengthY != 0 ? trampoline.strengthY : playerRB.velocity.y);
                    newVelocity.z = (-trampoline.strengthX * Mathf.Sin((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f)) + (trampoline.strengthZ * Mathf.Cos((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f));
                    if (Mathf.Abs(newVelocity.z) < 0.0001) {
                        newVelocity.z = playerRB.velocity.z;
                    }
                    Debug.Log("Xvel: " + newVelocity.x + "   ZVel: " + newVelocity.z + "   Parent rotation: " + col.transform.parent.eulerAngles.y + " Parent rotation (radians): " + ((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f));
                    playerRB.velocity = newVelocity;
                    break;
            }
        }

        // add constant velocity
        public void OnCollisionStay(Collision col)
        {
            switch (col.gameObject.layer) {
                // landing on trampoline: bounce up
                case 13:
                    // set velocity components based on trampoline attributes
                    Trampoline trampoline = col.gameObject.GetComponent<Trampoline>();
                    if (trampoline.constant > 0) {
                        Vector3 newVelocity = new Vector3(0,0,0);
                        newVelocity.x = (trampoline.strengthX * Mathf.Cos((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f)) + (trampoline.strengthZ * Mathf.Sin((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f));
                        if (Mathf.Abs(newVelocity.x) < 0.0001) {
                            newVelocity.x = playerRB.velocity.x;
                        }
                        newVelocity.y = (trampoline.strengthY != 0 ? trampoline.strengthY : playerRB.velocity.y);
                        newVelocity.z = (-trampoline.strengthX * Mathf.Sin((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f)) + (trampoline.strengthZ * Mathf.Cos((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f));
                        if (Mathf.Abs(newVelocity.z) < 0.0001) {
                            newVelocity.z = playerRB.velocity.z;
                        }
                        //Debug.Log("Xvel: " + newVelocity.x + "   ZVel: " + newVelocity.z + "   Parent rotation: " + col.transform.parent.eulerAngles.y + " Parent rotation (radians): " + ((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f));
                        playerRB.velocity = newVelocity;
                    }
                    break;
            }
        }

        // acceleration
        public void OnTriggerStay(Collider col)
        {
            switch (col.gameObject.layer) {
                // landing on trampoline: bounce up
                case 13:
                    // set velocity components based on trampoline attributes
                    Trampoline trampoline = col.gameObject.GetComponent<Trampoline>();
                    if (trampoline.constant > 0) {
                        Vector3 newVelocity = new Vector3(0,0,0);
                        newVelocity.x = (trampoline.strengthX * Mathf.Cos((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f)) + (trampoline.strengthZ * Mathf.Sin((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f));
                        newVelocity.y = (trampoline.strengthY != 0 ? trampoline.strengthY : playerRB.velocity.y);
                        newVelocity.z = (-trampoline.strengthX * Mathf.Sin((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f)) + (trampoline.strengthZ * Mathf.Cos((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f));
                        //Debug.Log("Xvel: " + newVelocity.x + "   ZVel: " + newVelocity.z + "   Parent rotation: " + col.transform.parent.eulerAngles.y + " Parent rotation (radians): " + ((col.transform.parent.eulerAngles.y * Mathf.PI)/180.0f));
                        playerRB.velocity += newVelocity;
                    }
                    break;
            }
        }

        private void HandleOnNewLife() {
            this.transform.position = GameManager.instance.GetMostRecentCorner();
        }
    }
}