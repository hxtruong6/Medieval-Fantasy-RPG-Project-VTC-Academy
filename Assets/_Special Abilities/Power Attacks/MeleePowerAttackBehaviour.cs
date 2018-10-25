using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePowerAttackBehaviour : AbilityBehaviour
{
    float damageToDeal = 0;
    GameObject target;

    public override void Use(AbilityUseParams useParams)
    {
        transform.LookAt(useParams.target.transform);
        GetReferences(useParams);
        PlayEffectOnEnemy(useParams.target);
        PlayEffectOnSelf(gameObject);
        PlayEffectOnWeapon(GetComponent<WeaponSystem>().GetCurrentWeaponObject());
        PlayAbilitySound();
        PlayAbilityAnimation();
    }

    private void GetReferences(AbilityUseParams useParams)
    {
        damageToDeal = useParams.baseDamage + (config as MeleePowerAttackConfig).GetExtraDamage();
        target = useParams.target;
    }

    private void PlayEffectOnWeapon(GameObject target)
    {
        if (GetEffectOnWeapon() == null)
            return;

        var particlePrefab = config.GetEffectOnWeapon();
        var particleObject = Instantiate
        (
            particlePrefab,
            target.transform.position,
            target.transform.rotation,
            target.transform
        );
        particleObject.GetComponent<ParticleSystem>().Play();
        var particleCleanUpTime = (config as MeleePowerAttackConfig).GetEffectDestroyTime();
        Destroy(particleObject, particleCleanUpTime);
    }

    private void PlayEffectOnEnemy(GameObject target)
    {
        if (GetEffectOnEnemy() == null)
            return;

        PlayParticleEffect(target, GetEffectOnEnemy());
    }

    private void PlayEffectOnSelf(GameObject target)
    {
        if (GetEffectOnSelf() == null)
            return;

        PlayParticleEffect(target, GetEffectOnSelf());
    }

    private void HitPowerAttack()
    {
        target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
    }
}
