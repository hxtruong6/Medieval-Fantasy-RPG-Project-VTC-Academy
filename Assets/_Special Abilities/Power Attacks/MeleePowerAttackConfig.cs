using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Medieval-Fantasy-RPG/Special Ability/Power Attack/Melee"))]
public class MeleePowerAttackConfig : AbilityConfig
{
    [Header("Power Attack Specific")]
    [SerializeField] float extraDamage = 10f;

    public override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
    {
        return objectToAttachTo.AddComponent<MeleePowerAttackBehaviour>();
    }

    public float GetExtraDamage()
    {
        return extraDamage;
    }
}
