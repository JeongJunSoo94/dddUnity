using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopViewMoveSMB : StateMachineBehaviour
{
    JJS.PlayerControllerWIYH player;
    JJS.CharacterController3D cC3D;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = animator.transform.gameObject.GetComponent<JJS.PlayerControllerWIYH>();
        cC3D = animator.transform.gameObject.GetComponent<JJS.CharacterController3D>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player.InputMove();
        player.InputRun();
        player.Rotation();
        animator.SetFloat("MoveX", cC3D.worldMoveDir.normalized.x* (player.isRun ? 2.0f:1.0f));
        animator.SetFloat("MoveZ", cC3D.worldMoveDir.normalized.z * (player.isRun ? 2.0f : 1.0f));
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
}
