using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]

public class Enemy : MonoBehaviour {

    [SerializeField] float chaseRadius = 6f;

    PlayerMovement player;
    Character character;
    float distanceToPlayer;

    
    void Start ()
    {
        character = GetComponent<Character>();
        player = GameObject.FindObjectOfType<PlayerMovement>();
    }
	
	void Update ()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer <= chaseRadius)
        {
            StartCoroutine(ChasePlayer());
        }
    }

    IEnumerator ChasePlayer()
    {
        while (distanceToPlayer <= chaseRadius)
        {
            character.SetDestination(player.transform.position);
            yield return new WaitForEndOfFrame();
        }
    }

    void OnDrawGizmos()
    {
        // TODO Draw attack sphere 

        // Draw chase sphere 
        Gizmos.color = new Color(0, 0, 255, .5f);
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }
}
