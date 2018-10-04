using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Medieval-Fantasy-RPG/Weapon"))]

public class WeaponConfig : ScriptableObject
{
    public Transform gripTransform;

    [SerializeField] GameObject weaponPrefab;
    [SerializeField] AnimationClip attackAnimation;
    [SerializeField] float minTimeBetweenHits = .5f;
    [SerializeField] float maxAttackRange = 2f;
    [SerializeField] float additionalDamage = 10f;

    public float GetMinTimeBetweenHits()
    {
        return minTimeBetweenHits;
    }

    public float GetMaxAttackRange()
    {
        return maxAttackRange;
    }

    public GameObject GetWeaponPrefab()
    {
        return weaponPrefab;
    }

    public AnimationClip GetAttackAnimClip()
    {
        //RemoveAnimationEvents();
        return attackAnimation;
    }

    public float GetAdditionalDamage()
    {
        return additionalDamage;
    }

    //In case animations has events
    //private void RemoveAnimationEvents()
    //{
    //    attackAnimation.events = new AnimationEvent[0];
    //}
}
