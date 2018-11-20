using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("Medieval-Fantasy-RPG/Projectile"))]

public class ProjectileConfig : ScriptableObject
{   
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] AnimationClip[] flyingAnimations;
    [SerializeField] float speed;
    [SerializeField] float vanishAfterSec = 1f;
    [SerializeField] AudioClip contactSound;
    [SerializeField] bool isPierceProjectile;

    public bool isAbilityProjectile;

    public GameObject GetProjectilePrefab()
    {
        return projectilePrefab;
    }

    public bool IsPiereceProjectile()
    {
        return isPierceProjectile;
    }

    public float GetProjectileSpeed()
    {
        return speed;
    }

    public float GetVanishTime()
    {
        return vanishAfterSec;
    }

    public AnimationClip GetAttackAnimClip()
    {
        if(flyingAnimations.Length > 0)
        {
            var clip = flyingAnimations[Random.Range(0, flyingAnimations.Length)];
            return clip;
        }
        return null;
    }

    public AudioClip GetContactSound()
    {
        return contactSound;
    }
}
