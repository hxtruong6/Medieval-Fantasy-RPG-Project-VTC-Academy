using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEnemy : MonoBehaviour {

    [SerializeField] Material normalMaterial;
    [SerializeField] Material outlineMaterial;
    [SerializeField] GameObject enemyCanvas;

    GameObject mainBody;
    Material[] mats;
    bool isMouseOver = false;

    void Start()
    {
        mainBody = GetComponentInChildren<MainBody>().gameObject;
    }

    public void SetHighLight(bool value)
    {       
        if (isMouseOver != value)
        {
            HighLight(value);
            isMouseOver = value;
        }       
    }


    private void HighLight(bool turnOn)
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
}
