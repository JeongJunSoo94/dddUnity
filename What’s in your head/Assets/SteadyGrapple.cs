using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KSU
{
    public class SteadyGrapple : MonoBehaviour
    {
        public SteadyGrappleAction player;
        public GameObject spawner;
        LineRenderer grappleRope;
        Vector3 endPosistion;

        public float departingOffset = 0.2f;
        public bool isEndPosition = false;
        public bool isSucceeded = false;

        public float moveSpeed = 15f;
        Rigidbody grappleRigidbody;
        public float horizonInertiaSpeed;
        public Vector3 horizonInertiaVec;
        public Vector3 fallingVelocity;

        public float delayDeactivationTime = 1f;

        Vector3 noAimRot = new Vector3(-7.631f, 181.991f, 91.828f);

        // Start is called before the first frame update
        void Start()
        {
            grappleRigidbody = GetComponent<Rigidbody>();
            grappleRope = GetComponent<LineRenderer>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(!isSucceeded)
            {
                if (isEndPosition)
                {
                    FallToGround();

                }
                else
                {
                    MoveToEndPosition();
                }
            }
            MakeRope();
        }

        public void InitGrapple(Vector3 startPos, Vector3 endPos)
        {
            transform.position = startPos;
            endPosistion = endPos;
            isEndPosition = false;
            isSucceeded = false;
        }

        void MoveToEndPosition()
        {
            //1.리지드바디 추가해서 벨로시티로움직이기
            if (Vector3.Distance(endPosistion, transform.position) < departingOffset)
            {
                isEndPosition = true;
                horizonInertiaVec = grappleRigidbody.velocity;
                horizonInertiaVec.y = 0;
                horizonInertiaSpeed = horizonInertiaVec.magnitude;
                StartCoroutine(nameof(DelayDeactivation));
            }
            else
            {
                isEndPosition = false;
            }
            transform.LookAt(endPosistion);
            Vector3 dir = (endPosistion - transform.position).normalized;
            grappleRigidbody.velocity = dir * moveSpeed;
        }

        void FallToGround()
        {
            horizonInertiaSpeed -= moveSpeed * Time.fixedDeltaTime;
            if (horizonInertiaSpeed < 0)
                horizonInertiaSpeed = 0f;
            fallingVelocity = horizonInertiaVec * horizonInertiaSpeed;
            fallingVelocity.y = grappleRigidbody.velocity.y - 9.8f * Time.fixedDeltaTime; 
        }

        void MakeRope()
        {
            grappleRope.SetPosition(0, spawner.transform.position);
            grappleRope.SetPosition(1, transform.position);
        }

        IEnumerator DelayDeactivation()
        {
            yield return new WaitForSeconds(delayDeactivationTime);
            if (this.gameObject.activeSelf)
            {
                player.RecieveGrappleInfo(false, null);
            }
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            // 이거 불렛처럼 트리거로 콜라이더 해야하고 그래플드 오브젝트의 디텍팅 트리거 태그는 그래플드 오브젝트와 구분해줘야함

            if((other.gameObject.layer != LayerMask.NameToLayer("UITriggers")) && (other.gameObject.layer != LayerMask.NameToLayer("Player")) && (other.gameObject.layer != LayerMask.NameToLayer("Bullet")))
            {
                if(other.CompareTag("GrappledObject"))
                {
                    player.RecieveGrappleInfo(true, other.gameObject);
                    isSucceeded = true;
                    grappleRigidbody.velocity = Vector3.zero;
                    StopCoroutine(nameof(DelayDeactivation));
                }
                else
                {
                    player.RecieveGrappleInfo(false, null);
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}

