using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using YC.Camera_;

namespace JCW.UI.InGame
{
    public class TargetIndicator : MonoBehaviour
    {
        [Header("감지 범위 수치")] [SerializeField] [Range(0, 100)] float range;
        [Header("상호작용 가능 범위 수치")] [SerializeField] [Range(0, 100)] float interactableRange;
        [Header("UI")] [SerializeField] GameObject detectUI;
        [Header("타겟 오브젝트의 위치")] [SerializeField] Transform target;
        [Header("이미지 트랜스폼")] [SerializeField] RectTransform imgTransform;
        [Header("게이지")] [SerializeField] Image gauge;
        [Header("클립 플레이어")] [SerializeField] VideoPlayer videoPlayer;

        [Header("넬라 - 감지 & 상호작용 스프라이트 및 클립")]
        [SerializeField] Sprite nella_DetectSprite;
        [SerializeField] Sprite nella_InteractableSprite;
        [SerializeField] VideoClip nella_SetOnClip;
        [SerializeField] VideoClip nella_SetOffClip;
        [Header("스테디 - 감지 & 상호작용 스프라이트 및 클립")]
        [SerializeField] Sprite steady_DetectSprite;
        [SerializeField] Sprite steady_InteractableSprite;
        [SerializeField] VideoClip steady_SetOnClip;
        [SerializeField] VideoClip steady_SetOffClip;

        private RectTransform canvasSize;
        private Camera mainCamera;

        Image interactiveSprite;


        // 현재 플레이어가 감지되는 지
        bool isActive = false;
        bool isInteractable = false;

        // 현재 캐릭터가 넬라인지
        bool isNella;


        Rect screenSize;
        // 화면에서 벗어나지 못하게 하는 오프셋 값
        private float screenLimitOffset;

        // 화면 안에 있을 때와 벗어났을 때의 이미지 크기
        Vector3 initImgScale;
        Vector3 outOfSightImgScale;



        private void Awake()
        {
            canvasSize = detectUI.GetComponent<RectTransform>();
            transform.localScale = new Vector3(range, range, range);
            screenLimitOffset = imgTransform.rect.width * 0.4f;
            outOfSightImgScale = imgTransform.localScale * 0.8f;
            initImgScale = imgTransform.localScale;
            interactiveSprite = imgTransform.gameObject.GetComponent<Image>();

            // 정식으로 사용할 때엔 아래 코드 쓸것
            //isNella = GameManager.Instance.characterOwner[PhotonNetwork.IsMasterClient];

            // 임시
            isNella = false;
        }
        protected void Update()
        {
            if (!isActive)
                return;

            // 타겟의 위치를 메인카메라의 스크린 좌표로 변경
            Vector3 indicatorPosition = mainCamera.WorldToScreenPoint(target.position);


            // 타겟이 카메라 앞에 있을 때
            if (indicatorPosition.z >= 0f)
            {
                // 타겟이 화면 안에 들어올 때
                if (indicatorPosition.x <= screenSize.x + screenSize.width && indicatorPosition.x >= screenSize.x
                   && indicatorPosition.y <= screenSize.y + screenSize.height && indicatorPosition.y >= screenSize.y)
                {
                    imgTransform.localScale = initImgScale;
                    indicatorPosition.z = 0f;
                }
                else
                    indicatorPosition = OutOfRange(indicatorPosition);
            }
            else
            {
                // 화면 뒤에 있을 때, 위치 뒤집어지는 것을 막기 위한 설정
                indicatorPosition *= -1f;
                indicatorPosition = OutOfRange(indicatorPosition);
            }

            imgTransform.position = indicatorPosition;
        }



