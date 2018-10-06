using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour {
    public Transform playerTransform;

    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;

    public bool lootAtPlayer = true;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPos = playerTransform.position;
        transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);
        if (lootAtPlayer)
        {
            transform.LookAt(playerTransform);
        }
    }
}
