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
    public GameObject gameManager;
    public float timeCutScene;
    public bool viewTimeLine = false;
    float timer = 0;
    public bool ativeEndObject;
    public GameObject endPanel;
    public GameObject environment;
    public bool ativeBoss;
    public GameObject boss;
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		if (viewTimeLine)
        {
            gameManager.GetComponent<GameManager>().enabled = false;
            FindObjectOfType<PlayerControl>().GetComponent<DemonTrigger>().enabled = false;
            canvasUI.gameObject.SetActive(false);
            enemy.gameObject.SetActive(false);
            follow.gameObject.SetActive(true);
            cameraCM.gameObject.SetActive(true);
            timeLine.Play();
            timer += Time.deltaTime;
            if (timer > timeCutScene)
            {
                viewTimeLine = false;
                if (ativeEndObject) {
                    endPanel.gameObject.SetActive(true);
                    environment.gameObject.SetActive(false);
                }
                else
                {
                    if (ativeBoss)
                    {
                        boss.gameObject.SetActive(true);
                        //var playerCurrentMeleeWeapon = FindObjectOfType<PlayerControl>().GetComponent<InventorySystem>().GetEquippedMeleeWeapon();
                        //playerCurrentMeleeWeapon.SetAttackRange(playerCurrentMeleeWeapon.GetMaxAttackRange() * 2);
                        //WeaponConfig demonCurrentMeleeWeapon = FindObjectOfType<PlayerControl>().GetComponent<DemonTrigger>().replaceMeleeWeapon;
                        //demonCurrentMeleeWeapon.SetAttackRange(demonCurrentMeleeWeapon.GetMaxAttackRange() * 2);
                    }
                    gameManager.GetComponent<GameManager>().enabled = true;
                    FindObjectOfType<PlayerControl>().GetComponent<DemonTrigger>().enabled = true;
                    canvasUI.gameObject.SetActive(true);
                    enemy.gameObject.SetActive(true);
                    follow.gameObject.SetActive(false);
                    this.gameObject.SetActive(false);
                    Destroy(this.gameObject);
                }
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
