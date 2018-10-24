using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    ProjectileConfig projectileConfig;
    GameObject shooter;//who fired this projectile

    public void SetProjectileConfig(ProjectileConfig configToSet)
    {
        projectileConfig = configToSet;
    }

    public void SetShooter(GameObject shooter)
    {
        this.shooter = shooter;
    }

    void OnCollisionEnter(Collision collision)
    {
        var layerCollidedWith = collision.gameObject.layer;
        if (shooter && layerCollidedWith != shooter.layer)
        {
            DealDamage(collision);           
        }
    }

    private void DealDamage(Collision collision)
    {
        var objectBeingHit = collision.gameObject;

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
        }
        else
        {
            damage = shooterWeapon.NormalAttackDamage();
            if (shooterWeapon.IsCriticalHit())
            {               
                objectBeingHit.GetComponent<HealthSystem>().PlayCriticalHitParticle(
                    shooterWeapon.GetCurrentWeapon().GetCriticalHitPrefab(),
                    shooterWeapon.GetCurrentWeapon().GetDestroyParticleTime());
                damage = damage * shooterWeapon.GetCriticalHitMultiplier();
            }
        }

        objectBeingHit.GetComponent<HealthSystem>().TakeDamage(damage);
        Destroy(gameObject);
    }
}
