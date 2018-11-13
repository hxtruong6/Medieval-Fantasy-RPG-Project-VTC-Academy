using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonJump : StateMachineBehaviour {
    private float timer;
    public float minTime;
    public float maxTime;
    public float speed;
    private Transform playerPos;
    

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        timer = Random.Range(minTime, maxTime);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (timer <= 0)
        {
            animator.SetTrigger("idle");
        }
        else
        {
            timer -= Time.deltaTime;
        }
        Vector3 target = new Vector3(playerPos.transform.position.x- 4, animator.transform.position.y, playerPos.transform.position.z -4);
        animator.transform.position = Vector3.MoveTowards(animator.transform.position, target, speed * Time.deltaTime*5);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
