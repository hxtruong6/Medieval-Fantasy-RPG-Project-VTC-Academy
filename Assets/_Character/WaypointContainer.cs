using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointContainer : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Vector3 firstPosition = transform.GetChild(0).position;
        Vector3 prevPosition = firstPosition;
        foreach (Transform waypoint in transform)
        {
            Gizmos.DrawSphere(waypoint.position, 0.2f);
            Gizmos.DrawLine(prevPosition, waypoint.position);
            prevPosition = waypoint.position;
        }
        Gizmos.DrawLine(prevPosition, firstPosition);
    }
}
