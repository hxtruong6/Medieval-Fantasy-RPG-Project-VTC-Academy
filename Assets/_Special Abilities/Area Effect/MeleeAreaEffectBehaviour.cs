﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAreaEffectBehaviour : AbilityBehaviour
{
    float damageToDeal = 0;
    GameObject target;

    public override void Use(AbilityUseParams useParams)
    {
        GetReferences(useParams);
        PlayAbilitySound();
        PlayAbilityAnimation();
    }

    private void GetReferences(AbilityUseParams useParams)
    {
        damageToDeal = useParams.baseDamage + (config as MeleeAreaEffectConfig).GetDamageToEachTarget();
        target = useParams.target;
    }  

    private void DealRadialDamage()
    {
        //Static sphere cast for targets
        PlayEffectOnSelf(gameObject);
        RaycastHit[] hits = Physics.SphereCastAll
        (
            transform.position,
            (config as MeleeAreaEffectConfig).GetRadius(),
            Vector3.up,
            (config as MeleeAreaEffectConfig).GetRadius()
        );

        foreach (RaycastHit hit in hits)
        {
            var damageable = hit.collider.gameObject.GetComponent<HealthSystem>();
            bool hitPlayer = hit.collider.gameObject.GetComponent<PlayerControl>();

            if (damageable != null && !hitPlayer)
            {
                damageable.TakeDamage(damageToDeal);
                PlayEffectOnEnemy(damageable.gameObject);
            }
        }
    }
}
