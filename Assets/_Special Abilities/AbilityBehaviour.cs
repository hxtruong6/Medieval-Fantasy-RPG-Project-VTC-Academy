using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBehaviour : MonoBehaviour
{
    protected AbilityConfig config;

    const string ATTACK_TRIGGER = "Attack";
    const string DEFAULT_ATTACK_STATE = "DEFAULT ATTACK";
    const float PARTICLE_CLEAN_UP_DELAY = 20f;

    public abstract void Use(AbilityUseParams useParams); 

    public void SetConfig(AbilityConfig configToSet)
    {
        config = configToSet;
    }

    protected void PlayParticleEffect()
    {
        var particlePrefab = config.GetParticlePrefab();
        var particleObject = Instantiate
        (
            particlePrefab,
            transform.position,
            particlePrefab.transform.rotation
        );
        particleObject.transform.parent = transform;
        particleObject.GetComponent<ParticleSystem>().Play();
        StartCoroutine(DestroyParticleWhenFinished(particleObject));
    }

    IEnumerator DestroyParticleWhenFinished(GameObject particlePrefab)
    {
        while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
        {
            yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
        }
        Destroy(particlePrefab);
        yield return new WaitForEndOfFrame();
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
