using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Medieval-Fantasy-RPG/Special Ability/Power Attack/Ranged"))]
public class RangedPowerAttackConfig : AbilityConfig
{
    [Header("Ranged Power Attack Specific")]
    [SerializeField] float extraDamage = 10f;
    [SerializeField] ProjectileConfig projectileConfig;

    public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
    {
        return objectToAttachTo.AddComponent<RangedPowerAttackBehaviour>();
    }

    public float GetExtraDamage()
    {
        return extraDamage;
    }

    public ProjectileConfig GetProjectileConfig()
    {
        return projectileConfig;
    }
}
