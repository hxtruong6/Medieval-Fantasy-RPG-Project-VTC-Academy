using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WeaponSystem : MonoBehaviour
{
    [Range(.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
    [SerializeField] float criticalHitMultiplier = 1.25f;

    [SerializeField] WeaponConfig currentWeaponConfig;
    [SerializeField] ProjectileConfig currentProjectileConfig;
    [HideInInspector] public bool canAttack = true;

    const string ATTACK_TRIGGER = "Attack";
    const string ATTACK_SPEED = "AttackSpeed";
    const string DEFAULT_ATTACK = "DEFAULT ATTACK";
    const string ENEMY_LAYER_MASK = "Enemy";
    const string BOSS_LAYER_MASK = "Boss";
    GameObject target;
    GameObject weaponObject;
    Animator animator;
    AnimationClip attackClip;
    Character character;
    PlayerControl player;
    float? lastHitTime;

    public WeaponConfig GetCurrentWeapon() { return currentWeaponConfig; }

    public GameObject GetCurrentWeaponObject() { return weaponObject; }

    public ProjectileConfig GetCurrentProjectileConfig() { return currentProjectileConfig; }

    public void SetCurrentProjectileConfig(ProjectileConfig configToSet) { currentProjectileConfig = configToSet; }

    public void SetCurrentWeapon(WeaponConfig weapon) { currentWeaponConfig = weapon; }

    public float GetCriticalHitMultiplier() { return criticalHitMultiplier; }

    public void SetTarget(GameObject targetToChange) { target = targetToChange; }

    void Start()
    {
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();
        player = GetComponent<PlayerControl>();

        PutWeaponInHand(currentWeaponConfig);
        SetAttackAnimation();
    }

    private GameObject RequestRighHand()
    {
        var dominantHands = GetComponentsInChildren<RightHand>();
        int numberOfDominantHands = dominantHands.Length;
        Assert.IsFalse(numberOfDominantHands <= 0, "Khong tim thay RightHand script nao tren " + gameObject.name);
        Assert.IsFalse(numberOfDominantHands > 1, "Co qua nhieu RightHand script tren " + gameObject.name + " xoa bot di.");
        return dominantHands[0].gameObject;
    }

    private GameObject RequestLeftHand()
    {
        var dominantHands = GetComponentsInChildren<LeftHand>();
        int numberOfDominantHands = dominantHands.Length;
        Assert.IsFalse(numberOfDominantHands <= 0, "Khong tim thay LeftHand script nao tren " + gameObject.name);
        Assert.IsFalse(numberOfDominantHands > 1, "Co qua nhieu LeftHand script tren " + gameObject.name + " xoa bot di.");
        return dominantHands[0].gameObject;
    }

    public void PutWeaponInHand(WeaponConfig weaponToUse)
    {
        currentWeaponConfig = weaponToUse;
        //Assert.IsTrue(weaponToUse.GetWeaponPrefab() == null, "There is no current weapon");
        var weaponPrefab = weaponToUse.GetWeaponPrefab();
        GameObject dominantHand;
        if (currentWeaponConfig.IsRightHandWeapon())
        {
            dominantHand = RequestRighHand();
        }
        else
        {
            dominantHand = RequestLeftHand();
        }
        Destroy(weaponObject);
        weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
        weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
        weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        weaponObject.layer = 0;
        if(weaponObject.GetComponent<InfoItem>())
            weaponObject.GetComponent<InfoItem>().RemoveInfo();
    }

    private GameObject SpawnProjectile()
    {
        var projectilePrefab = currentProjectileConfig.GetProjectilePrefab();
        var firingPos = GetComponentInChildren<ArrowShootingPosition>();
        var projectileObject = Instantiate(projectilePrefab, firingPos.transform);

        var projectile = projectileObject.GetComponentInChildren<Projectile>();
        projectile.SetProjectileConfig(currentProjectileConfig);
        projectile.SetRangedWeaponConfig(currentWeaponConfig);
        projectile.SetShooter(gameObject);

        projectileObject.transform.parent = GameManager.instance.tempObjects;
        return projectileObject;
    }

    IEnumerator MoveProjectile(GameObject projectile, Vector3 from, Vector3 targetToShoot, float speed, float vanishAfterSec)
    {
        float startTime = Time.time;
        var normalizeDirection = (targetToShoot - from).normalized;
        var vanishTime = Time.time + vanishAfterSec;
        projectile.transform.LookAt(targetToShoot);

        while (Time.time < vanishTime && projectile != null)
        {
            projectile.transform.position += normalizeDirection * (Time.deltaTime * speed);
            yield return null;
        }
        Destroy(projectile);
    }

    public void SetProjectileDirection(ProjectileConfig projectileToUse)
    {
        currentProjectileConfig = projectileToUse;
        var projectileObject = SpawnProjectile();

        var targetToShoot = target.GetComponentInChildren<MainBody>();
        var targetCenter = targetToShoot.GetComponent<Renderer>().bounds.center;

        StartCoroutine(MoveProjectile(projectileObject,
                                        projectileObject.transform.position,
                                        targetCenter,
                                        currentProjectileConfig.GetProjectileSpeed(),
                                        currentProjectileConfig.GetVanishTime()));
    }

    private void ShootArrow()
    {
        SetProjectileDirection(currentProjectileConfig);
    }

    public void SetAttackAnimation()
    {
        if (!character.GetOverrideController())
        {
            Debug.Break();
            Debug.LogAssertion("Them animation override controller vao " + gameObject);
        }
        else
        {
            var animatorOverrideController = character.GetOverrideController();
            if (player && player.isInDemonForm)
                animatorOverrideController = GetComponent<DemonTrigger>().GetOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            attackClip = currentWeaponConfig.GetAttackAnimClip();
            animatorOverrideController[DEFAULT_ATTACK] = attackClip;
            animator.SetFloat(ATTACK_SPEED, currentWeaponConfig.GetAttackAnimSpeedMultiplier());
        }
    }

    public void CancleAction()
    {
        canAttack = true;
        character.SetDestination(character.transform.position);
        animator.Play("Grounded");
    }

    public bool IsCriticalHit()
    {
        bool isCriticalHit = Random.Range(0f, 1f) <= criticalHitChance;
        return isCriticalHit;
    }

    public void Hit(WeaponConfig dealDamageWeapon = null)
    {
        float damageToDeal = 0;
        var criticalPrefab = currentWeaponConfig.GetCriticalHitPrefab();
        var criticalEffectTime = currentWeaponConfig.GetDestroyParticleTime();

        if (dealDamageWeapon == null)
            damageToDeal = NormalAttackDamage();
        else
        {
            damageToDeal = NormalAttackDamage(dealDamageWeapon);
            criticalPrefab = dealDamageWeapon.GetCriticalHitPrefab();
            criticalEffectTime = dealDamageWeapon.GetDestroyParticleTime();
        }

        if (IsCriticalHit())
        {
            damageToDeal = damageToDeal * criticalHitMultiplier;

            target.GetComponent<HealthSystem>().PlayHitEffect(
                criticalPrefab,
                criticalEffectTime);
        }

        if(player && player.isInDemonForm)
        {
            DealDamageAroundTarget(target, GetComponent<DemonTrigger>().normalMeleeAOERadius, damageToDeal);          
        }
        else
        {
            if(player)
                GetComponent<RageSystem>().GainRagePoints(GetComponent<RageSystem>().attackedGain);
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }
    }

    private void DealDamageAroundTarget(GameObject target, float dealDamageRadius, float damageToDeal)
    {
        Collider[] hitColliders;
        hitColliders = Physics.OverlapSphere(target.transform.position, dealDamageRadius, LayerMask.GetMask(ENEMY_LAYER_MASK));
        
        List<Enemy> affectedTarget = new List<Enemy>();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            var enemy = hitColliders[i].GetComponent<Enemy>();
            
            if (affectedTarget.Contains(enemy))
            {
                continue;
            }
            if(enemy)
            {
                enemy.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
                affectedTarget.Add(enemy);
            }
        }

        hitColliders = Physics.OverlapSphere(target.transform.position, dealDamageRadius, LayerMask.GetMask(BOSS_LAYER_MASK));
        List<WyvernBehavior> affectedBoss = new List<WyvernBehavior>();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            var boss = hitColliders[i].GetComponent<WyvernBehavior>();
            if (affectedBoss.Contains(boss)) continue;

            if(boss)
            {
                boss.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
                affectedBoss.Add(boss);
            }          
        }

        var effect = GetComponentInParent<DemonTrigger>().normalMeleeAttackEffect;
        if(effect)
            target.GetComponent<HealthSystem>().PlayHitEffect(effect, 10);
    }

    IEnumerator ChangeAttackStatus()
    {
        float weaponHitPeriod = attackClip.length + (1 - currentWeaponConfig.GetAttackAnimSpeedMultiplier()) * attackClip.length;
        float timeToWait = weaponHitPeriod + currentWeaponConfig.GetMinTimeBetweenHits();
        yield return new WaitForSeconds(timeToWait);
        canAttack = true;
    }


    public void AttackTarget(GameObject targetToAttack)
    {
        if (weaponObject == null)
            PutWeaponInHand(currentWeaponConfig);

        target = targetToAttack;

        bool attackerStillAlive = GetComponent<HealthSystem>().HealthAsPercentage > 0;
        bool targetStillAlive = target.GetComponent<HealthSystem>().HealthAsPercentage > 0;

        if (attackerStillAlive && targetStillAlive)
        {
            if (canAttack)
            {
                canAttack = false;
                RunAnimationAttackOnce();
            }
        }
        if (character.GetComponent<PlayerControl>())
        {
            character.CurrentState = CharacterState.attacking;
        }
    }

    private void RunAnimationAttackOnce()
    {
        transform.LookAt(target.transform);
        SetAttackAnimation();
        animator.SetTrigger(ATTACK_TRIGGER);
        StartCoroutine(ChangeAttackStatus());
    }

    public int GetWeaponDamage(WeaponConfig weapon = null)
    {
        if (weapon == null)
            return Random.Range(currentWeaponConfig.GetMinDamage(), currentWeaponConfig.GetMaxDamage());
        else
            return Random.Range(weapon.GetMinDamage(), weapon.GetMaxDamage());
    }

    public float NormalAttackDamage(WeaponConfig weapon = null)
    {
        if (weapon == null)
            return character.GetBaseDamage() + GetWeaponDamage();
        else
            return character.GetBaseDamage() + GetWeaponDamage(weapon);
    }

    public void DestroyWeaponObject()
    {
        Destroy(weaponObject);
    }
}