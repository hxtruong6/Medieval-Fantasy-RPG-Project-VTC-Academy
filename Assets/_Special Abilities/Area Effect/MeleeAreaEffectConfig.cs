using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Medieval-Fantasy-RPG/Special Ability/Area Effect/Melee"))]

public class MeleeAreaEffectConfig : AbilityConfig
{
    [Header("Area Effect Specific")]
    [SerializeField] float radius = 5f;
    [SerializeField] float damageToEachTarget = 15f;

    public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
    {
        return objectToAttachTo.AddComponent<MeleeAreaEffectBehaviour>();
    }

    public float GetDamageToEachTarget()
    {
        return damageToEachTarget;
    }

    public float GetRadius()
    {
        return radius;
    }
}
