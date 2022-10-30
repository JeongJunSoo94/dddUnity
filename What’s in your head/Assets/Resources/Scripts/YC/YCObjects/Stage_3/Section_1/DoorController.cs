﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


/// <summary> 
/// 
/// Current Issue       : 
/// 
/// Object Name         : 나무 출입문
/// 
/// Object Description  : 나무 버튼이 눌리면 해당 오브젝테에게 SendMessage를 보내 출입문이 열리게 된다
/// 
/// Script Description  : 
///                         
/// Player Intraction   : 
/// 
/// </summary>

namespace YC_OBJ
{
    public class DoorController : MonoBehaviour
    {
        [Header("<기획 편집 사항>")]

        [Header("[애니메이션 속도 (0 ~ 1.5 추천)]")]
        [SerializeField] float animationSpeed = 0.3f;

        Animator animator;
        PhotonView pv;

        private void Awake()
        {
            animator = this.gameObject.GetComponent<Animator>();

            animator.speed = animationSpeed;

            pv = this.gameObject.GetComponent<PhotonView>();
        }

        private void Start() { }

        public void SetOpen(bool _isOpen)
        {
            if(pv.IsMine)
                animator.SetBool("isOpen", _isOpen);
        }     
    }
}