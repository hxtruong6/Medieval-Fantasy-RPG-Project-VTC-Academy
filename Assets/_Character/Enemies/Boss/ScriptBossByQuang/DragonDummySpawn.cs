using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonDummySpawn : MonoBehaviour {
    public GameObject dragon;
    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DestroyDragonDummy()
    {

        dragon.gameObject.SetActive(true);
        dragon.transform.parent = null;
        dragon.gameObject.layer = 12;
        Destroy(this.gameObject);
    }
}
