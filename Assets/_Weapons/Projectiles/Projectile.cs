using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    const string TEMP_OBJECTS = "TempObjects";

    ProjectileConfig projectileConfig;
    WeaponConfig rangedWeaponConfig;
    GameObject effectOnEnemy;
    GameObject shooter;

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
        particleObject.transform.parent = GameObject.FindGameObjectWithTag(TEMP_OBJECTS).transform;
        particleObject.GetComponent<ParticleSystem>().Play();
    }

    void OnCollisionEnter(Collision collision)
    {
        var layerCollidedWith = collision.gameObject.layer;
        if (shooter && layerCollidedWith != shooter.layer)
        {
            DealDamage(collision.gameObject);

            if(effectOnEnemy)
                PlayParticleEffect(collision.gameObject, effectOnEnemy);
        }
    }

    private void DealDamage(GameObject objectBeingHit)
    {
        if (!objectBeingHit.GetComponent<HealthSystem>() ||
            objectBeingHit.GetComponent<HealthSystem>().HealthAsPercentage < 0)
        {
            return;
        }
     
        var shooterWeapon = shooter.GetComponent<WeaponSystem>();
        float damage = 0;
        
        if (projectileConfig.isAbilityProjectile)
        {
            damage = shooterWeapon.GetWeaponDamage();
            damage += shooter.GetComponent<Character>().GetBaseDamage();
            damage += shooter.GetComponent<RangedPowerAttackBehaviour>().GetAbilityDamage();
            objectBeingHit.GetComponent<HealthSystem>().TakeDamage(damage);
        }
        else
        {
            shooterWeapon.SetTarget(objectBeingHit);
            shooterWeapon.Hit(rangedWeaponConfig);
        }

        Destroy(gameObject);
    }
}
