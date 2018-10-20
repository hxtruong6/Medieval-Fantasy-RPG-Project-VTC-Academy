using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerAttackBehaviour : AbilityBehaviour
{
    ParticleSystem myParticleSystem;

    public override void Use(AbilityUseParams useParams)//GameObject target
    {
        transform.LookAt(useParams.target.transform);
        DealDamage(useParams);
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAbilityAnimation();
    }

    private void DealDamage(AbilityUseParams useParams)//GameObject target
    {
        float damageToDeal = useParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage();
        useParams.target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
    }
}
