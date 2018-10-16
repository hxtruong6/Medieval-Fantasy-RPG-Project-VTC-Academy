using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public FlockingAgent[] agents;
    public float neighbourDistance;
    public float[,] distances;
    public float aligmentWeight;
    public float cohesionWeight;
    public float separationWeight;

    private int lastFrame = -1;
    // Use this for initialization
    void Start()
    {
        distances = new float[agents.Length, agents.Length];
    }

    private void CalculateDistances()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            for (int j = i + 1; j < agents.Length; j++)
            {
                distances[i, j] = distances[j, i] =
                    Vector3.Distance(agents[i].transform.position,
                        agents[j].transform.position);
            }
        }
    }

    public Vector3 Calculate(int agentIndex)
    {
        var steering = Vector3.zero;
        var neighbours = GetNeighbours(agentIndex);
        Debug.Log(agentIndex + " has : " + neighbours.Count);
        if (agents[agentIndex].alignment)
        {
            steering += Aligment(agentIndex, neighbours) * aligmentWeight;
        }
        if (agents[agentIndex].cohesion)
        {
            steering += Cohesion(agentIndex, neighbours) * cohesionWeight;
        }
        if (agents[agentIndex].separation)
        {
            steering += Separation(agentIndex, neighbours) * separationWeight;
        }

        Debug.Log(agentIndex + " move: "+ steering);
        return steering;
    }

    public Vector3 Aligment(int agentIndex, List<int> neighbours)
    {
        var averangeHeading = Vector3.zero;

        if (neighbours.Count > 0)
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                averangeHeading += agents[agentIndex].Heading();
            }

            averangeHeading /= neighbours.Count;
            averangeHeading -= agents[agentIndex].Heading();
        }

        return averangeHeading;
    }

    public Vector3 Cohesion(int agentIndex, List<int> neighbours)
    {
        if (neighbours.Count > 0)
        {
            Vector3 centerOfMass = Vector3.zero;
            for (int i = 0; i < neighbours.Count; i++)
            {
                centerOfMass += agents[neighbours[i]].transform.position;
            }

            centerOfMass /= neighbours.Count;
            return agents[agentIndex].Seek(centerOfMass);
        }

        return Vector3.zero;
    }

    public Vector3 Separation(int agentIndex, List<int> neighbours)
    {
        var steering = Vector3.zero;
        for (int i = 0; i < neighbours.Count; i++)
        {
            var toAgent = agents[agentIndex].transform.position -
                          agents[neighbours[i]].transform.position;
            var mag = toAgent.magnitude;
            if (mag > 0)
            {
                steering += toAgent.normalized / mag;
            }
        }

        return steering;
    }

    public List<int> GetNeighbours(int agentIndex)
    {
        if (Time.frameCount > lastFrame)
        {
            lastFrame = Time.frameCount;
            CalculateDistances();
        }

        var list = new List<int>();
        for (int i = 0; i < agents.Length; i++)
        {
            //Debug.Log(agentIndex +"-> "+ i +": " + distances[agentIndex,i]);
            if (i != agentIndex && distances[agentIndex, i] < neighbourDistance)
            {
                list.Add(i);
            }
        }

        return list;
    }

    void OnDrawGizmos()
    {
        // Draw attack sphere
        Gizmos.color = new Color(255f, 0, 30f, 0.5f);
        for (int i = 0; i < agents.Length; i++)
        {
            Gizmos.DrawWireSphere(agents[i].gameObject.transform.position, neighbourDistance);
        }

    }

#if UNITY_EDITOR
    [ContextMenu("Get References")]
    private void GetReferncesNeighbors()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            UnityEditor.Undo.RecordObject(agents[i], "Modify agent");
            agents[i].flock = this;
            agents[i].index = i;
        }
    }
#endif

}
