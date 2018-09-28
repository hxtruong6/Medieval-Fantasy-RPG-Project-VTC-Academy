using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CameraRaycaster cameraRaycaster;

    Character character;

    void Start()
    {
        character = GetComponent<Character>();

        RegisterForMouseEvents();
    }

    void RegisterForMouseEvents()
    {
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();
        cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
    }

    void Update()
    {
        
    }

    void OnMouseOverPotentiallyWalkable(Vector3 destination)
    {
        if (Input.GetMouseButton(0))
        {
            character.SetDestination(destination);
        }
    }

    void OnMouseOverEnemy(Enemy enemy)
    {
        //TODO Impliment
    }
}
