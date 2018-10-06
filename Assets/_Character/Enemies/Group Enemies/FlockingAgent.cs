using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingAgent : MonoBehaviour
{
    public Transform target;
    public Transform[] enemies;
    public Rigidbody rigid;
    public float seekingSpeed;
    public float fleeingSpeed;
    public float safeDistance;
	
    public bool seeking;
    public bool alignment;
    public bool cohesion;
    public bool separation;
    public bool flocking;

    public Flock flock;
    public int index;
    public Vector3 Heading()
    {
        return rigid.velocity;
    }

    void FixedUpdate()
    {
        var steering = Vector3.zero;

        if (seeking)
        {
            steering += Seek(target.position);
        }
        //for (int i = 0; i < enemies.Length; i++)
        //{
        //    steering += Flee(enemies[i].position);
        //}
        if (flocking)
        {
            steering += flock.Calculate(index);
        }
        steering /= 10f;
        rigid.velocity += steering;
    }

    public void FindTarget()
    {
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
        var desiredVel = transform.position - pos;
        if (desiredVel.magnitude > safeDistance)
            return Vector3.zero;

        desiredVel = desiredVel.normalized * fleeingSpeed;
        return desiredVel - rigid.velocity;
    }
}
