using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernFireProject : MonoBehaviour {

    [SerializeField] private float fireProjectileSpeed;
    [SerializeField] private Object fireProjectile;
    [SerializeField] private Transform fireProjectPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void fireShooting()
    {
        Rigidbody instantiatedProjectile = Instantiate(fireProjectile,transform.position,transform.rotation) as Rigidbody;

        instantiatedProjectile.velocity = transform.TransformDirection(new Vector3(0, 0, fireProjectileSpeed));
    }
}
