using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    ProjectileConfig projectileConfig;
    WeaponConfig rangedWeaponConfig;
    GameObject shooter;//who fired this projectile

    public void SetProjectileConfig(ProjectileConfig configToSet)
    {      
        projectileConfig = configToSet;
    }

    public void SetRangedWeaponConfig(WeaponConfig configToSet)
    {
        rangedWeaponConfig = configToSet;
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
            print("Get dame");
            DealDamage(collision.gameObject);       
        }
    }

    private void DealDamage(GameObject objectBeingHit)
    {
        if(objectBeingHit.GetComponent<PlayerControl>())
        {
            print("2");
            
        }

        print(objectBeingHit.GetComponent<HealthSystem>());
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
            print("Dame: " + damage);
        }

        Destroy(gameObject);
    }
}
