using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderActiveObject : MonoBehaviour {
    public GameObject[] objectsPrefab;
    public bool[] objectAtive;
    public bool destroyObject;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControl>())
        {
            for (int i = 0; i < objectsPrefab.Length; i++)
            {
                objectsPrefab[i].gameObject.SetActive(objectAtive[i]);
                if (destroyObject)
                Destroy(this.gameObject);
            }
        }
    }
}
