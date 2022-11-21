using JCW.AudioCtrl;
using JCW.Dialog;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JCW.Object
{
    [RequireComponent(typeof(PhotonView))]
    public class StartPoint : MonoBehaviour
    {
        [Header("탑뷰 세팅")] [SerializeField] bool isTopView = false;
        [Header("사이드뷰 세팅")] [SerializeField] bool isSideView = false;
        StringBuilder currentStageSection;
        StringBuilder dialogString;
        PhotonView pv;

        private void Awake()
        {
            if (GameManager.Instance == null)
            {
                Debug.Log("게임 매니저가 없으므로 중단");
                return;
            }

            pv = PhotonView.Get(this);

            // 현재 스테이지와 섹션 가져오기
            currentStageSection = new(10, 10);
            currentStageSection.Append("S");
            currentStageSection.Append(GameManager.Instance.curStageIndex);
            currentStageSection.Append("S");
            currentStageSection.Append(GameManager.Instance.curStageType);
            Debug.Log("현재 스테이지와 섹션 : " + currentStageSection.ToString());

            dialogString = new(10, 10);
            dialogString.Append(currentStageSection);
            dialogString.Append("_N");
            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.SetDialogs(dialogString.ToString());
                dialogString.Replace("_N", "_E", 4, 2);
                DialogManager.Instance.SetDialogs(dialogString.ToString());
                dialogString.Replace("_E", "_S", 4, 2);
                DialogManager.Instance.SetDialogs(dialogString.ToString());
            }

            StartCoroutine(nameof(WaitForPlayer));
        }

        IEnumerator WaitForPlayer()
        {
            //yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene(true) && GameManager.Instance.GetCharOnScene(false));
            yield return new WaitUntil(() => GameManager.Instance.isCharOnScene.Count == 2);
            yield return new WaitUntil(() => GameManager.Instance.GetCharOnScene(true) && GameManager.Instance.GetCharOnScene(false));
            if (PhotonNetwork.IsMasterClient)
            {
                SoundManager.Instance.PlayBGM_RPC(currentStageSection.ToString());
                pv.RPC(nameof(SetView), RpcTarget.AllViaServer);
            }


            yield break;
        }

        [PunRPC]
        void SetView()
        {
            if (isTopView)
                GameManager.Instance.isTopView = isTopView;
            else if (isSideView)
                GameManager.Instance.isSideView = isSideView;
        }
    }

}
