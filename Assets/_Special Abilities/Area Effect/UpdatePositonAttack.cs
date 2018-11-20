using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePositonAttack : StateMachineBehaviour
{
    //private Transform transformPos;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //transformPos = GameObject.FindGameObjectWithTag("Roll").GetComponent<Transform>();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Vector3 target = new Vector3(transformPos.position.x, animator.transform.position.y, transformPos.transform.position.z);
        //animator.transform.position = Vector3.MoveTowards(animator.transform.position, target, Time.deltaTime * 7);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.parent.position = animator.transform.position;
    }
}
