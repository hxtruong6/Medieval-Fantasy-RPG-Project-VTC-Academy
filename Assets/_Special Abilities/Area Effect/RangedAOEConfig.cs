using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Medieval-Fantasy-RPG/Special Ability/Area Effect/Ranged"))]
public class RangedAOEConfig : AbilityConfig
{
    [Header("Ranged AOE Attack Specific")]
    [SerializeField] float damageToEachTarget = 10f;
    [SerializeField] ProjectileConfig projectileConfig;
    [SerializeField] float degreeBetweenArrows = 30f;

    public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
    {
        return objectToAttachTo.AddComponent<RangedAOEBehaviour>();
    }

    public float GetDamageToEachTarget()
    {
        return damageToEachTarget;
    }

    public ProjectileConfig GetProjectileConfig()
    {
        return projectileConfig;
    }

    public float GetDegreeBetweenArrows()
    {
        return degreeBetweenArrows;
    }
}
