using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;

namespace KSU
{
    public class RopeAction : MonoBehaviour
    {
        Camera mainCamera;
        PlayerController3D playerController;
        CharacterState3D playerState;
        PlayerInteractionState interactionState;

        public GameObject minDistRope;

        public float escapingRopeSpeed = 6f;
        public float escapingRopeDelayTime = 1f;

        public void RideRope()
        {
            playerState.IsAirJumping = false;
            interactionState.isRidingRope = true;
            interactionState.isMoveToRope = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            minDistRope.GetComponentInChildren<RopeSpawner>().StartRopeAction(this.gameObject);
        }
        public void EscapeRope()
        {
            StartCoroutine("DelayEscape");
            float jumpPower = minDistRope.GetComponentInChildren<RopeSpawner>().EndRopeAction(this.gameObject);
            Vector3 inertiaVec = mainCamera.transform.forward;
            inertiaVec.y = 0;

            transform.LookAt(transform.position + inertiaVec);
            playerController.MakeinertiaVec(escapingRopeSpeed, inertiaVec.normalized);
            playerController.moveVec = Vector3.up * playerController.jumpSpeed * jumpPower;
            playerController.enabled = true;
        }


        IEnumerator DelayEscape()
        {
            interactionState.isMoveFromRope = true;
            yield return new WaitForSeconds(escapingRopeDelayTime);
            interactionState.isMoveFromRope = false;
            interactionState.isRidingRope = false;
        }
    }
}
