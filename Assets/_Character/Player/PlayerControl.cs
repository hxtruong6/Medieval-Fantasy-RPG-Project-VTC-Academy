using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    CameraRaycaster cameraRaycaster;

    Character character;
    Enemy enemy;
    WeaponSystem weaponSystem;

    void Start()
    {
        character = GetComponent<Character>();
        weaponSystem = GetComponent<WeaponSystem>();

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
            weaponSystem.StopAttacking();
            character.SetDestination(destination);
        }
    }

    private bool IsTargetInRange(GameObject target)
    {
        float distanceToTarget = (target.transform.position - transform.position).magnitude;
        return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
    }

    void OnMouseOverEnemy(Enemy enemyToSet)
    {
        enemy = enemyToSet;

        if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
        {
            weaponSystem.AttackTargetOnce(enemy.gameObject);
        }
        else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy.gameObject))
        {
            StartCoroutine(MoveAndAttack(enemy));
        }

        //TODO Impliment Move to enemy and use skills
    }

    IEnumerator MoveToTarget(Enemy target)
    {
        character.SetDestination(target.transform.position);
        while (!IsTargetInRange(target.gameObject))
        {
            yield return new WaitForEndOfFrame();
            character.SetDestination(target.transform.position);
        }
        yield return new WaitForEndOfFrame();
    }

    IEnumerator MoveAndAttack(Enemy enemy)
    {
        yield return StartCoroutine(MoveToTarget(enemy));
        weaponSystem.AttackTargetOnce(enemy.gameObject);
    }
}
