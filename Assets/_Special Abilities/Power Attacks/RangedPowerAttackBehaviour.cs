using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedPowerAttackBehaviour : AbilityBehaviour
{
    AbilityUseParams useParams;
    public override void Use(AbilityUseParams useParamsToSet)
    {
        useParams = useParamsToSet;
        transform.LookAt(useParams.target.transform);
        PlayAbilitySound();
        PlayAbilityAnimation();
    }

    private GameObject SpawnProjectile(ProjectileConfig configToUse)
    {
        var projectileConfig = configToUse;
        var projectilePrefab = projectileConfig.GetProjectilePrefab();
        var firingPos = GetComponentInChildren<ArrowShootingPosition>();
        var projectileObject = Instantiate(projectilePrefab, firingPos.transform);

        var projectile = projectileObject.GetComponentInChildren<Projectile>();
        projectile.SetProjectileConfig(projectileConfig);
        projectile.SetEfectOnContact(GetEffectOnEnemy());
        projectile.SetShooter(gameObject);

        projectileObject.transform.parent = GameManager.instance.tempObjects;
        return projectileObject;
    }

    IEnumerator MoveProjectile(GameObject projectile, Vector3 from, Vector3 target, float speed, float vanishAfterSec)
    {
        float startTime = Time.time;
        var normalizeDirection = (target - from).normalized;
        var vanishTime = Time.time + vanishAfterSec;
        projectile.transform.LookAt(target);
        while (Time.time < vanishTime && projectile != null)
        {            
            projectile.transform.position += normalizeDirection * (Time.deltaTime * speed);
            yield return null;
        }
        Destroy(projectile);
    }

    private void SetProjectileDirection(ProjectileConfig configToUse)
    {
        var projectileObject = SpawnProjectile(configToUse);
        var target = useParams.target;

        var targetToShoot = target.GetComponentInChildren<MainBody>();
        var targetCenter = targetToShoot.GetComponentInChildren<Renderer>().bounds.center;

        StartCoroutine(MoveProjectile(projectileObject,
                                      projectileObject.transform.position,
                                      targetCenter,
                                      configToUse.GetProjectileSpeed(),
                                      configToUse.GetVanishTime()));
    }

    private void ShootPowerAttack()
    {
        GetComponent<EnergySystem>().ConsumeEnergy(GetEnergyCost());
        SetProjectileDirection((config as RangedPowerAttackConfig).GetProjectileConfig());
        GetComponent<RageSystem>().GainRagePoints(GetComponent<RageSystem>().powerAttackGain);
    }

    public float GetAbilityDamage()
    {
        float damageToDeal = (config as RangedPowerAttackConfig).GetExtraDamage();
        return damageToDeal;
    }
}
