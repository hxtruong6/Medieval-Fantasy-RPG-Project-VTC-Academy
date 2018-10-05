using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Medieval-Fantasy-RPG/Weapon"))]

public class WeaponConfig : ScriptableObject
{
    public Transform gripTransform;

    [SerializeField] GameObject weaponPrefab;
    [SerializeField] AnimationClip[] attackAnimations;
    [SerializeField] float minTimeBetweenHits = 0.5f;
    [SerializeField] float maxAttackRange = 2f;
    [SerializeField] int minDamage = 1;
    [SerializeField] int maxDamage = 10;

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
        var clip = attackAnimations[Random.Range(0, attackAnimations.Length)];
        return clip;
    }

    public int GetMinDamage()
    {
        return minDamage;
    }

    public int GetMaxDamage()
    {
        return maxDamage;
    }
}
