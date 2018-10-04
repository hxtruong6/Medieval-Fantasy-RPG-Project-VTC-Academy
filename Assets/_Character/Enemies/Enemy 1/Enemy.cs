using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(WeaponSystem))]

public class Enemy : MonoBehaviour {

    [SerializeField] float chaseRadius = 6f;
    [SerializeField] WaypointContainer patrolPath;
    [SerializeField] float waypointTolerance = 2f;
    [SerializeField] float waitAtWaypointTime = 3f;
    PlayerControl player;
    Character character;
    float distanceToPlayer;
    float currentWeaponRange;
    int nextWaypointIndex;
    

    enum State { idle, patrolling, attacking, chasing }
    State state = State.idle;

    void Start ()
    {
        character = GetComponent<Character>();
        player = GameObject.FindObjectOfType<PlayerControl>();
    }
	
	void Update ()
    {
        if (player.GetComponent<HealthSystem>().healthAsPercentage <= 0 ||
            GetComponent<HealthSystem>().healthAsPercentage <= 0)
        {
            StopAllCoroutines();
            Destroy(this);//to stop enemies from continue moving even when died
        }

        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        WeaponSystem weaponSystem = GetComponent<WeaponSystem>();//the weapon system dont change but the weapon may, depend on designer
        currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();

        if (distanceToPlayer > chaseRadius && state != State.patrolling)
        {
            StopAllCoroutines();
            weaponSystem.StopAttacking();
            StartCoroutine(Patrol());
        }
        if (distanceToPlayer <= chaseRadius && state != State.chasing)
        {
            StopAllCoroutines();
            weaponSystem.StopAttacking();
            StartCoroutine(ChasePlayer());
        }
        if (distanceToPlayer <= currentWeaponRange && state != State.attacking)
        {
            state = State.attacking;
            StopAllCoroutines();
            weaponSystem.AttackTargetOnce(player.gameObject);
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
        while (distanceToPlayer >= currentWeaponRange)
        {
            character.SetDestination(player.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }

    void OnDrawGizmos()
    {
        // Draw attack sphere 
        Gizmos.color = new Color(255f, 0, 0, .5f);
        Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

        // Draw chase sphere 
        Gizmos.color = new Color(0, 0, 255, .5f);
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
