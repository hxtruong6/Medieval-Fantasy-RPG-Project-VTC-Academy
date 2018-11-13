using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonAttack : StateMachineBehaviour {
    public float endTime;
    public enum AttackType { jump = 0, attackR = 1, attackL = 2, attackC = 3, attackB = 4, attackICE = 5, attackAOE = 6 }
    public AttackType attackType;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (endTime <= 0)
        {
            animator.SetTrigger("idle");
        }
        else
        {
            endTime -= Time.deltaTime;
        }
        DragonBoss.Attacking((int)attackType);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
