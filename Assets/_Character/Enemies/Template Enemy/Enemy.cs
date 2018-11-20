//#if UNITY_EDITOR
using System.Collections;
//using UnityEditor;
//#endif
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

    public WaypointContainer patrolPath;
    public float waypointTolerance = 1f;
    public float waitAtWaypointTime = 3f;
    public float angleAbleLooking = 30f;
    public float radiusAbleLooking = 10f;

    protected PlayerControl player;
    protected Character character;

    protected float idleTime = 0f;
    protected float chasingTime = 0f;
    protected float currentWaitAtWaypointTime = 0f;
    protected float currentWeaponRange;
    protected int nextWaypointIndex;
    protected WeaponSystem weaponSystem;
    protected Vector3 nextWaypointPos;


    Rigidbody rigid;
    NavMeshAgent agent;
    float distanceToPlayer;
    bool isDead;
    private Vector3 lastPosition;
    private float fleeingTime = 0f;

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
        lastPosition = transform.position;
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

    public bool PlayerOrEnemyAliveToContinue()
    {
        //player = GameObject.FindObjectOfType<PlayerControl>();
        if (GetComponent<HealthSystem>().HealthAsPercentage <= 0)
        {
            this.enabled = false;
            return false;
        }
        else if (player.GetComponent<HealthSystem>().HealthAsPercentage <= 0)
        {
            return false;
        }
        return true;
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
        if (isDead && !PlayerOrEnemyAliveToContinue())//NEW
            return;
        
        var steering = Vector3.zero;
        distanceToPlayer = GetDistanceToPlayer();
        switch (character.CurrentState)
        {
            case CharacterState.attacking:
                {
                    transform.LookAt(player.transform);
                    if (weaponSystem.canAttack == false) break;
                    if (fleeing && distanceToPlayer < fleeingRadius)
                    {

                        character.CurrentState = CharacterState.fleeing;
                    }

                    else if (distanceToPlayer > currentWeaponRange)
                    {
                        character.CurrentState = distanceToPlayer < chasingRadius ? CharacterState.chasing : CharacterState.idling;
                    }
                    else
                    {
                        Attacking();
                    }
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
                        character.CurrentState = CharacterState.patrolling;

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
                    lastPosition = transform.position;
                    if (distanceToPlayer > fleeingRadius)
                    {
                        character.CurrentState = distanceToPlayer > chasingRadius ? CharacterState.chasing : CharacterState.idling;
                        break;
                    }
                    //else if (fleeingTime > 1f && Vector3.Distance(lastPosition, transform.position) < 0.5f)
                    //{
                    //    fleeingTime = 0;
                    //    lastPosition = transform.position;
                    //    character.CurrentState = CharacterState.attacking;
                    //    //transform.LookAt(player.transform);
                    //    //transform.position = Vector3.MoveTowards(transform.position, player.transform.position, fleeingSpeed);
                    //    //steering = Seek(RandomNavSphere(transform.position, patrollRadius, -1));
                    //}
                    //else if (fleeingTime > 1f)
                    //{
                    //    lastPosition = transform.position;
                    //}
                    //else
                    //{
                    //    fleeingTime += Time.deltaTime;
                    //}
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
                        currentWaitAtWaypointTime = 0f;
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

                    if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
                    {
                        if (currentWaitAtWaypointTime < waitAtWaypointTime)
                        {
                            currentWaitAtWaypointTime += Time.deltaTime;
                        }
                        else
                        {
                            currentWaitAtWaypointTime = 0f;
                            // Choose next waypoint index, cycling to start if necessary
                            if (patrolPath != null)
                            {
                                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
                                nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                            }
                            else
                            {
                                nextWaypointPos = RandomNavSphere(transform.position, patrollRadius);
                            }
                        }


                    }
                    steering += Seek(nextWaypointPos) * patrolSpeed;
                    break;
                }
            default:
                {
                    break;
                }

        }
        agent.SetDestination(transform.position + steering);

    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);
        return navHit.position;
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
    public void SwicthChasingState()
    {
        character.CurrentState = CharacterState.chasing;
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if ((collision.gameObject.GetComponent<Projectile>() || collision.gameObject.GetComponent<PierceProjectile>()) &&
    //        (character.CurrentState == CharacterState.idling || character.CurrentState == CharacterState.patrolling))
    //    {
    //        character.CurrentState = CharacterState.chasing;
    //    }
    //}

    public IEnumerator Kill(float destroyTime)//NEW
    {
        character.SetDestination(transform.position);
        isDead = true;
        yield return new WaitForSeconds(destroyTime);
        Destroy(gameObject);
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
        UnityEditor.Handles.color = new Color(0, 1f, 0, 0.2f);
        UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -angleAbleLooking / 2, radiusAbleLooking);
        UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, transform.forward, angleAbleLooking / 2, radiusAbleLooking);

    }
#endif

}