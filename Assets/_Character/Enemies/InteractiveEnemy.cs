using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEnemy : MonoBehaviour {

    [SerializeField] Material normalMaterial;
    [SerializeField] Material outlineMaterial;
    [SerializeField] GameObject enemyCanvas;

    [HideInInspector] public bool isSelected = false;

    GameObject mainBody;
    Material[] mats;
    

    void Start()
    {
        mainBody = GetComponentInChildren<MainBody>().gameObject;
    }


    public void HighLight(bool turnOn)
    {
        mats = mainBody.GetComponent<SkinnedMeshRenderer>().materials;       
       
        if (turnOn)
        {
            mats[0] = outlineMaterial;
            enemyCanvas.gameObject.SetActive(true);
            isSelected = true;
        }
        else
        {
            mats[0] = normalMaterial;
            enemyCanvas.gameObject.SetActive(false);
            isSelected = false;
        }

        mainBody.GetComponent<SkinnedMeshRenderer>().materials = mats;
    }

    private void OnMouseEnter()
    {
        var enemyHP = GetComponent<HealthSystem>().HealthAsPercentage;
        if ( enemyHP > 0)
        {
            HighLight(true);
        }
    }

    private void OnMouseExit()
    {
        HighLight(false);
        isSelected = false;
        GetComponent<HealthSystem>().FlashEnemyHealthBar();
    }
}
