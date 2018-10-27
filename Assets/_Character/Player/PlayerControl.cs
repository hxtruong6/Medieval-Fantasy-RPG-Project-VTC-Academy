using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float pickItemRadius = 2.5f;

    Character character;
    Enemy enemy;
    DropItem dropItem;
    WeaponSystem weaponSystem;
    InventorySystem inventorySystem;   
    SpecialAbilities abilities;

    KeyCode switchWeaponKey = KeyCode.Tab;
    KeyCode meleeAOESkillKey = KeyCode.Q;
    KeyCode rangedAOESkillKey = KeyCode.W;

    bool isAlive = true;

    void Start()
    {
        character = GetComponent<Character>();
        weaponSystem = GetComponent<WeaponSystem>();
        inventorySystem = GetComponent<InventorySystem>();
        abilities = GetComponent<SpecialAbilities>();

        RegisterForMouseEvents();
    }

    void RegisterForMouseEvents()
    {
        var cameraRaycaster = FindObjectOfType<CameraRaycaster>();
        cameraRaycaster.onMouseOverDropItem += OnMouseOverDropItem;
        cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
        cameraRaycaster.onMouseOverPotentiallyWalkable += OnMouseOverPotentiallyWalkable;
    }

    private void StopMoving()
    {
        character.SetDestination(transform.position);
    }

    void Update()
    {
        if (Input.GetKeyDown(switchWeaponKey))
        {
            StopCurrentAction();
            inventorySystem.SwitchWeapon();
        }
        if(Input.GetKeyDown(meleeAOESkillKey))
        {
            if(weaponSystem.GetCurrentWeapon().IsMeleeWeapon())
                UseAoESkill(2);
        }
        if (Input.GetKeyDown(rangedAOESkillKey))
        {
            UseRangedAOESkill();
        }
    }

    private void UseRangedAOESkill()
    {
        StopMoving();
        StopCurrentAction();
        if (!weaponSystem.GetCurrentWeapon().IsMeleeWeapon())
            UseAoESkill(3);
    }

    private bool IsItemInPickUpRange(GameObject target)
    {
        float distanceToTarget = (target.transform.position - transform.position).magnitude;
        return distanceToTarget <= pickItemRadius;
    }

    private bool CheckAttackConditions(Enemy enemyToCheck)
    {
        if (!isAlive)
            return false;

        if (enemyToCheck.GetComponent<HealthSystem>().healthAsPercentage <= 0)
        {
            if (enemyToCheck == enemy)
                enemy.GetComponent<InteractiveEnemy>().HighLight(false);
            return false;
        }
        return true;
    }

    void OnMouseOverDropItem(DropItem itemToSet)
    {
        if (!isAlive)
            return;

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
        if (!CheckAttackConditions(enemyToSet))
            return;

        enemy = enemyToSet;

        if (Input.GetMouseButton(0) && IsTargetInAttackRange(enemy.gameObject))
        {
            NormalAttack(enemy);
        }
        else if (Input.GetMouseButton(0) && !IsTargetInAttackRange(enemy.gameObject))
        {
            StartCoroutine(MoveAndAttack(enemy));
        }
        else if (Input.GetMouseButtonDown(1) && IsTargetInAttackRange(enemy.gameObject))
        {
            UsePowerAttack(enemy);
        }
        else if (Input.GetMouseButtonDown(1) && !IsTargetInAttackRange(enemy.gameObject))
        {
            StartCoroutine(MoveAndPowerAttack(enemy)); ;
        }
        //TODO Impliment move to enemy and use target aoe skills
    }

    private void NormalAttack(Enemy enemy)
    {
        StopCurrentAction();
        StopMoving();
        transform.LookAt(enemy.transform);
        weaponSystem.AttackTarget(enemy.gameObject);
    }

    private void UsePowerAttack(Enemy target)
    {
        abilities.SetSkillTarget(target.gameObject);
        int skillIndex = weaponSystem.GetCurrentWeapon().IsMeleeWeapon() ? 0 : 1;
        abilities.AttemptSpecialAbility(skillIndex);
    }

    private void UseAoESkill(int skillIndex)
    {
        abilities.AttemptSpecialAbility(skillIndex);
    }

    void OnMouseOverPotentiallyWalkable(Vector3 destination)
    {
        if (!isAlive)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            StopCurrentAction();
            weaponSystem.CancleAction();
            character.CurrentState = CharacterState.running;
            character.SetDestination(destination);
        }
    }

    private void StopCurrentAction()
    {
        StopAllCoroutines();
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

        StopMoving();
        yield return new WaitForEndOfFrame();
    }

    IEnumerator MoveAndAttack(Enemy enemy)
    {
        yield return StartCoroutine(MoveToTarget(enemy.gameObject));
        weaponSystem.AttackTarget(enemy.gameObject);
    }

    IEnumerator MoveAndPowerAttack(Enemy target)
    {
        yield return StartCoroutine(MoveToTarget(enemy.gameObject));
        UsePowerAttack(target);
    }

    IEnumerator MoveAndPickUpItem(DropItem item)
    {
        yield return StartCoroutine(MoveToTarget(item.gameObject));
        inventorySystem.PickUpNewWeapon(item);
    }

    private void IdlingState()
    {
        character.CurrentState = CharacterState.idling;
    }

    public void Killed()
    {
        isAlive = false;
        StopCurrentAction();
        StopMoving();
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
