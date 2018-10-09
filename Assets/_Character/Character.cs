using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum CharacterState { idling, attacking, running }

[RequireComponent(typeof(NavMeshAgent))]

public class Character : MonoBehaviour
{
    [SerializeField] float moveSpeedMultiplier = 0.7f;
    [SerializeField] float movingTurnSpeed = 360;
    [SerializeField] float stationaryTurnSpeed = 180;
    [SerializeField] float moveThreshold = 1f;
    [SerializeField] float animationSpeedMultiplier = 1.5f;
    [SerializeField] AnimatorOverrideController animatorOverrideController;
    [SerializeField] float baseDamage = 10f;

    Vector3 clickPoint;
    NavMeshAgent agent;
    Animator animator;
    Rigidbody myRigidBody;
    float turnAmount;
    float forwardAmount;
    bool isAlive = true;
    CharacterState currentState = CharacterState.idling;

    public CharacterState CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    public float GetBaseDamage()
    {
        return baseDamage;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = true;

        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.constraints = RigidbodyConstraints.FreezeRotation;

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = true;
    }

    void Update()
    {
        if (agent.remainingDistance > agent.stoppingDistance && isAlive)
        {
            Move(agent.desiredVelocity);
        }
        else
        {
            Move(Vector3.zero);
        }
    }

    public void SetDestination(Vector3 worldPosition)
    {
        agent.destination = worldPosition;
    }

    public float GetAnimationSpeedMultiplier()
    {
        return animationSpeedMultiplier;
    }

    public AnimatorOverrideController GetOverrideController()
    {
        return animatorOverrideController;
    }

    void Move(Vector3 movement)
    {
        SetForwardAndTurn(movement);
        ApplyExtraTurnRotation();
        UpdateAnimator(movement);
    }

    public void Kill()
    {
        isAlive = false;
    }

    void SetForwardAndTurn(Vector3 movement)
    {
        if (movement.magnitude > moveThreshold)
        {
            movement.Normalize();
        }
        var localMove = transform.InverseTransformDirection(movement);
        turnAmount = Mathf.Atan2(localMove.x, localMove.z);
        forwardAmount = localMove.z;
    }

    void UpdateAnimator(Vector3 move)
    {
        animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
        animator.speed = animationSpeedMultiplier;
    }

    void ApplyExtraTurnRotation()
    {
        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
    }

    void OnAnimatorMove()
    {
        if (Time.deltaTime > 0)
        {
            Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

            velocity.y = myRigidBody.velocity.y;
            myRigidBody.velocity = velocity;
        }
    }
}
