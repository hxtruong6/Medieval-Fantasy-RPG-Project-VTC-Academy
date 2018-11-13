using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonIdle : StateMachineBehaviour {
    private float timer;
    public float minTime;
    public float maxTime;
    public enum StateType { jump, attackR, attackL, attackC, attackB, attackICE}
    public StateType stateType;
    
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = Random.Range(minTime, maxTime);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timer <=0)
        {
            animator.SetTrigger(stateType.ToString());
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
