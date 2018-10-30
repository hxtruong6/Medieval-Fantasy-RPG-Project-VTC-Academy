using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour {

    [SerializeField] TextMeshPro textMesh;

    public void Active(float amount, Vector3 position)
    {
        transform.position = Camera.main.WorldToScreenPoint(position);
        textMesh.text = amount.ToString();
    }
}