        // 카메라 범위를 벗어났을 때를 위한 설정
        protected Vector3 OutOfRange(Vector3 indicatorPosition)
        {
            imgTransform.localScale = outOfSightImgScale;
            indicatorPosition.z = 0f;

            // 현재 카메라 화면의 중심 위치 잡기
            Vector3 canvasCenter = new Vector3((screenSize.x + screenSize.width / 2f), (screenSize.y + screenSize.height / 2f), 0f);

            // UI 위치-> 화면 중심 벡터
            indicatorPosition -= canvasCenter;

            // 화면 범위 제한
            float divX = (screenSize.width / 2f - screenLimitOffset) / Mathf.Abs(indicatorPosition.x);
            float divY = (screenSize.height / 2f - screenLimitOffset) / Mathf.Abs(indicatorPosition.y);

            // x 좌표가 먼저 테두리에 닿았을 때
            if (divX < divY)
            {
                // x축에 대한 각도
                float angle = Vector3.SignedAngle(Vector3.right, indicatorPosition, Vector3.forward);
                indicatorPosition.x = Mathf.Sign(indicatorPosition.x) * (screenSize.width / 2f - screenLimitOffset);
                indicatorPosition.y = Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.x;
            }
            else
            {
                // y축에 대한 각도
                float angle = Vector3.SignedAngle(Vector3.up, indicatorPosition, Vector3.forward);

                indicatorPosition.y = Mathf.Sign(indicatorPosition.y) * (screenSize.height / 2f - screenLimitOffset);
                indicatorPosition.x = -Mathf.Tan(Mathf.Deg2Rad * angle) * indicatorPosition.y;
            }

            // 원상복귀
            indicatorPosition += canvasCenter;
            return indicatorPosition;
        }


        // 지금은 임시로 트리거 Enter/Exit으로 하고 있지만
        // 정식으로 사용할 때엔 플레이어가 레이를 쏴서 거리에 따라 온 오프 시켜야함.
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                // 자기꺼일때만 켜기
                if (other.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // 여기서는 바로 켜주는걸로 되어있지만, 정식으로 사용할 때엔 플레이어가 레이를 쏘도록 함수를 써야할 듯.
                    // other.gameObject.SendMessage("레이 쏘는 함수", 매개변수-오브젝트);                    
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Nella") || other.CompareTag("Steady"))
            {
                // 자기꺼일때만 켜기
                if (other.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    // 여기서는 바로 켜주는걸로 되어있지만, 정식으로 사용할 때엔 플레이어 레이 리스트에서 지우도록.
                    // other.gameObject.SendMessage("레이 쏘는 함수", 매개변수-오브젝트);    
                }
            }
        }

        public void SetUI(bool _isActive, bool _isSetOn, float _dist, Camera _cam)
        {
            detectUI.SetActive(_isActive);
            isActive = _isActive;

            if (isActive)
            {
                videoPlayer.targetCamera = _cam;
                mainCamera = videoPlayer.targetCamera;
                SetSreenInfo();
                // 상호작용 범위 밖->안 & 안->밖 들어갔을 때만 애니메이션 재생과 함께 스프라이트 변경
                if (isInteractable != _isSetOn)
                {
                    isInteractable = _isSetOn;
                    ConvertVideo(_isSetOn);
                }
                
                if(!isInteractable)
                {
                    // 거리에 따라 게이지 줄어들게 끔 해주기
                    // 1 - (_dist-상호작용 범위)/(감지범위 - 상호작용 범위) == FillValue에 넣어줌.
                    gauge.fillAmount = 1 - (_dist - interactableRange) / (range - interactableRange);
                }
            }
        }

        public void ConvertVideo(bool _isSetOn)
        {
            // 이미지를 잠깐 꺼주고 동영상 켜주기
            interactiveSprite.enabled = false;
            videoPlayer.Stop();
            StopCoroutine(nameof(PlayVideoClip));
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = true;
            if (isNella)
                videoPlayer.clip = _isSetOn ? nella_SetOnClip : nella_SetOffClip;
            else
                videoPlayer.clip = _isSetOn ? steady_SetOnClip : steady_SetOffClip;

            StartCoroutine(nameof(PlayVideoClip), _isSetOn);
        }

        IEnumerator PlayVideoClip(bool _isSetOn)
        {
            videoPlayer.Play();
            yield return new WaitForSeconds(1f);
            videoPlayer.Stop();
            if (isNella)
                interactiveSprite.sprite = _isSetOn ? nella_InteractableSprite : nella_DetectSprite;
            else
                interactiveSprite.sprite = _isSetOn ? steady_InteractableSprite : steady_DetectSprite;

            // 동영상 끄고 이미지 켜주기
            videoPlayer.gameObject.GetComponent<RawImage>().enabled = false;
            interactiveSprite.enabled = true;


            yield return null;
        }

        void SetSreenInfo()
        {
            Rect cameraPos = mainCamera.rect;
            screenSize = new(canvasSize.rect.width * cameraPos.x,
                             canvasSize.rect.height * cameraPos.y,
                             canvasSize.rect.width * cameraPos.width,
                             canvasSize.rect.height * cameraPos.height);
        }

    }
}
