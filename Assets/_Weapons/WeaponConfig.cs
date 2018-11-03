using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] bool isRightHandWeapon = true;
    [SerializeField] bool isMeleeWeapon = true;
    [SerializeField] GameObject criticalHitParticlePrefab;
    [SerializeField] float destroyParticleAfterSec;
    [SerializeField] float attackAnimSpeedMultiplier = 1;
    [SerializeField] Sprite weaponIcon;
    [SerializeField] GameObject dropParticlePrefab;

    public float GetMinTimeBetweenHits() { return minTimeBetweenHits; }

    public float GetMaxAttackRange() { return maxAttackRange; }

    public GameObject GetWeaponPrefab() { return weaponPrefab; }

    public AnimationClip GetAttackAnimClip()
    {
        var clip = attackAnimations[Random.Range(0, attackAnimations.Length)];
        return clip;
    }

    public GameObject GetCriticalHitPrefab() { return criticalHitParticlePrefab; }

    public float GetDestroyParticleTime() { return destroyParticleAfterSec; }

    public int GetMinDamage() { return minDamage; }

    public int GetMaxDamage() { return maxDamage; }

    public bool IsRightHandWeapon() { return isRightHandWeapon; }

    public bool IsMeleeWeapon() { return isMeleeWeapon; }

    public float GetAttackAnimSpeedMultiplier() { return attackAnimSpeedMultiplier; }

    public Sprite GetWeaponIcon() { return weaponIcon; }

    public GameObject GetDropParticlePrefab() { return dropParticlePrefab; }
}
