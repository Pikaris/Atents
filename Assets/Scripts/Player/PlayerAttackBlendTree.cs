using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackBlendTree : StateMachineBehaviour
{
    Player player;

    //private void OnEnable()
    //{
    //    player = FindAnyObjectByType<Player>();
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = player ?? GameManager.Instance.Player;
        player.RestoreSpeed();
    }
}
