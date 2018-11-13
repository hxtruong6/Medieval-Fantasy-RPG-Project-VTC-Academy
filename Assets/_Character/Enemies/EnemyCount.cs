using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCount : MonoBehaviour {
    public Image healthBar;
	// Use this for initialization
	void Start () {
        EndLocation.AddToEnemyCount();
	}
	
	// Update is called once per frame
	void Update () {
		if (healthBar.fillAmount <= 0)
        {
            EndLocation.RemoveEnemy();
            Destroy(this.gameObject.GetComponent<EnemyCount>());
        }
	}
}
