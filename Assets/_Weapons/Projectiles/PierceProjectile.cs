using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceProjectile : MonoBehaviour
{
    ProjectileConfig projectileConfig;
    WeaponConfig rangedWeaponConfig;
    GameObject shooter;//who fired this projectile
    List<Enemy> contactEnemies;

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

    private void OnTriggerEnter(Collider other)
    {
        print("?");
        if (other.GetComponent<Enemy>())
        {
            var enemy = other.GetComponent<Enemy>();
            if (!contactEnemies.Contains(enemy))
            {
                DealDamage(other.gameObject);
                contactEnemies.Add(enemy);
            }
        }
    }

    private void DealDamage(GameObject objectBeingHit)
    {
        if (!objectBeingHit.GetComponent<HealthSystem>() ||
            objectBeingHit.GetComponent<HealthSystem>().healthAsPercentage < 0)
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
