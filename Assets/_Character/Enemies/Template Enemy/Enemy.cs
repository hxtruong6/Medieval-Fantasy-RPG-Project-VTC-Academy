#if UNITY_EDITOR
using System;
using System.Collections;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(WeaponSystem))]

public class Enemy : MonoBehaviour
{
    public bool seeking;
    public bool fleeing;

    public float seekingSpeed = 8f;
    //    public float expandSeekingRadius = 4f; // if enemy is chasing player, seekingRadius will expand the radius until player is be out of range 
    public float fleeingSpeed = 8f;
    public float patrolSpeed = 0.5f;


    public float seekingRadius = 8f; // if player is out of this distance, the enemy doesn't seek
    public float fleeingRadius = 5f;
    public float patrollRadius = 5f;
    public float chasingRadius = 10f; // to chasing player after already attacking player

    public float idleTimeLimited = 5f;
    public float chasingTimeLimited = 10f;
    public float patrolTimeLimited = 8f;

    public WaypointContainer patrolPath;
    public float waypointTolerance = 1f;
    public float waitAtWaypointTime = 3f;
    public float angleAbleLooking = 30f;
    public float radiusAbleLooking = 10f;

    protected PlayerControl player;
    protected Character character;

    protected float idleTime = 0f;
    protected float chasingTime = 0f;
    protected float patrolTime = 0f;
    protected float currentWeaponRange;
    protected int nextWaypointIndex;
    protected WeaponSystem weaponSystem;
    protected Vector3 nextWaypointPos;


    Rigidbody rigid;
    NavMeshAgent agent;
    float distanceToPlayer;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = true;
        character = GetComponent<Character>();
        player = FindObjectOfType<PlayerControl>();
        weaponSystem = GetComponent<WeaponSystem>();//the weapon system dont change but the weapon may, depend on 
        currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
    
        //StartCoroutine(getCurrentWeaponRange());

