using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        //var layerCollidedWith = collision.gameObject.layer;
        //if (shooter && layerCollidedWith != shooter.layer)
        //{
            DealDamage(collision.gameObject);
        //}
    }


    private void DealDamage(GameObject objectBeingHit)
    {
        if (!objectBeingHit.GetComponent<HealthSystem>() ||
            objectBeingHit.GetComponent<HealthSystem>().HealthAsPercentage < 0)
        {
            return;
        }
        float damage = 0;
        //AudioSource audioSource = GetComponentInParent<AudioSource>();
        //audioSource.PlayOneShot(projectileConfig.GetContactSound());

 
        Destroy(gameObject);
    }
}
