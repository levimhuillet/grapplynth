using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class GrappleGun : MonoBehaviour {
        private LineRenderer lr;
        private Vector3 grapplePoint;
        [SerializeField] private Transform shootPoint, grappleHookRopeTransform, cameraTransform, playerTransform;
        [SerializeField] private float maxDistance;
        [SerializeField] private GrappleHook grappleHook;

        private LayerMask whatIsGrapplable;

        private bool hookIsOut = false;

        // Start is called before the first frame update
        void Start() {
            lr = GetComponent<LineRenderer>();
        }

        public void LoadVarsFromPlayerController(Grapplynth.PlayerController playerController) {
            whatIsGrapplable = playerController.WhatIsGrapplable;
            maxDistance = playerController.MaxGrappleDistance;
            grappleHook.LoadVarsFromPlayerController(playerController);
        }

        private void LateUpdate() {
            //if (joint != null)
            DrawRope();
        }

        public void StartGrappleMouse() {
            RaycastHit hit;

            Vector3 pos = Input.mousePosition;
            pos.z = 0;
            Ray ray = Camera.main.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit, maxDistance, whatIsGrapplable)) {
                hookIsOut = true;

                grapplePoint = hit.point;

                grappleHook.ShootHook(grapplePoint);

                lr.positionCount = 2;
            }
        }

        public void StartGrappleTouch(Touch touch) {
            RaycastHit hit;

            Vector3 pos = touch.position;
            pos.z = 0;
            Ray ray = Camera.main.ScreenPointToRay(pos);

            if (Physics.Raycast(ray, out hit, maxDistance, whatIsGrapplable)) {
                hookIsOut = true;

                grapplePoint = hit.point;

                grappleHook.ShootHook(grapplePoint);

                lr.positionCount = 2;
            }
        }

        private void DrawRope() {
            lr.SetPosition(0, shootPoint.position);
            lr.SetPosition(1, grappleHookRopeTransform.position);
        }

        public void EndGrapple() {
            grappleHook.RetractHook();

            //lr.positionCount = 0;

            hookIsOut = false;
        }

        public bool IsGrappling => hookIsOut;

        public Vector3 GetGrapplePoint => grapplePoint;
    }
}