using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Vector3 textOffset;

    public void Create(float amount, Vector3 location, Color color)
    {
        var instance = Instantiate(prefab);
        instance.transform.SetParent(transform);
        var damageText = instance.GetComponent<DamageText>();
        damageText.Active(amount, location + textOffset, color);
    }

}
