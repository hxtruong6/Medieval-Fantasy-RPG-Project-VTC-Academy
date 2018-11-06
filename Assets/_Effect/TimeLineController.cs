using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineController : MonoBehaviour {
    public PlayableDirector timeLine;
    public GameObject canvasUI;
    public GameObject cameraCM;
    public GameObject follow;
    public GameObject enemy;
    public float timeCutScene;
    public bool viewTimeLine = false;
    float timer = 0;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		if (viewTimeLine)
        {
            canvasUI.gameObject.SetActive(false);
            enemy.gameObject.SetActive(false);
            follow.gameObject.SetActive(true);
            cameraCM.gameObject.SetActive(true);
            timeLine.Play();
            timer += Time.deltaTime;
            if (timer > timeCutScene)
            {
                viewTimeLine = false;
                canvasUI.gameObject.SetActive(true);
                enemy.gameObject.SetActive(true);
                follow.gameObject.SetActive(false);
                this.gameObject.SetActive(false);
                Destroy(this.gameObject);
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControl>())
        {
            viewTimeLine = true;
        }
    }
}
