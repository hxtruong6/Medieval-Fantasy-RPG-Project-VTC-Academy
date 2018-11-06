using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WyvernFireProjectile : MonoBehaviour {

    [SerializeField] private float fireProjectileSpeed;
    [SerializeField] private Rigidbody fireProjectile;
    [SerializeField] private Transform fireProjectPosition;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    public void fireShooting()
    {
        Rigidbody instantiatedProjectile = Instantiate(fireProjectile,fireProjectPosition.position,fireProjectPosition.rotation);
        instantiatedProjectile.velocity = fireProjectPosition.TransformDirection(new Vector3(0, 0, fireProjectileSpeed));
    }
}
