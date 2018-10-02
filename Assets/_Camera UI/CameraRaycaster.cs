﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraRaycaster : MonoBehaviour {
    [SerializeField] Texture2D walkCursor;
    [SerializeField] Texture2D enemyCursor;
    [SerializeField] Texture2D unknownCursor;
    [SerializeField] Vector2 cursorHotspot;

    const int POTENTIALLY_WALKABLE_LAYER = 8;
    float maxRaycastDepth = 100f;

    public delegate void OnMouseOverEnemy(Enemy enemy);
    public event OnMouseOverEnemy onMouseOverEnemy;

    public delegate void OnMouseOverTerrain(Vector3 destination);
    public event OnMouseOverTerrain onMouseOverPotentiallyWalkable;

    void Update()
    {
        // Check if pointer is over an interactable UI element
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Impliment UI interaction
        }
        else
        {
            PerformRaycasts();
        }
    }

    void PerformRaycasts()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (RaycastForEnemy(ray))
        {
            return;
        }
        if (RaycastForPotentiallyWalkable(ray))
        {
            return;
        }
        Cursor.SetCursor(unknownCursor, cursorHotspot, CursorMode.Auto);
    }

    bool RaycastForEnemy(Ray ray)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, maxRaycastDepth))
        {
            var gameObjectHit = hitInfo.collider.gameObject;
            var enemyHit = gameObjectHit.GetComponent<Enemy>();
            if (enemyHit)
            {
                Cursor.SetCursor(enemyCursor, cursorHotspot, CursorMode.Auto);
                onMouseOverEnemy(enemyHit);
                return true;
            }
        }
        return false;
    }

    private bool RaycastForPotentiallyWalkable(Ray ray)
    {
        RaycastHit hitInfo;
        LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
        bool potentiallyWalkableHit = Physics.Raycast(ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer);
        if (potentiallyWalkableHit)
        {
            Cursor.SetCursor(walkCursor, cursorHotspot, CursorMode.Auto);
            onMouseOverPotentiallyWalkable(hitInfo.point);
            return true;
        }
        return false;
    }
}
