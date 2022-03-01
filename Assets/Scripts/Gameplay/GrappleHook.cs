using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class GrappleHook : MonoBehaviour {
        private SpringJoint joint;
        [SerializeField] private Transform playerTransform, guntipTransform;
        private Vector3 grapplePoint;

        private LayerMask whatIsGrapplable;
        public void SetWhatIsGrapplable(LayerMask layerMask) { whatIsGrapplable = layerMask; }

        [System.Serializable]
        private enum GrappleHookState {
            shooting,
            onwall,
            retracting,
            idle
        }
        private GrappleHookState hookState = GrappleHookState.idle;

        private Vector3 targetPos;

        private float shootSpeed = 14f;
        private float maxSpinSpeed = 720f;
        private float currSpinSpeed = 0;
        private float maxDistance;

        private bool canGrappleThroughWalls;

        private Transform parentTransform;

        // Start is called before the first frame update
        void Start() {
            parentTransform = transform.parent;
        }

        public void LoadVarsFromPlayerController(Grapplynth.PlayerController playerController) {
            canGrappleThroughWalls = playerController.CanHookThroughWalls;
            whatIsGrapplable = playerController.WhatIsGrapplable;
        }

        void Update() {
            if (hookState == GrappleHookState.idle || hookState == GrappleHookState.onwall)
                return;

            if (Vector3.Distance(transform.position, targetPos) < 0.1f) {
                //Close enough
                switch (hookState) {
                    case GrappleHookState.shooting: {
                            hookState = GrappleHookState.onwall;
                            CreateJoint();
                            break;
                        }

                    case GrappleHookState.retracting: {
                            hookState = GrappleHookState.idle;
                            ResetHook();
                            break;
                        }
                }
            }
            else {
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * shootSpeed);
            }

            transform.Rotate(0, 0, currSpinSpeed * Time.deltaTime);
        }


        /*private void OnCollisionEnter(Collision collision)
        {
            if ((onWall == false) && (retracting == false))
            {
                HitWall();
                onWall = true;
            }
        }*/

        public void ShootHook(Vector3 targetPosition) {
            transform.parent = null;
            currSpinSpeed = maxSpinSpeed;

            hookState = GrappleHookState.shooting;

            //Find where to move towards
            RaycastHit hitInfo;
            if ((!canGrappleThroughWalls) && (Physics.Linecast(guntipTransform.position, targetPosition, out hitInfo, whatIsGrapplable))) {
                grapplePoint = hitInfo.point;
            }
            else {
                //This *shouldn't* be called, but just in case
                grapplePoint = targetPosition;
            }

            if (grapplePoint == null)
                return;

            targetPos = grapplePoint;

            transform.LookAt(targetPosition);
        }

        public void CreateJoint() {
            currSpinSpeed = 0;

            joint = playerTransform.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;

            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(playerTransform.position, grapplePoint);

            joint.maxDistance = 0; //distanceFromPoint * 0.5f;
            joint.minDistance = 0;

            joint.spring = 15;
            joint.damper = 7f;
            joint.massScale = 4.5f;
        }

        public void RetractHook() {
            currSpinSpeed = -maxSpinSpeed;

            hookState = GrappleHookState.retracting;
            targetPos = parentTransform.position;
            Destroy(joint);
        }

        public void ResetHook() {
            currSpinSpeed = 0;

            transform.parent = parentTransform;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }
    }
}