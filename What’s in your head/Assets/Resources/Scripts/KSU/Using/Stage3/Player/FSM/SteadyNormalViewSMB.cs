using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class SteadyNormalViewSMB : CharacterBaseSMB
    {
        
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool("wasAirJump", false);
            animator.SetBool("isJump", false);
            animator.SetBool("isAttack", false);
            //GetPlayerController(animator).characterState.aim = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!GetPlayerController(animator).characterState.swap)
            {
                WeaponCheck(animator);
                GetPlayerController(animator).playerMouse.ik.enableIK = false;
            }
            if (GetPlayerController(animator).characterState.isMine)
            {
                InputCheck(animator);
            }
            check(animator);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetPlayerController(animator).MoveStop();
        }
        void WeaponCheck(Animator animator)
        {
            if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            {
                if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canNoAimAttack)
                {
                    animator.SetLayerWeight(1, 1);
                }
                else
                {
                    if (animator.GetLayerWeight(1) == 0 && GetPlayerController(animator).characterState.aim)
                    {
                        animator.SetLayerWeight(1, 1);
                    }
                    else
                    {
                        animator.SetLayerWeight(1, 0);
                    }
                }
            }
            else
            {
                if (animator.GetLayerWeight(1) == 1)
                {
                    animator.SetLayerWeight(1, 0);
                }
            }
        }

        void InputCheck(Animator animator)
        {
            //// << 디펜스 모드 입장시 스테이트 전환 YC
            //if (GameManager.Instance.isTopView)
            //{
            //    GetPlayerController(animator).characterState.top = !GetPlayerController(animator).characterState.top;
            //}

            GetPlayerController(animator).InputRun();
            GetPlayerController(animator).InputMove();
            GetPlayerController(animator).InputJump();
            if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim) /////////// 3스테이지 전용 코드
            {
                if (!animator.GetBool("isShootingGrapple"))
                {
                    GetPlayerController(animator).InputDash();
                }
            }
            else
            {
                GetPlayerController(animator).InputDash();
            }

            if (KeyManager.Instance.GetKey(PlayerAction.Fire))
            {
                if (!animator.GetBool("WeaponSwap"))
                {
                    if (!GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
                    {
                        if (!GetPlayerController(animator).characterState.IsDashing)
                        {
                            animator.SetBool("isAttackNext", true);
                            animator.SetBool("isAttack", true);
                        }
                    }
                }
            }
            if (KeyManager.Instance.GetKeyDown(PlayerAction.Swap) && !GetPlayerController(animator).characterState.swap)
            {
                if (!animator.GetBool("isAttack"))
                {
                    GetPlayerController(animator).characterState.swap = true;
                    animator.SetBool("WeaponSwap", true);
                }
            }

            //if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim && !GetPlayerController(animator).characterState.top && KeyManager.Instance.GetKey(PlayerAction.Aim))
            //{
            //    animator.SetBool("Aim", true);
            //}
        }

        void check(Animator animator)
        {
            if (GetPlayerController(animator).characterState.top)
            {
                Cursor.lockState = CursorLockMode.Confined;
                GetPlayerController(animator).playerMouse.TopViewUpdate();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            animator.SetFloat("HorizonVelocity", (GetPlayerController(animator).characterState.isMove ? (GetPlayerController(animator).characterState.isRun ? 1.0f : 0.5f) : 0.0f));

            if (!GetPlayerController(animator).characterState.IsGrounded)
            {
                animator.SetBool("isAir", true);
                if (!GetPlayerController(animator).characterState.IsJumping)
                {
                    animator.SetBool("JumpDown",true);
                    return;
                }
            }
            else
            {
                animator.SetBool("isAir", false);
            }


            if (!GetPlayerController(animator).characterState.isMove)
            {
                GetPlayerController(animator).characterState.isRun = false;
            }
            if (GetPlayerController(animator).characterState.IsJumping)
            {
                animator.SetBool("isJump", true);
                return;
            }

            if (GetPlayerController(animator).characterState.IsDashing)
            {
                animator.SetBool("isDash", true);
                return;
            }




        }
    }

}
