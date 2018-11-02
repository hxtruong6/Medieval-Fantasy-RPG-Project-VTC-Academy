using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviour : MonoBehaviour
{
    protected AbilityConfig config;

    const string ATTACK_TRIGGER = "Attack";
    const string DEFAULT_ATTACK_STATE = "DEFAULT ATTACK";
    const string TEMP_OBJECTS = "TempObjects";

    public abstract void Use(AbilityUseParams useParams); 

    public void SetConfig(AbilityConfig configToSet)
    {
        config = configToSet;
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

    protected GameObject GetEffectOnSelf()
    {
        return config.GetEffectOnSelf();
    }

    protected void PlayEffectOnSelf(GameObject target)
    {
        if (GetEffectOnSelf() == null)
            return;

        PlayParticleEffect(target, GetEffectOnSelf());
    }

    protected GameObject GetEffectOnEnemy()
    {
        return config.GetEffectOnEnemy();
    }

    protected void PlayEffectOnEnemy(GameObject target)
    {
        if (GetEffectOnEnemy() == null)
            return;

        PlayParticleEffect(target, GetEffectOnEnemy());
    }

    protected float GetEnergyCost()
    {
        return config.GetEnergyCost();
    }

    protected GameObject GetEffectOnWeapon()
    {
        return config.GetEffectOnWeapon();
    }

    protected void PlayEffectOnWeapon(GameObject target)
    {
        if (GetEffectOnWeapon() == null)
            return;

        var particlePrefab = config.GetEffectOnWeapon();
        var particleObject = Instantiate
        (
            particlePrefab,
            target.transform.position,
            target.transform.rotation,
            target.transform
        );
        particleObject.GetComponent<ParticleSystem>().Play();
        var particleCleanUpTime = (config as MeleePowerAttackConfig).GetEffectDestroyTime();
        Destroy(particleObject, particleCleanUpTime);
    }

    protected void PlayAbilitySound()
    {
        var abilitySound = config.GetRandomAbilitySound();
        var audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(abilitySound);
    }

    protected void PlayAbilityAnimation()
    {
        var animatorOverrideController = GetComponent<Character>().GetOverrideController();
        var animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = animatorOverrideController;
        animatorOverrideController[DEFAULT_ATTACK_STATE] = config.GetAbilityAnimation();
        animator.SetTrigger(ATTACK_TRIGGER);
    }
}
