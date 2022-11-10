using Cinemachine;
using JCW.Object;
using JCW.UI.Options.InputBindings;
using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YC.CameraManager_;

namespace JCW.UI.InGame
{
    [RequireComponent(typeof(PhotonView))]
    public class CPR : MonoBehaviour
    {
        [Header("부활 게이지 이미지")] [SerializeField] Image heartGauge;
        [Header("부활 게이지 증가량")] [SerializeField] [Range(0f,0.05f)] float increaseValue = 0.005f;
        [Header("버튼 입력 시 증가량")] [SerializeField] [Range(0f,0.05f)] float addIncreaseValue = 0.02f;
        [Header("버튼 입력 시 재생될 비디오")] [SerializeField] VideoPlayer heartBeat;

        PhotonView photonView;
        bool isNella;
        GameObject curPlayer;
        Camera mainCam;

        Vector3 originalPos;

        RectTransform heartRect;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];
            curPlayer = transform.parent.parent.parent.parent.gameObject;
            if(photonView.IsMine)
                mainCam = isNella ? CameraManager.Instance.cameras[0] : CameraManager.Instance.cameras[1];
            originalPos = transform.GetChild(2).gameObject.GetComponent<RectTransform>().position;
            heartRect = transform.GetChild(2).gameObject.GetComponent<RectTransform>();
        }

        private void OnEnable()
        {
            // 여기서 카메라나 플레이어 움직임 막아야함
            if(photonView.IsMine)
            {                
                mainCam.GetComponent<CinemachineBrain>().enabled = false;
                curPlayer.GetComponent<PlayerState>().isOutOfControl = true;                
            }
            if (GameManager.Instance.isTopView)
            {
                // 블러와 검은 화면 꺼두기
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);

                // 
                Vector3 pos = originalPos;
                if (photonView.IsMine)
                    pos.x = isNella ? pos.x - 100f : pos.x + 100f;
                else
                    pos.x = isNella ? pos.x + 100f : pos.x - 100f;
                pos.y -= 300f;
                heartRect.position = pos;
                heartRect.localScale *= 0.7f;
            }
            // 아이템 UI
            transform.parent.parent.parent.GetChild(0).gameObject.SetActive(false);

        }

        private void OnDisable()
        {
            //여기서 카메라나 플레이어 움직임 켜야함.
            if (photonView.IsMine)
            {
                mainCam.GetComponent<CinemachineBrain>().enabled = true;
                curPlayer.GetComponent<PlayerState>().isOutOfControl = false;
            }

            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(true);

            transform.GetChild(2).gameObject.GetComponent<RectTransform>().position = originalPos;
            transform.GetChild(2).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
            heartRect.anchoredPosition = new Vector2(0, heartRect.anchoredPosition.y);

            // 아이템 UI
            transform.parent.parent.parent.GetChild(0).gameObject.SetActive(true);
        }


        void Update()
        {
            if (!photonView.IsMine)
                return;
            photonView.RPC(nameof(IncreaseValue), RpcTarget.AllViaServer, increaseValue * Time.deltaTime, isNella, false);
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Interaction))
                photonView.RPC(nameof(IncreaseValue), RpcTarget.AllViaServer, (float)addIncreaseValue, isNella, true);
        }

        [PunRPC]
        void IncreaseValue(float value, bool isNella, bool isPress)
        {
            heartGauge.fillAmount += value;
            if (isPress && !heartBeat.isPlaying)
                heartBeat.Play();
            if (heartGauge.fillAmount >= 1f)
            {
                GameManager.Instance.SetAliveState(isNella, true);
                heartGauge.fillAmount = 0f;
                GameManager.Instance.MediateRevive(false);
                if(!GameManager.Instance.isTopView && photonView.IsMine)
                    CameraManager.Instance.ReviveCam(isNella);
                curPlayer.GetComponent<Animator>().SetBool("isDead", false);
            }
        }

        public void Resurrect()
        {
            GameManager.Instance.curPlayerHP = 12;
            if (!File.Exists(Application.dataPath + "/Resources/CheckPointInfo/Stage" +
                GameManager.Instance.curStageIndex + "/Section" + GameManager.Instance.curSection + ".json"))
            {
                Debug.Log(GameManager.Instance.curSection);
                Debug.Log("체크포인트 불러오기 실패");
                return;
            }

            string jsonString = File.ReadAllText(Application.dataPath + "/Resources/CheckPointInfo/Stage" +
                GameManager.Instance.curStageIndex + "/Section" + GameManager.Instance.curSection + ".json");

            SavePosition.PlayerInfo data = JsonUtility.FromJson<SavePosition.PlayerInfo>(jsonString);
            curPlayer.transform.SetPositionAndRotation(new Vector3((float)data.position[0], (float)data.position[1], (float)data.position[2]), new Quaternion((float)data.rotation[0], (float)data.rotation[1], (float)data.rotation[2], (float)data.rotation[3]));
        }
    }
}

