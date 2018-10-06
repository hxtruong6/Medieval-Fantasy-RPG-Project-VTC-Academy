using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WeaponSystem : MonoBehaviour
{
    [Range(.1f, 1.0f)] [SerializeField] float criticalHitChance = 0.1f;
    [SerializeField] float criticalHitMultiplier = 1.25f;
    [SerializeField] ParticleSystem criticalHitParticle;

    [SerializeField] WeaponConfig currentWeaponConfig;

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

    //public Animator GetAnimator()
    //{
    //    return animator;
    //}

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

            float animSpeed = attackClip.length / currentWeaponConfig.GetMinTimeBetweenHits();
            animator.SetFloat("AttackSpeed", animSpeed);
            //animator.SetFloat("AnimOffSet", Random.Range(0f, 0.2f));
        }
    }

    public void CancleAttack()
    {
        if(character.GetComponent<PlayerControl>().StateOfPlayer == PlayerControl.PlayerState.attacking)
            animator.Play("Grounded", 0, 0);
    }

    public void Hit()
    {
        target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
    }

    public void StopAttacking()
    {        
        animator.StopPlayback();
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
        if(character.GetComponent<PlayerControl>())
        {
            character.GetComponent<PlayerControl>().StateOfPlayer = PlayerControl.PlayerState.attacking;
        }
    }

    private void RunAnimationAttackOnce()
    {
        transform.LookAt(target.transform);
        SetAttackAnimation();
        animator.SetTrigger(ATTACK_TRIGGER);
    }

    private float CalculateDamage()
    {
        bool isCriticalHit = Random.Range(0f, 1f) <= criticalHitChance;
        int weaponDamage = Random.Range(currentWeaponConfig.GetMinDamage(), currentWeaponConfig.GetMaxDamage());
        float damageBeforeCritical = character.GetBaseDamage() + weaponDamage;
        //if (isCriticalHit)
        //{
        //    print("Crit");
        //    criticalHitParticle.Play();
        //    return damageBeforeCritical * criticalHitMultiplier;
        //}
        //else
        //{
            return damageBeforeCritical;
        //}
    }
}