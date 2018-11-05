using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBoss : MonoBehaviour {
    public GameObject cameraActive;
    public GameObject cameraNotAtive;
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
            cameraActive.gameObject.SetActive(true);
            cameraNotAtive.gameObject.SetActive(false);
        }
    }
}
