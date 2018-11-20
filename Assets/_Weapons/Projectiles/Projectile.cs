using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    ProjectileConfig projectileConfig;
    WeaponConfig rangedWeaponConfig;
    GameObject effectOnEnemy;
    GameObject shooter;
    List<GameObject> hitEnemy = new List<GameObject>();

    public void SetProjectileConfig(ProjectileConfig configToSet)
    {      
        projectileConfig = configToSet;
    }

    public void SetRangedWeaponConfig(WeaponConfig configToSet)
    {
        rangedWeaponConfig = configToSet;
    }

    public void SetEfectOnContact(GameObject effect)
    {
        effectOnEnemy = effect;
    }

    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    protected void PlayParticleEffect(GameObject target, GameObject effect)
    {
        var particlePrefab = effect;
        var particleObject = Instantiate
        (
            particlePrefab,
            target.transform.position,
            particlePrefab.transform.rotation
        );
        particleObject.transform.parent = target.transform;
        particleObject.transform.parent = GameManager.instance.tempObjects;
        particleObject.GetComponent<ParticleSystem>().Play();
        StartCoroutine(DestroyParticleWhenFinished(particleObject));
    }

    IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
    {
        while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
        {
            yield return new WaitForSeconds(GameManager.instance.PARTICLE_CLEAN_UP_DELAY);
        }
        Destroy(particlePrefab);
        yield return new WaitForEndOfFrame();
    }

    private void OnTriggerEnter(Collider other)
    {
        var layerCollidedWith = other.gameObject.layer;
        if (shooter && layerCollidedWith != shooter.layer)
        {
            DealDamage(other.gameObject);

            if(effectOnEnemy)
                PlayParticleEffect(other.gameObject, effectOnEnemy);
        }
    }


    private void DealDamage(GameObject objectBeingHit)
    {
        if (!objectBeingHit.GetComponent<HealthSystem>() ||
            objectBeingHit.GetComponent<HealthSystem>().HealthAsPercentage < 0)
        {
            return;
        }
        float damage = 0;
        var enemyBodyPart = objectBeingHit.GetComponent<BodyPart>();
        if (enemyBodyPart){
            
            switch (enemyBodyPart.partType)
            {
                case BodyPartType.Head:
                    damage += enemyBodyPart.headDamage;
                    break;
                case BodyPartType.Wing:
                    damage += enemyBodyPart.wingDamage;
                    break;
                case BodyPartType.Leg:
                    damage += enemyBodyPart.legDamage;
                    break;
                case BodyPartType.Forearm:
                    damage += enemyBodyPart.forearmDamage;
                    break;
                case BodyPartType.Tail:
                    damage += enemyBodyPart.tailDamage;
                    break;
                case BodyPartType.Body:
                    damage += enemyBodyPart.bodyDamage;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
         
        }
        AudioSource audioSource = GetComponentInParent<AudioSource>();
        audioSource.PlayOneShot(projectileConfig.GetContactSound());

        var shooterWeapon = shooter.GetComponent<WeaponSystem>();
             
        if (projectileConfig.isAbilityProjectile)
        {
            damage += shooterWeapon.GetWeaponDamage();
            damage += shooter.GetComponent<Character>().GetBaseDamage();
            damage = damage * shooter.GetComponent<RangedPowerAttackBehaviour>().GetAbilityDamage();
            objectBeingHit.GetComponent<HealthSystem>().TakeDamage(damage);
        }
        else
        {
            if (hitEnemy.Contains(objectBeingHit))
                return;

            shooterWeapon.SetTarget(objectBeingHit);
            shooterWeapon.Hit(rangedWeaponConfig);
            hitEnemy.Add(objectBeingHit);
        }
        // TODO: if enough damage -> show animation
        //if (enemyBodyPart)
        //{
        //    if (damage >  enoughToGetHit)
        //}
        if(!projectileConfig.IsPiereceProjectile())
            Destroy(gameObject);
    }
}
