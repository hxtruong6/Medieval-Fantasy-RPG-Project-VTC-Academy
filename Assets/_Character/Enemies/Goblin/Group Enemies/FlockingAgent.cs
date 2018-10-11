using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FlockingAgent : MonoBehaviour
{
    private Transform target;
    //    public Transform[] enemies;
    public Rigidbody rigid;
    public float seekingSpeed;
    public float seekingRadius; // if player is out of this distance, the enemy doesn't seek
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

    public Flock flock;
    public int index;
    NavMeshAgent agent;


    public Vector3 Heading()
    {
        return rigid.velocity;
    }

    void Start()
    {
        FindTarget();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updatePosition = true;

    }


#if UNITY_EDITOR

    void OnDrawGizmos()
    {
        // Draw attack sphere 
        //Gizmos.color = new Color(255f, 240f, 0, .5f);
        //Gizmos.DrawWireSphere(transform.position, safeDistance);
        Handles.color = new Color(0, 1f, 0, 0.2f);
        //Handles.DrawWireArc(transform.position, transform.up, transform.forward, angleAbleLooking, radiusAbleLooking);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, -angleAbleLooking / 2, radiusAbleLooking);
        Handles.DrawSolidArc(transform.position, transform.up, transform.forward, angleAbleLooking / 2, radiusAbleLooking);

    }
#endif

    bool CheckTargetInLooking()
    {
        Vector3 vecToTarget = target.position - transform.position;
        var flatVecToTarget = Vector3.ProjectOnPlane(vecToTarget, Vector3.up);
        var flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        var angle = Vector3.Angle(flatVecToTarget, flatForward);
        if (angle < angleAbleLooking / 2 && Vector3.Distance(transform.position, target.position) < radiusAbleLooking)
        {
            return true;
        }

        return false;
    }


    void FixedUpdate()
    {
        FindTarget();
        if (CheckTargetInLooking())
        {
            Debug.Log(gameObject.name + " is looking Target");
        }

        var steering = Vector3.zero;

        if (seeking)
        {
            steering += Seek(target.position);
        }

        if (fleeing)
        {
            //            for (int i = 0; i < enemies.Length; i++)
            //            {
            //                steering += Flee(enemies[i].position);
            //            }
            steering += Flee(target.position);
        }
        if (flocking)
        {
            steering += flock.Calculate(index);
        }
        //steering /= 10f;
        agent.SetDestination(rigid.position + steering);
        //rigid.velocity += steering;
    }

    void EnemyMoving()
    {
        FindTarget();
        var steering = Vector3.zero;

        if (seeking)
        {
            steering += Seek(target.position);
        }
        if (fleeing)
        {
            steering += Flee(target.position);
        }
        if (flocking)
        {
            steering += flock.Calculate(index);
        }
        agent.SetDestination(rigid.position + steering);
    }

    public void FindTarget()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        //Debug.Log("Player: " + target.position);
    }

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

    public Vector3 Seek(Vector3 pos)
    {
        if (!seeking) return Vector3.zero;
        if (Vector3.Distance(target.position, flock.transform.position) > seekingRadius)
        {
            return Vector3.zero;
        }
        var desiredVel =
            (pos - transform.position);

        if (desiredVel.magnitude < 0.2f)
        {
            return Vector3.zero;
        }
        return desiredVel - rigid.velocity;
    }

    private Vector3 Flee(Vector3 pos)
    {
        if (!fleeing) return Vector3.zero;
        var desiredVel = transform.position - pos;
        if (desiredVel.magnitude > safeDistance)
            return Vector3.zero;

        desiredVel = desiredVel.normalized * fleeingSpeed;
        return desiredVel - rigid.velocity;
    }
}