        //if (fleeing && fleeingRadius < currentWeaponRange)
        //{
        //    fleeingRadius = currentWeaponRange;
        //}
    }

    public void UpdateCurrentWeaponRange()
    {
        currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
    }

    private IEnumerator getCurrentWeaponRange()
    {
        yield return new WaitUntil(() => weaponSystem.GetCurrentWeapon() != null);
        currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
    }

    public Vector3 GetHeading()
    {
        return rigid.velocity;
    }

    public float GetDistanceToPlayer()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    public void PlayerOrEnemyAliveToContinue()
    {
        //player = GameObject.FindObjectOfType<PlayerControl>();
        if (player.GetComponent<HealthSystem>().healthAsPercentage <= 0 ||
            GetComponent<HealthSystem>().healthAsPercentage <= 0)
        {
            Destroy(this);//to stop enemies from continue moving even when died
        }
    }
    bool CheckTargetInLooking()
    {
        Vector3 vecToTarget = player.transform.position - transform.position;
        var flatVecToTarget = Vector3.ProjectOnPlane(vecToTarget, Vector3.up);
        var flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        var angle = Vector3.Angle(flatVecToTarget, flatForward);
        if (angle < angleAbleLooking / 2 && Vector3.Distance(transform.position, player.transform.position) < radiusAbleLooking)
        {
            return true;
        }

        return false;
    }
    void FixedUpdate()
    {
        PlayerOrEnemyAliveToContinue();
        var steering = Vector3.zero;
        distanceToPlayer = GetDistanceToPlayer();
        switch (character.CurrentState)
        {
            case CharacterState.attacking:
                {
                    if (fleeing && distanceToPlayer < fleeingRadius)
                    {
                        //TODO: act finish animation
                        character.CurrentState = CharacterState.fleeing;
                    }

                    else if (distanceToPlayer > currentWeaponRange)
                    {
                        character.CurrentState = distanceToPlayer < chasingRadius ? CharacterState.chasing : CharacterState.idling;
                    }
                    else
                        weaponSystem.AttackTarget(player.gameObject);
                    break;
                }
            case CharacterState.chasing:
                {
                    if (distanceToPlayer <= currentWeaponRange)
                    {
                        character.CurrentState = CharacterState.attacking;
                        break;
                    }
                    //if ((seeking && distanceToPlayer > seekingRadius && !CheckTargetInLooking())
                    //    && (distanceToPlayer > chasingRadius))
                    //{
                    //    character.CurrentState = CharacterState.idling;
                    //    break;
                    //}

                    if (distanceToPlayer > chasingRadius)
                    {
                        character.CurrentState = CharacterState.idling;
                        break;
                    }

                    // Chasing player
                    if (chasingTime > chasingTimeLimited)
                    {
                        steering = Vector3.zero;
                    }
                    else
                    {
                        chasingTime += Time.deltaTime;
                        steering += Seek(player.transform.position);
                    }

                    break;
                }
            case CharacterState.fleeing:
                {
                    // TODO: finish animation??
                    if (distanceToPlayer > fleeingRadius)
                    {
                        character.CurrentState = distanceToPlayer > chasingRadius ? CharacterState.chasing : CharacterState.idling;

                    }
                    else
                        steering += Flee(player.transform.position);
                    break;
                }
            case CharacterState.idling:
                {
                    if (distanceToPlayer <= currentWeaponRange)
                    {
                        character.CurrentState = CharacterState.attacking;
                        idleTime = 0f;
                    }
                    else if (seeking && (distanceToPlayer <= seekingRadius
                                    || CheckTargetInLooking()))
                    {
                        character.CurrentState = CharacterState.chasing;
                        chasingTime = 0f;
                        idleTime = 0f;
                    }
                    else if (fleeing && distanceToPlayer <= fleeingRadius)
                    {
                        character.CurrentState = CharacterState.fleeing;
                        idleTime = 0f;
                    }

                    // after idle time. enemy will patrolling
                    if (idleTime > idleTimeLimited)
                    {
                        character.CurrentState = CharacterState.patrolling;
                        idleTime = 0f;
                        // set to get new position
                        nextWaypointPos = transform.position;
                        patrolTime = 0f;
                    }
                    else
                        idleTime += Time.deltaTime;
                    break;
                }
            case CharacterState.patrolling:
                {
                    if (distanceToPlayer <= currentWeaponRange)
                    {
                        character.CurrentState = CharacterState.attacking;
                        idleTime = 0f;
                        break;
                    }
                    else if (seeking && (distanceToPlayer <= seekingRadius
                                         || CheckTargetInLooking()))
                    {
                        character.CurrentState = CharacterState.chasing;
                        chasingTime = 0f;
                        idleTime = 0f;
                        break;
                    }
                    else if (fleeing && distanceToPlayer <= fleeingRadius)
                    {
                        character.CurrentState = CharacterState.fleeing;
                        idleTime = 0f;
                        break;
                    }
                    /*
                    * patrolTime is used to avoid some destination, enemy can not reach.
                    */
                    if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance
                        || (patrolPath == null && patrolTime > patrolTimeLimited)
                        || (patrolPath != null && patrolTime > waitAtWaypointTime))
                    {
                        // Choose next waypoint index, cycling to start if necessary
                        if (patrolPath != null)
                        {
                            nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
                            nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                        }
                        else
                        {
                            nextWaypointPos = transform.position +
                                              new Vector3(
                                                    UnityEngine.Random.Range(-patrollRadius, patrollRadius),
                                                    0,
                                                    UnityEngine.Random.Range(-patrollRadius, patrollRadius));

                        }
                        patrolTime = 0f;
                    }
                    patrolTime += Time.deltaTime;

                    steering += Seek(nextWaypointPos) * patrolSpeed;
                    break;
                }
            default:
                {
                    break;
                }

        }
        agent.SetDestination(rigid.position + steering);
    }

    public void Attacking()
    {
        weaponSystem.AttackTarget(player.gameObject);
    }

    public Vector3 Seek(Vector3 pos)
    {
        var desiredVel = (pos - transform.position);
        if (desiredVel.magnitude < 0.2f)
        {
            return Vector3.zero;
        }

        desiredVel = desiredVel.normalized * seekingSpeed;
        return desiredVel - rigid.velocity;
    }

    private Vector3 Flee(Vector3 pos)
    {
        var desiredVel = transform.position - pos;
        if (desiredVel.magnitude > fleeingRadius)
            return Vector3.zero;

        desiredVel = desiredVel.normalized * fleeingSpeed;
        return desiredVel - rigid.velocity;
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw attack sphere 
        Gizmos.color = new Color(255f, 0, 0, .5f);
        Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

        // Draw chase sphere 
        Gizmos.color = new Color(0, 0, 255, .5f);
        //Gizmos.DrawWireSphere(transform.position, chaseRadius);

        // Draw flee sphere 
        Gizmos.color = new Color(0, 255, 255, .5f);
        Gizmos.DrawWireSphere(transform.position, fleeingRadius);

        // Draw chasing sphere 
        Gizmos.color = new Color(0, 50, 10, .5f);
        Gizmos.DrawWireSphere(transform.position, chasingRadius);

        // Draw able looking
        Handles.color = new Color(0, 1f, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -angleAbleLooking / 2, radiusAbleLooking);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, angleAbleLooking / 2, radiusAbleLooking);

    }
#endif

}
