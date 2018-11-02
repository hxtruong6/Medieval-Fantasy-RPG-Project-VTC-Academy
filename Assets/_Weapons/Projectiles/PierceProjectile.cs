using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceProjectile : MonoBehaviour
{
    const string TEMP_OBJECTS = "TempObjects";

    ProjectileConfig projectileConfig;
    WeaponConfig rangedWeaponConfig;
    GameObject shooter;//who fired this projectile
    List<Enemy> contactEnemies;
    GameObject effectOnEnemy;

    public void SetProjectileConfig(ProjectileConfig configToSet)
    {      
        projectileConfig = configToSet;
        contactEnemies = new List<Enemy>();
    }

    public void SetRangedWeaponConfig(WeaponConfig configToSet)
    {
        rangedWeaponConfig = configToSet;
    }

    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    public void SetEfectOnContact(GameObject effect)
    {
        effectOnEnemy = effect;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Enemy>())
        {
            var enemy = other.GetComponent<Enemy>();
            if (!contactEnemies.Contains(enemy))
            {
                DealDamage(other.gameObject);
                contactEnemies.Add(enemy);
                PlayParticleEffect(enemy.gameObject, effectOnEnemy);
            }
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
    }
}
