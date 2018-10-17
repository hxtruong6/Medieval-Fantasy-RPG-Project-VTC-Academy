using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityConfig : ScriptableObject
{
    [Header("Special Ability General")]
    [SerializeField] float energyCost = 10f;
    [SerializeField] GameObject particlePrefab;
    [SerializeField] AnimationClip abilityAnimation;
    [SerializeField] AudioClip[] audioClips;

    protected AbilityBehaviour behaviour;

    public abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);

    public void AttachAbilityTo(GameObject objectToAttachTo)
    {
        AbilityBehaviour behaviourComponent = GetBehaviourComponent(objectToAttachTo);
        behaviourComponent.SetConfig(this);
        behaviour = behaviourComponent;
    }

    //public void Use(AbilityUseParams useParams)
    //{
    //    behaviour.Use(useParams);
    //}

    public void Use(GameObject target)
    {
        behaviour.Use(target);
    }

    public float GetEnergyCost()
    {
        return energyCost;
    }

    public GameObject GetParticlePrefab()
    {
        return particlePrefab;
    }

    public AudioClip GetRandomAbilitySound()
    {
        if(audioClips.Length > 0)
            return audioClips[Random.Range(0, audioClips.Length)];
        return null;
    }

    public AnimationClip GetAbilityAnimation()
    {
        return abilityAnimation;
    }
}
