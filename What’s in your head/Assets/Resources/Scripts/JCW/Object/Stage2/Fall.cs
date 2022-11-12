using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KSU.Object.Stage2;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class Fall : LinkedObjectWithReciever
    {
        [Header("추락 초기 속도")] [SerializeField] float fallSpeed;
        [Header("추락 가속도")] [SerializeField] float fallAccelerateSpeed;
        [Header("바로 아래의 플랫폼")] [SerializeField] Transform groundPlatform;
        //Transform groundPlatform;

        Vector3 finalPos;
        PhotonView pv;
        private void Awake()
        {
            if (!groundPlatform)
            {
                Debug.Log("추락할 플랫폼이 지정되지 않았습니다.");
                Destroy(this.gameObject);
                return;
            }
            pv = PhotonView.Get(this);
            transform.GetChild(0).GetComponent<SandSackShadow>().groundPlatform = this.groundPlatform;
            finalPos = transform.position;
            finalPos.y = groundPlatform.position.y;
        }

        void Update()
        {
            if (!isActivated)
                return;
            transform.position = Vector3.MoveTowards(transform.position, finalPos, Time.deltaTime * fallSpeed);
            fallSpeed += Time.deltaTime * fallAccelerateSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            //Debug.Log(other.gameObject.layer);

            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                //Debug.Log("모래주머니 캐릭터 깔고 뭉갬");
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("Platform"))
            {
                //Debug.Log("모래주머니 땅에 추락");

                //현재는 추락 시 오브젝트 자체를 없애지만, 이펙트 실행하는 함수를 실행 후에 이펙트가 끝난 이후에 없애거나 SetActive(false)해야 할듯.

                //pv.RPC(nameof(CheckStopEffect), RpcTarget.AllViaServer);
                CheckStopEffect();

                //Destroy(this.gameObject);
            }
        }

        void CheckStopEffect()
        {
            Debug.Log("이펙트 재생");
            isActivated = false;
            GetComponent<CapsuleCollider>().enabled = false;
            transform.GetChild(2).gameObject.SetActive(true);
            StartCoroutine(nameof(StopEffect));
        }

        IEnumerator StopEffect()
        {
            yield return new WaitUntil(() => transform.GetChild(2).gameObject.GetComponent<ParticleSystem>().isStopped);
            Debug.Log("모래주머니 삭제");
            Destroy(this.gameObject);
        }
    }

}
