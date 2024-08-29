using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsLandingEndedTwo : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Reset the landing flag when the landing animation ends
        PlayerControls playerControls = animator.GetComponentInParent<PlayerControls>();
        if (playerControls != null)
        {
            playerControls.OnLandingComplete();
        }
    }
}
