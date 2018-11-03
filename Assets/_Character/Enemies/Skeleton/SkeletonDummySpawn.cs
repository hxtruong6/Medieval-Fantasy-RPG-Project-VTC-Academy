using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonDummySpawn : MonoBehaviour
{

    public GameObject Skeleton;
	// Use this for initialization
	void Start ()
	{
	    //Skeleton = gameObject.GetComponentInParent<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void DestroySkeletonDummy()
    {
        
        Skeleton.gameObject.SetActive(true);
        Skeleton.transform.parent = null;
        Skeleton.gameObject.layer = 9;
        Destroy(this.gameObject.GetComponent<SphereCollider>());
    }
}
