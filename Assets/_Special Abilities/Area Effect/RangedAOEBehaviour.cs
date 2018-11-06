using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAOEBehaviour : AbilityBehaviour
{
    float numberOfArrows = 7;

    public override void Use(AbilityUseParams useParamsToSet)
    {
        PlayAbilitySound();
        PlayAbilityAnimation();
    }

    private GameObject SpawnProjectile(ProjectileConfig configToUse, float rotationY)
    {
        var projectileConfig = configToUse;
        var projectilePrefab = projectileConfig.GetProjectilePrefab();
        var firingPos = GetComponentInChildren<ArrowShootingPosition>();
        var projectileObject = Instantiate(projectilePrefab, firingPos.transform);

        var projectile = projectileObject.GetComponentInChildren<PierceProjectile>();
        projectile.SetProjectileConfig(projectileConfig);
        projectile.SetEfectOnContact(GetEffectOnEnemy());
        projectile.SetShooter(gameObject);

        Vector3 rotationVector = new Vector3(0, rotationY, 0);
        Quaternion rotation = Quaternion.Euler(rotationVector);
        projectileObject.transform.localRotation = rotation;

        projectileObject.transform.parent = GameManager.instance.tempObjects;
        return projectileObject;
    }

    IEnumerator MoveProjectile(GameObject projectile, Vector3 from, Vector3 target, float speed, float vanishAfterSec)
    {
        float startTime = Time.time;
        var normalizeDirection = (target - from).normalized;
        var vanishTime = Time.time + vanishAfterSec;
        while (Time.time < vanishTime && projectile != null)
        {
            projectile.transform.position += projectile.transform.forward * (Time.deltaTime * speed);
            yield return null;
        }
        Destroy(projectile);
    }

    private void SetProjectileDirection(ProjectileConfig configToUse, float rotationY)
    {
        var projectileObject = SpawnProjectile(configToUse, rotationY);
            
        var firingPos = GetComponentInChildren<ArrowShootingPosition>();
        var target = firingPos.transform.forward * 10000;
        target.y = GetComponentInChildren<MainBody>().GetComponent<Renderer>().bounds.center.y;
        StartCoroutine(MoveProjectile(projectileObject,
                                      projectileObject.transform.position,
                                      target,
                                      configToUse.GetProjectileSpeed(),
                                      configToUse.GetVanishTime()));
    }
    
    private void ShootAOEAttack()
    {
        GetComponent<EnergySystem>().ConsumeEnergy(GetEnergyCost());

        SetProjectileDirection((config as RangedAOEConfig).GetProjectileConfig(), 0);
        float degree = (config as RangedAOEConfig).GetDegreeBetweenArrows();
        for (int i = 1; i <= (numberOfArrows - 1) / 2; i++)
        {
            SetProjectileDirection((config as RangedAOEConfig).GetProjectileConfig(), i * degree);
        }
        for (int i = 1; i <= (numberOfArrows - 1) / 2; i++)
        {
            SetProjectileDirection((config as RangedAOEConfig).GetProjectileConfig(), -(i * degree));
        }
    }

    public float GetAbilityDamage()
    {
        float damageToDeal = (config as RangedAOEConfig).GetDamageToEachTarget();
        return damageToDeal;
    }
}
