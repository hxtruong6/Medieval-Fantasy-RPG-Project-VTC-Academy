using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonOnTriggerSpawn : MonoBehaviour {
    public GameObject dragon;
    public GameObject particle;
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControl>())
        {
            dragon.gameObject.SetActive(true);
            Destroy(this.gameObject.GetComponent<BoxCollider>());
            Destroy(particle);
        }
    }
}
