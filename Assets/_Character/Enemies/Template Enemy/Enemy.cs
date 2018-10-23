using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(WeaponSystem))]

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float chaseRadius = 10f;
    [SerializeField] protected float fleeRadius = 4f;
    [SerializeField] protected WaypointContainer patrolPath;
    [SerializeField] protected float waypointTolerance = 2f;
    [SerializeField] protected float waitAtWaypointTime = 3f;
    [SerializeField] protected float radiusThreshold = 1f;
    [SerializeField] protected float angleAbleLooking = 30f;
    [SerializeField] protected float radiusAbleLooking = 10f;
    PlayerControl player;
    Character character;
    float distanceToPlayer;
    float currentWeaponRange;
    int nextWaypointIndex;

    public bool enableFleeing = false;
    enum State { idle, patrolling, attacking, chasing, fleeing }
    [SerializeField] State state = State.idle;

    private WeaponSystem weaponSystem;

    void Start()
    {
        character = GetComponent<Character>();
        player = GameObject.FindObjectOfType<PlayerControl>();
        weaponSystem = GetComponent<WeaponSystem>();//the weapon system dont change but the weapon may, depend on 
        currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
    }

    //    void Update()
    //    {
    //        PlayerOrEnemyAliveToContinue();
    //        if (distanceToPlayer > chaseRadius && state != State.patrolling)
    //        {
    //            StopAllCoroutines();
    //            weaponSystem.StopAttacking();
    //            StartCoroutine(Patrol());
    //        }
    //        if (enableChasing
    //            && distanceToPlayer <= chaseRadius
    //            && distanceToPlayer > fleeRadius
    //            && state != State.chasing)
    //        {
    //            StopAllCoroutines();
    //            weaponSystem.StopAttacking();
    //            StartCoroutine(ChasePlayer());
    //        }
    //
    //        if (enableFleeing
    //            && distanceToPlayer <= fleeRadius - radiusThreshold
    //            && state != State.fleeing)
    //        {
    //            StopAllCoroutines();
    //            weaponSystem.StopAttacking();
    //            StartCoroutine(FleePlayer());
    //        }
    //        if (distanceToPlayer <= currentWeaponRange && state != State.attacking)
    //        {
    //            state = State.attacking;
    //            StopAllCoroutines();
    //            weaponSystem.AttackTargetOnce(player.gameObject);
    //        }
    //    }

    public void UpdateDistanceToPlayer()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
    }

    public void PlayerOrEnemyAliveToContinue()
    {
        //player = GameObject.FindObjectOfType<PlayerControl>();
        if (player.GetComponent<HealthSystem>().healthAsPercentage <= 0 ||
            GetComponent<HealthSystem>().healthAsPercentage <= 0)
        {
            StopAllCoroutines();
            Destroy(this);//to stop enemies from continue moving even when died
        }
    }

    public void Patroling()
    {
        if (distanceToPlayer > chaseRadius && state != State.patrolling)
        {
            StopAllCoroutines();
            weaponSystem.StopAttacking();
            StartCoroutine(Patrol());
        }
    }

    public void Attacking()
    {
        if (distanceToPlayer <= currentWeaponRange && state != State.attacking)
        {
            state = State.attacking;
            StopAllCoroutines();
            weaponSystem.AttackTarget(player.gameObject);
        }
    }

    public void Chasing()
    {
        if (distanceToPlayer <= chaseRadius
            && ((enableFleeing && distanceToPlayer > fleeRadius + radiusThreshold) || !enableFleeing)  // if flee then chasing in range[fleeRadius + threshold, chasingRadius] else chasing
            && state != State.chasing)
        {
            StopAllCoroutines();
            weaponSystem.StopAttacking();
            StartCoroutine(ChasePlayer());
        }
    }

    public void Fleeing()
    {
        if (enableFleeing
            && distanceToPlayer <= fleeRadius - radiusThreshold
            && state != State.fleeing)
        {
            StopAllCoroutines();
            weaponSystem.StopAttacking();
            StartCoroutine(FleePlayer());
        }
    }

    IEnumerator Patrol()
    {
        state = State.patrolling;
        while (patrolPath != null)
        {
            Vector3 nextWaypointPos =
               patrolPath.transform.GetChild(nextWaypointIndex).position;
            character.SetDestination(nextWaypointPos);

            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            {
                yield return new WaitForSeconds(waitAtWaypointTime);
                // Choose next waypoint index, cycling to start if necessary
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
                character.SetDestination(nextWaypointPos);
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    IEnumerator ChasePlayer()
    {
        state = State.chasing;
        while (distanceToPlayer >= currentWeaponRange
               && distanceToPlayer <= chaseRadius
               && ((enableFleeing && distanceToPlayer > fleeRadius + radiusThreshold) || !enableFleeing))
        {
            character.SetDestination(player.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FleePlayer()
    {
        state = State.fleeing;
        Vector3 newPos = Vector3.zero;
        Vector3 dirToPlayer = Vector3.zero;

        while (distanceToPlayer <= fleeRadius)
        {
            dirToPlayer = transform.position - player.transform.position;
            newPos = transform.position + dirToPlayer;
            character.SetDestination(newPos);
            yield return new WaitForEndOfFrame();
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw attack sphere 
        Gizmos.color = new Color(255f, 0, 0, .5f);
        Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

        // Draw chase sphere 
        Gizmos.color = new Color(0, 0, 255, .5f);
        Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Draw flee sphere 
        Gizmos.color = new Color(0, 255, 255, .5f);
        Gizmos.DrawWireSphere(transform.position, fleeRadius);

        // Draw able looking
        Handles.DrawSolidArc(transform.forward, Vector3.up, transform.position, angleAbleLooking,  radiusAbleLooking);
    }
#endif
}
