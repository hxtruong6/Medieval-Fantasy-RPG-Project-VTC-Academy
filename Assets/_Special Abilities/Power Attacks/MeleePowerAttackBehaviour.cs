using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleePowerAttackBehaviour : AbilityBehaviour
{
    ParticleSystem myParticleSystem;

    public override void Use(AbilityUseParams useParams)
    {
        transform.LookAt(useParams.target.transform);
        DealDamage(useParams);
        PlayParticleEffect(useParams.target);
        PlayAbilitySound();
        PlayAbilityAnimation();
    }

    private void DealDamage(AbilityUseParams useParams)
    {
        float damageToDeal = useParams.baseDamage + (config as MeleePowerAttackConfig).GetExtraDamage();
        useParams.target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
    }
}
