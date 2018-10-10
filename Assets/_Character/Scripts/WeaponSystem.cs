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

    const string TEMP_OBJECTS = "TempObjects";
    const string ATTACK_TRIGGER = "Attack";
    const string DEFAULT_ATTACK = "DEFAULT ATTACK";

    GameObject target;
    GameObject weaponObject;
    Animator animator;
    Character character;
    float lastHitTime;

    void Start()
    {
        animator = GetComponent<Animator>();
        character = GetComponent<Character>();

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
    }

    private GameObject SpawnProjectile()
    {
        var projectilePrefab = currentProjectileConfig.GetProjectilePrefab();
        var firingPos = GetComponentInChildren<ArrowShootingPosition>();
        var projectileObject = Instantiate(projectilePrefab, firingPos.transform);

        var projectile = projectileObject.GetComponent<Projectile>();
        projectile.SetProjectileConfig(currentProjectileConfig);
        projectile.SetShooter(gameObject);

        projectileObject.transform.parent = GameObject.FindGameObjectWithTag(TEMP_OBJECTS).transform;
        return projectileObject;
    }

    public void FiringOffProjectile(ProjectileConfig projectileToUse)
    {
        currentProjectileConfig = projectileToUse;
        var projectileObject = SpawnProjectile();

        var targetToShoot = target.GetComponentInChildren<Renderer>().bounds.center;
        StartCoroutine(MoveObject(projectileObject,
                                  projectileObject.transform.position,
                                  targetToShoot,
                                  currentProjectileConfig.GetProjectileSpeed(),
                                  currentProjectileConfig.GetVanishTime()));
    }

    IEnumerator MoveObject(GameObject projectile, Vector3 from, Vector3 target, float speed, float vanishAfterSec)
    {
        float startTime = Time.time;
        var normalizeDirection = (target - from).normalized;
        var vanishTime = Time.time + vanishAfterSec;
        while (Time.time < vanishTime && projectile)
        {
            projectile.transform.position += normalizeDirection * (Time.deltaTime * speed);
            yield return null;
        }
        Destroy(projectile);
    }

    public WeaponConfig GetCurrentWeapon()
    {
        return currentWeaponConfig;
    }

    private void SetAttackAnimation()
    {
        if (!character.GetOverrideController())
        {
            Debug.Break();
            Debug.LogAssertion("Them animation override controller vao " + gameObject);
        }
        else
        {
            var animatorOverrideController = character.GetOverrideController();
            animator.runtimeAnimatorController = animatorOverrideController;
            var attackClip = currentWeaponConfig.GetAttackAnimClip();
            animatorOverrideController[DEFAULT_ATTACK] = attackClip;

            //float animSpeed = attackClip.length / currentWeaponConfig.GetMinTimeBetweenHits();
            //animator.SetFloat("AttackSpeed", animSpeed);
            //animator.SetFloat("AnimOffSet", Random.Range(0f, 0.2f));
        }
    }

    public void CancleAttack()
    {
        if (character.CurrentState == CharacterState.attacking)
        {
            animator.Play("Grounded");
        }
    }

    public void Hit()
    {
        target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
    }

    public void ShootArrow()
    {
        FiringOffProjectile(currentProjectileConfig);
    }

    public void StopAttacking()
    {
        //TODO impliment
    }

    public void AttackTarget(GameObject targetToAttack)
    {
        target = targetToAttack;

        bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage > 0;
        bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage > 0;

        if (attackerStillAlive && targetStillAlive)
        {
            float weaponHitPeriod = currentWeaponConfig.GetMinTimeBetweenHits();
            float timeToWait = weaponHitPeriod * character.GetAnimationSpeedMultiplier();

            if (Time.time - lastHitTime >= timeToWait)
            {
                lastHitTime = Time.time;
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
    }

    public float CalculateDamage()
    {
        bool isCriticalHit = Random.Range(0f, 1f) <= criticalHitChance;
        int weaponDamage = Random.Range(currentWeaponConfig.GetMinDamage(), currentWeaponConfig.GetMaxDamage());
        float damageBeforeCritical = character.GetBaseDamage() + weaponDamage;
        if (isCriticalHit)
        {
            PlayCriticalHitParticle();
            return damageBeforeCritical * criticalHitMultiplier;
        }
        else
        {
            return damageBeforeCritical;
        }
    }

    protected void PlayCriticalHitParticle()
    {
        var particlePrefab = currentWeaponConfig.GetCriticalHitPrefab();
        var particleObject = Instantiate
        (
            particlePrefab,
            target.transform.position,
            particlePrefab.transform.rotation
        );
        particleObject.transform.parent = target.transform;
        particleObject.GetComponent<ParticleSystem>().Play();
        particleObject.transform.parent = GameObject.FindGameObjectWithTag(TEMP_OBJECTS).transform;
        StartCoroutine(DestroyParticleAfterFinishedSec(particleObject));
    }

    IEnumerator DestroyParticleAfterFinishedSec(GameObject particlePrefab)
    {
        while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
        {
            yield return new WaitForSeconds(currentWeaponConfig.GetDestroyParticleTime());
        }
        Destroy(particlePrefab);
    }
}