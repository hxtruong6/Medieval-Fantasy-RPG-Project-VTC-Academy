using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour {

    [SerializeField] TextMeshProUGUI textMesh;

    public void Active(float amount, Vector3 position, Color color)
    {
        transform.position = Camera.main.WorldToScreenPoint(position);
        textMesh.text = amount.ToString();
        textMesh.color = color;
    }
}
