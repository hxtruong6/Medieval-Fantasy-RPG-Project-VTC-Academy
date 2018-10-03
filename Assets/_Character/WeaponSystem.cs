using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WeaponSystem : MonoBehaviour
{
    [Range(.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
    [SerializeField] float criticalHitMultiplier = 1.25f;
    [SerializeField] ParticleSystem criticalHitParticle;

    [SerializeField] float baseDamage = 10f;
    [SerializeField] WeaponConfig currentWeaponConfig = null;

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

    void Update()
    {

    }

    private GameObject RequestDominantHand()
    {
        var dominantHands = GetComponentsInChildren<DominantHand>();
        int numberOfDominantHands = dominantHands.Length;
        Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand found on Player, please add one");
        Assert.IsFalse(numberOfDominantHands > 1, "Multiple DominantHand scripts on Player, please remove one");
        return dominantHands[0].gameObject;
    }

    public void PutWeaponInHand(WeaponConfig weaponToUse)
    {
        currentWeaponConfig = weaponToUse;
        var weaponPrefab = weaponToUse.GetWeaponPrefab();
        GameObject dominantHand = RequestDominantHand();
        Destroy(weaponObject);
        weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
        weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
        weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
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
            animatorOverrideController[DEFAULT_ATTACK] = currentWeaponConfig.GetAttackAnimClip();
        }
    }

    public void Hit()
    {
        target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
    }

    public void StopAttacking()
    {
        animator.StopPlayback();
    }

    public void AttackTargetOnce(GameObject targetToAttack)
    {
        target = targetToAttack;
        bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage > 0;
        bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage > 0;

        if (attackerStillAlive && targetStillAlive)
        {
            if (Time.time - lastHitTime >= currentWeaponConfig.GetMinTimeBetweenHits())
            {
                transform.LookAt(target.transform);
                SetAttackAnimation();
                animator.SetTrigger(ATTACK_TRIGGER);
                lastHitTime = Time.time;
            }
        }          
    }

    private float CalculateDamage()
    {
        bool isCriticalHit = Random.Range(0f, 1f) <= criticalHitChance;
        float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage();
        if (isCriticalHit)
        {
            criticalHitParticle.Play();
            return damageBeforeCritical * criticalHitMultiplier;
        }
        else
        {
            return damageBeforeCritical;
        }
    }
}