using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonICEAOE : MonoBehaviour {
    public int damge = 5;
    private float timer;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (timer <= 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<HealthSystem>().TakeDamage(damge);
            timer = 1;
        }
        else timer -= Time.deltaTime;
    }
}
