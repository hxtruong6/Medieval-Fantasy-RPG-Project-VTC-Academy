using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedPowerAttackBehaviour : AbilityBehaviour
{
    const string TEMP_OBJECTS = "TempObjects";
    AbilityUseParams useParams;
    public override void Use(AbilityUseParams useParamsToSet)//GameObject target
    {
        useParams = useParamsToSet;
        transform.LookAt(useParams.target.transform);
        var weapon = GetComponent<WeaponSystem>();
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
        projectile.SetShooter(gameObject);

        projectileObject.transform.parent = GameObject.FindGameObjectWithTag(TEMP_OBJECTS).transform;
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

    public void SetProjectileDirection(ProjectileConfig configToUse)
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
        SetProjectileDirection((config as RangedPowerAttackConfig).GetProjectileConfig());
    }

    public float GetAbilityDamage()
    {
        float damageToDeal = (config as RangedPowerAttackConfig).GetExtraDamage();
        return damageToDeal;
    }
}
