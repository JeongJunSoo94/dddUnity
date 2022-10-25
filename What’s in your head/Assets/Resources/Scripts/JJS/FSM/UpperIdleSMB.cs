using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JCW.UI.Options.InputBindings;
using JJS.CharacterSMB;
namespace JJS
{
    public class UpperIdleSMB : CharacterBaseSMB
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetLayerWeight(1, 0);
            if (!GetPlayerController(animator).characterState.top)
                GetPlayerController(animator).characterState.aim = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            check(animator);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}

        void check(Animator animator)
        {
            //if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            //{
            //    if (GetPlayerController(animator).characterState.top)
            //    {
            //        animator.SetBool("Aim", true);
            //        animator.SetBool("Top", true);
            //    }
            //}

            if (GetPlayerController(animator).playerMouse.weaponInfo[GetPlayerController(animator).playerMouse.GetUseWeapon()].canAim)
            {
                if (GetPlayerController(animator).characterState.top)
                {
                    animator.SetBool("Aim", true);
                    animator.SetBool("Top", true);
                    return;
                }

                if (GetPlayerController(animator).characterState.isMine)
                {
                    if (GetPlayerController(animator).characterState.CanJump)
                    {
                        animator.SetBool("Aim", KeyManager.Instance.GetKey(PlayerAction.Aim));
                    }
                }
            }
        }
    }
}
