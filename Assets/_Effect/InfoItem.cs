using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoItem : MonoBehaviour {
    [SerializeField] Material normalMaterial;
    [SerializeField] Material outlineMaterial;
    [SerializeField] GameObject infoItem;
    [HideInInspector] public bool isSelected = false;

    GameObject mainBody;
    Material[] mats;


    void Start()
    {
        mainBody = GetComponentInChildren<MainBody>().gameObject;
    }


    public void HighLight(bool turnOn)
    {
        mats = mainBody.GetComponent<MeshRenderer>().materials;

        if (turnOn)
        {
            mats[0] = outlineMaterial;
            infoItem.gameObject.SetActive(true);
            isSelected = true;
        }
        else
        {
            mats[0] = normalMaterial;
            infoItem.gameObject.SetActive(false);
            isSelected = false;
        }

        mainBody.GetComponent<MeshRenderer>().materials = mats;
    }

    private void OnMouseEnter()
    {
        HighLight(true);
    }

    private void OnMouseExit()
    {
        HighLight(false);
    }

    public void RemoveInfo()
    {
        Destroy(infoItem);
    }
}
