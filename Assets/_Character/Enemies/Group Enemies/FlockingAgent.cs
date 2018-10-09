using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlockingAgent : MonoBehaviour
{
    private Transform target;
//    public Transform[] enemies;
    public Rigidbody rigid;
    public float seekingSpeed;
    public float seekingRadius; // if player is out of this distance, the enemy doesn't seek
    public float fleeingSpeed;
    public float safeDistance;

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

//    void OnDrawGizmos()
//    {
//        // Draw attack sphere 
//        Gizmos.color = new Color(255f, 240f, 0, .5f);
//        Gizmos.DrawWireSphere(transform.position, safeDistance);
//    }

    void FixedUpdate()
    {
        FindTarget();   
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
        if (Vector3.Distance(target.position, flock.transform.position) > seekingRadius )
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
