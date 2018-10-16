using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float pickItemRadius = 2.5f;

    CameraRaycaster cameraRaycaster;

    Character character;
    Enemy enemy;
    DropItem dropItem;
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

    void Update()
    {
        if (Input.GetKeyDown(switchWeaponKey))
        {
            StopAllCoroutines();
            inventorySystem.SwitchWeapon();
        }
    }

    private bool IsItemInPickUpRange(GameObject target)
    {
        float distanceToTarget = (target.transform.position - transform.position).magnitude;
        return distanceToTarget <= pickItemRadius;
    }

    void OnMouseOverDropItem(DropItem itemToSet)
    {
        dropItem = itemToSet;

        if (Input.GetMouseButton(0) && IsItemInPickUpRange(dropItem.gameObject))
        {
            inventorySystem.PickUpNewWeapon(dropItem);
        }
        else if (Input.GetMouseButton(0) && !IsItemInPickUpRange(dropItem.gameObject))
        {
            StartCoroutine(MoveAndPickUpItem(dropItem));
        }
    }

    void OnMouseOverEnemy(Enemy enemyToSet)
    {
        ChangeTargetEnemy(enemyToSet);

        if (Input.GetMouseButton(0) && IsTargetInAttackRange(enemy.gameObject))
        {
            transform.LookAt(enemy.transform);
            weaponSystem.AttackTarget(enemy.gameObject);
        }
        else if (Input.GetMouseButton(0) && !IsTargetInAttackRange(enemy.gameObject))
        {
            StartCoroutine(MoveAndAttack(enemy));
        }

        //TODO Impliment Move to enemy and use skills
    }

    private void ChangeTargetEnemy(Enemy enemyToSet)
    {
        if (enemyToSet != enemy && enemy != null)
        {
            enemy.GetComponent<InteractiveEnemy>().SetHighLight(false);
        }

        enemy = enemyToSet;
        enemy.GetComponent<InteractiveEnemy>().SetHighLight(true);
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

    void RegisterForMouseEvents()
    {
        cameraRaycaster = FindObjectOfType<CameraRaycaster>();
        cameraRaycaster.onMouseOverDropItem += OnMouseOverDropItem;
        cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
    }

    private bool IsTargetInAttackRange(GameObject target)
    {
        float distanceToTarget = (target.transform.position - transform.position).magnitude;
        return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetMaxAttackRange();
    }

    IEnumerator MoveToTarget(GameObject target)
    {
        character.CurrentState = CharacterState.running;
        character.SetDestination(target.transform.position);
        
        if(target.GetComponent<Enemy>())
        {
            while (!IsTargetInAttackRange(target.gameObject))
            {
                yield return null;
            }
        }
        if(target.GetComponent<DropItem>())
        {
            while (!IsItemInPickUpRange(target.gameObject))
            {
                yield return null;
            }
        }

        character.SetDestination(character.transform.position);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator MoveAndAttack(Enemy enemy)
    {
        yield return StartCoroutine(MoveToTarget(enemy.gameObject));
        weaponSystem.AttackTarget(enemy.gameObject);
    }

    IEnumerator MoveAndPickUpItem(DropItem item)
    {
        yield return StartCoroutine(MoveToTarget(item.gameObject));
        inventorySystem.PickUpNewWeapon(item);
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

        Gizmos.color = new Color(0, 0, 255f, .5f);
        Gizmos.DrawWireSphere(transform.position, pickItemRadius);
    }
}
