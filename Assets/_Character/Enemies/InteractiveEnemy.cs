using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEnemy : MonoBehaviour {

    [SerializeField] Material normalMaterial;
    [SerializeField] Material outlineMaterial;
    [SerializeField] GameObject enemyCanvas;

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
        }
        else
        {
            mats[0] = normalMaterial;
            enemyCanvas.gameObject.SetActive(false);
        }

        mainBody.GetComponent<SkinnedMeshRenderer>().materials = mats;
    }

    private void OnMouseEnter()
    {
        var enemyHP = GetComponent<HealthSystem>().healthAsPercentage;
        if ( enemyHP > 0)
        {
            HighLight(true);
        }
    }

    private void OnMouseExit()
    {
        HighLight(false);
    }
}
