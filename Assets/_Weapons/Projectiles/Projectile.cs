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
            Destroy(gameObject);
        }
    }

    private void DealDamage(Collision collision)
    {
        var objectBeingHit = collision.gameObject;
        var damage = shooter.GetComponent<WeaponSystem>().CalculateDamage();
        if(objectBeingHit.GetComponent<HealthSystem>())
            objectBeingHit.GetComponent<HealthSystem>().TakeDamage(damage);
    }
}
