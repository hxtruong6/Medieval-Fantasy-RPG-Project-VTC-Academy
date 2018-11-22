using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PierceProjectile : MonoBehaviour
{
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
        particleObject.transform.parent = GameManager.instance.tempObjects;
        particleObject.GetComponent<ParticleSystem>().Play();
        StartCoroutine(DestroyParticleWhenFinished(particleObject));
    }

    IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
    {
        while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
        {
            yield return new WaitForSeconds(GameManager.instance.PARTICLE_CLEAN_UP_DELAY);
        }
        Destroy(particlePrefab);
        yield return new WaitForEndOfFrame();
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
        if(other.GetComponent<DragonBoss>())
        {
            DealDamage(other.gameObject);
            PlayParticleEffect(other.gameObject, effectOnEnemy);
        }
    }

    private void DealDamage(GameObject objectBeingHit)
    {
        if (!objectBeingHit.GetComponent<HealthSystem>() ||
            objectBeingHit.GetComponent<HealthSystem>().HealthAsPercentage < 0)
        {
            return;
        }

        AudioSource audioSource = GetComponentInParent<AudioSource>();
        audioSource.PlayOneShot(projectileConfig.GetContactSound());

        var shooterWeapon = shooter.GetComponent<WeaponSystem>();
        float damage = 0;
        
        if (projectileConfig.isAbilityProjectile)
        {
            damage = shooterWeapon.GetWeaponDamage();
            damage += shooter.GetComponent<Character>().GetBaseDamage();
            damage = damage * shooter.GetComponent<RangedAOEBehaviour>().GetAbilityDamage();
            objectBeingHit.GetComponent<HealthSystem>().TakeDamage(damage);
        }
        else
        {
            shooterWeapon.SetTarget(objectBeingHit);
            shooterWeapon.Hit(rangedWeaponConfig);
        }
    }
}
