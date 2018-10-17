using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerAttackBehaviour : AbilityBehaviour
{
    ParticleSystem myParticleSystem;

    public override void Use(GameObject target)//AbilityUseParams useParams)
    {
        DealDamage(target);
        PlayParticleEffect();
        PlayAbilitySound();
        PlayAbilityAnimation();
    }

    private void DealDamage(GameObject target)//AbilityUseParams useParams)
    {
        float damageToDeal = /*useParams.baseDamage +*/ (config as PowerAttackConfig).GetExtraDamage();
        /* useParams.*/
        target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
    }
}
