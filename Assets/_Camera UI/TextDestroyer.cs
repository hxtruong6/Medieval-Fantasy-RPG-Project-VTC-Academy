using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextDestroyer : MonoBehaviour {

    [SerializeField] GameObject objectToDestroy;

    public void DestroyText()
    {
        Destroy(objectToDestroy);
    }
}
