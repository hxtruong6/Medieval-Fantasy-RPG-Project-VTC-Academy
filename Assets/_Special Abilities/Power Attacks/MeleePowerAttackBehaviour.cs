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

    private void HitPowerAttack()
    {
        PlayEffectOnEnemy(target);
        target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
    }
}
