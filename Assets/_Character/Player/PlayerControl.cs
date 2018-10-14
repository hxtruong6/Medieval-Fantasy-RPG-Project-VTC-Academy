using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    CameraRaycaster cameraRaycaster;

    Character character;
    Enemy enemy;
    WeaponSystem weaponSystem;
    InventorySystem inventorySystem;

    KeyCode switchWeaponKey = KeyCode.Tab;

    void Start()
    {
        character = GetComponent<Character>();
        weaponSystem = GetComponent<WeaponSystem>();
        inventorySystem = GetComponent<InventorySystem>();

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
        if (Input.GetKeyDown(switchWeaponKey))
        {
            StopAllCoroutines();
            inventorySystem.SwitchWeapon();
        }
    }

    void OnMouseOverPotentiallyWalkable(Vector3 destination)
    {
        if (Input.GetMouseButton(0))
        {
            weaponSystem.CancleAction();
            character.CurrentState = CharacterState.running;
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
            weaponSystem.AttackTarget(enemy.gameObject);
        }
        else if (Input.GetMouseButton(0) && !IsTargetInRange(enemy.gameObject))
        {
            StartCoroutine(MoveAndAttack(enemy));
        }

        //TODO Impliment Move to enemy and use skills
    }

    IEnumerator MoveToTarget(Enemy target)
    {
        character.CurrentState = CharacterState.running;
        character.SetDestination(target.transform.position);
        while (!IsTargetInRange(target.gameObject))
        {
            yield return new WaitForEndOfFrame();
        }
        character.SetDestination(character.transform.position);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator MoveAndAttack(Enemy enemy)
    {
        yield return StartCoroutine(MoveToTarget(enemy));
        weaponSystem.AttackTarget(enemy.gameObject);
    }

    private void StandStill()
    {
        character.CurrentState = CharacterState.idling;
    }

    void OnDrawGizmos()
    {
        if (weaponSystem != null)
        {
            Gizmos.color = new Color(255f, 0, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, weaponSystem.GetCurrentWeapon().GetMaxAttackRange());
        }
    }
}
