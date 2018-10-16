using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FlockingAgent : MonoBehaviour
{
    enum State { IDLE, PATROLLING, ATTACKING, CHASING, FLEEING }
    [SerializeField] State state = State.IDLE;
    Character character;
    //    public Transform[] enemies;
    public Rigidbody rigid;
    public float seekingSpeed;
    public float seekingRadius; // if player is out of this distance, the enemy doesn't seek
                                //    public float expandSeekingRadius = 4f; // if enemy is chasing player, seekingRadius will expand the radius until player is be out of range 
    public float fleeingSpeed;
    public float safeDistance;
    [SerializeField] protected float angleAbleLooking = 30f;
    [SerializeField] protected float radiusAbleLooking = 10f;

    public bool seeking;
    public bool fleeing;
    public bool alignment;
    public bool cohesion;
    public bool separation;
    public bool flocking;
    public float chasingTimeLimited = 10f;
    private float chasingTime = 0f;

    public Flock flock;
    public int index;
    NavMeshAgent agent;

    private WeaponSystem weaponSystem;
    private float currentWeaponRange;
    PlayerControl player;


    public Vector3 Heading()
    {
        return rigid.velocity;
    }

    void Start()
    {
        character = GetComponent<Character>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = true;
        chasingTime = 0f;

        player = GameObject.FindObjectOfType<PlayerControl>();
        weaponSystem = GetComponent<WeaponSystem>();//the weapon system dont change but the weapon may, depend on 
        currentWeaponRange = weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
    }


#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw attack sphere 
        Gizmos.color = new Color(25f, 140f, 0, .5f);
        //Gizmos.DrawWireSphere(transform.position, safeDistance);
        //Gizmos.DrawWireSphere(transform.position, seekingRadius);
        Handles.color = new Color(0, 1f, 0, 0.2f);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -angleAbleLooking / 2, radiusAbleLooking);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, angleAbleLooking / 2, radiusAbleLooking);

    }
#endif

    public void PlayerOrEnemyAliveToContinue()
    {
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
        if (angle < angleAbleLooking / 2 && Vector3.Distance(transform.position, character.transform.position) < radiusAbleLooking)
        {
            return true;
        }

        return false;
    }


    void FixedUpdate()
    {
        PlayerOrEnemyAliveToContinue();
        var steering = Vector3.zero;
        var distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        switch (state)
        {
            case State.ATTACKING:
                {
                    if (distanceToPlayer > currentWeaponRange)
                    {
                        state = State.IDLE;
                    }
                    else
                        weaponSystem.AttackTarget(player.gameObject);
                    break;
                }
            case State.CHASING:
                {
                    if (distanceToPlayer <= currentWeaponRange)
                    {
                        state = State.ATTACKING;
                        break;
                    }
                    else if (distanceToPlayer > seekingRadius && !CheckTargetInLooking())
                    {
                        state = State.IDLE;
                        break;
                    }

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
            case State.FLEEING:
                {
                    if (distanceToPlayer <= currentWeaponRange)
                    {
                        state = State.ATTACKING;
                        break;
                    }
                    else if (distanceToPlayer > safeDistance)
                    {
                        state = State.IDLE;
                        break;
                    }

                    steering += Flee(player.transform.position);
                    break;
                }
            case State.IDLE:
                {
                    if (distanceToPlayer <= currentWeaponRange)
                    {
                        state = State.ATTACKING;
                    }
                    else if (seeking && (distanceToPlayer <= seekingRadius
                                    || CheckTargetInLooking()))
                    {
                        state = State.CHASING;
                        chasingTime = 0f;
                    }
                    else if (fleeing && distanceToPlayer <= safeDistance)
                    {
                        state = State.FLEEING;
                    }
                    break;
                }
            default:
                {
                    break;
                }

        }

        if (flocking)
        {
            steering += flock.Calculate(index);
        }
        agent.SetDestination(rigid.position + steering);
    }

    //    void EnemyMoving()
    //    {
    //        FindTarget();
    //        var steering = Vector3.zero;
    //
    //        //TODO: need to an expand radius when is chasing target
    //        //            || (stateMoving == StateMoving.SEEKING
    //        //                && Vector3.Distance(pos, transform.position) + expandSeekingRadius > seekingRadius))
    //
    //        if (seeking && (Vector3.Distance(target.position, transform.position) <= seekingRadius
    //                        || CheckTargetInLooking()))
    //        {
    //            steering += Seek(target.position);
    //        }
    //        if (fleeing)
    //        {
    //            steering += Flee(target.position);
    //        }
    //        if (flocking)
    //        {
    //            steering += flock.Calculate(index);
    //        }
    //        agent.SetDestination(rigid.position + steering);
    //    }

    //    private void FindTarget()
    //    {
    //        target = GameObject.FindGameObjectWithTag("Player").transform;
    //        //Debug.Log("Player: " + target.position);
    //    }

    public void EnableSeek()
    {
        seeking = true;
        flocking = false;
    }

    public void EnableFlee()
    {
        seeking = false;
        flocking = true;
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
        if (desiredVel.magnitude > safeDistance)
            return Vector3.zero;

        desiredVel = desiredVel.normalized * fleeingSpeed;
        return desiredVel - rigid.velocity;
    }
}
