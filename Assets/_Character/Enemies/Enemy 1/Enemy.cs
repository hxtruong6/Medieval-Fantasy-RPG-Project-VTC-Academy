using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
[RequireComponent(typeof(HealthSystem))]

public class Enemy : MonoBehaviour {

    [SerializeField] float chaseRadius = 6f;

    PlayerControl player;
    Character character;
    float distanceToPlayer;

    
    void Start ()
    {
        character = GetComponent<Character>();
        player = GameObject.FindObjectOfType<PlayerControl>();
    }
	
	void Update ()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer <= chaseRadius)
        {
            StartCoroutine(ChasePlayer());
        }

        if (player.GetComponent<HealthSystem>().healthAsPercentage <= 0 ||
            GetComponent<HealthSystem>().healthAsPercentage <= 0)
        {
            StopAllCoroutines();
            Destroy(this);//to stop enemies's behavior
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
