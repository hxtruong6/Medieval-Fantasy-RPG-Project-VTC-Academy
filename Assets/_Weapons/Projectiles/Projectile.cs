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
        var damage = shooter.GetComponent<WeaponSystem>().CalculateDamage();
        if(objectBeingHit.GetComponent<HealthSystem>())
        {
            if(objectBeingHit.GetComponent<HealthSystem>().healthAsPercentage > 0)
            {
                objectBeingHit.GetComponent<HealthSystem>().TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}
