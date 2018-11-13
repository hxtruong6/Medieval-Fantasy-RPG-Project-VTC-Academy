using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] float pickItemRadius = 2.5f;
    [SerializeField] KeyCode switchWeaponKey = KeyCode.Tab;
    [SerializeField] KeyCode meleeAOESkillKey = KeyCode.Q;
    [SerializeField] KeyCode rangedAOESkillKey = KeyCode.W;
    [SerializeField] KeyCode useSmallHealthKey = KeyCode.Alpha1;
    [SerializeField] KeyCode useLargeHealthKey = KeyCode.Alpha2;
    [SerializeField] KeyCode useSmallManaKey = KeyCode.Alpha3;
    [SerializeField] KeyCode useLargeManaKey = KeyCode.Alpha4;

    Character character;
    GameObject enemy;
    WeaponSystem weaponSystem;
    InventorySystem inventorySystem;
    SpecialAbilities abilities;



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
        //cameraRaycaster.onMouseOverBoss += OnMouseOverBoss;
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
        if (Input.GetKeyDown(meleeAOESkillKey))
        {
            if (weaponSystem.GetCurrentWeapon().IsMeleeWeapon())
                UseAoESkill(2);
        }
        if (Input.GetKeyDown(rangedAOESkillKey))
        {
            UseRangedAOESkill();
        }

        if (Input.GetKeyDown(useSmallHealthKey))
        {
            inventorySystem.UseSmallHealthPotion();
        }
        if (Input.GetKeyDown(useLargeHealthKey))
        {
            inventorySystem.UseLargeHealthPotion();
        }

        if (Input.GetKeyDown(useSmallManaKey))
        {
            inventorySystem.UseSmallManaPotion();
        }
        if (Input.GetKeyDown(useLargeManaKey))
        {
            inventorySystem.UseLargeManaPotion();
        }
    }

    private void UseRangedAOESkill()
    {
        StopMoving();
        StopCurrentAction();
        if (!weaponSystem.GetCurrentWeapon().IsMeleeWeapon())
            UseAoESkill(3);
    }

    private bool CheckAttackConditions(GameObject enemyToCheck)
    {
        if (!isAlive)
            return false;

        if (enemyToCheck.GetComponent<HealthSystem>().HealthAsPercentage <= 0)
            return false;

        return true;
    }

    private bool IsItemInPickUpRange(GameObject target)
    {
        float distanceToTarget = (target.transform.position - transform.position).magnitude;
        return distanceToTarget <= pickItemRadius;
    }

    void OnMouseOverDropItem(LootItem itemToPick)
    {
        if (!isAlive)
            return;

        if (Input.GetMouseButtonDown(0) && IsItemInPickUpRange(itemToPick.gameObject))
        {
            inventorySystem.PickUpNewItem(itemToPick);
        }
        else if (Input.GetMouseButtonDown(0) && !IsItemInPickUpRange(itemToPick.gameObject))
        {
            StartCoroutine(MoveAndPickUpItem(itemToPick));
        }
    }

    void OnMouseOverEnemy(GameObject enemyToSet)
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

    private void NormalAttack(GameObject enemy)
    {
        if (weaponSystem.canAttack == false)
            return;

        StopCurrentAction();
        StopMoving();
        transform.LookAt(enemy.transform);
        weaponSystem.AttackTarget(enemy.gameObject);
    }

    private void UsePowerAttack(GameObject target)
    {
        StopMoving();
        StopCurrentAction();
        abilities.SetSkillTarget(target);
        int skillIndex = weaponSystem.GetCurrentWeapon().IsMeleeWeapon() ? 0 : 1;
        abilities.AttemptSpecialAbility(skillIndex);
    }

    private void UseAoESkill(int skillIndex)
    {
        StopMoving();
        StopCurrentAction();
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

        if (target.GetComponent<Enemy>())
        {
            while (!IsTargetInAttackRange(target.gameObject))
            {
                yield return null;
            }
        }
        if (target.GetComponent<WyvernBehavior>())
        {
            while (!IsTargetInAttackRange(target.gameObject))
            {
                yield return null;
            }
        }
        if (target.GetComponent<LootItem>())
        {
            while (!IsItemInPickUpRange(target.gameObject))
            {
                yield return null;
            }
        }

        StopMoving();
        yield return new WaitForEndOfFrame();
    }

    IEnumerator MoveAndAttack(GameObject enemy)
    {
        yield return StartCoroutine(MoveToTarget(enemy));
        weaponSystem.AttackTarget(enemy);
    }

    IEnumerator MoveAndPowerAttack(GameObject target)
    {
        yield return StartCoroutine(MoveToTarget(enemy));
        UsePowerAttack(target);
    }

    IEnumerator MoveAndPickUpItem(LootItem item)
    {
        yield return StartCoroutine(MoveToTarget(item.gameObject));
        inventorySystem.PickUpNewItem(item);
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
