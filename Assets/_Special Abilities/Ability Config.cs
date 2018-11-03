using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AbilityUseParams
{
    public GameObject target;
    public float baseDamage;

    public AbilityUseParams(GameObject target, float baseDamage)
    {
        this.target = target;
        this.baseDamage = baseDamage;
    }
}

public abstract class AbilityConfig : ScriptableObject
{
    [Header("Special Ability General")]
    [SerializeField] float energyCost = 10f;
    [SerializeField] AnimationClip abilityAnimation;
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] float coolDownTime;
    [SerializeField] GameObject effectOnSelf;
    [SerializeField] GameObject effectOnWeapon;
    [SerializeField] GameObject effectOnEnemy;

    protected AbilityBehaviour behaviour;
    float? lastUseTime = null;

    public abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);

    public void AttachAbilityTo(GameObject objectToAttachTo)
    {
        AbilityBehaviour behaviourComponent = GetBehaviourComponent(objectToAttachTo);
        behaviourComponent.SetConfig(this);
        behaviour = behaviourComponent;
    }

    public bool Use(AbilityUseParams useParams)
    {
        if (lastUseTime == null || Time.time >= lastUseTime + coolDownTime)
        {
            behaviour.Use(useParams);
            lastUseTime = Time.time;
            return true;
        }
        return false;
    }

    public float GetEnergyCost() { return energyCost; }

    public AudioClip GetRandomAbilitySound()
    {
        if (audioClips.Length > 0)
            return audioClips[Random.Range(0, audioClips.Length)];
        return null;
    }

    public AnimationClip GetAbilityAnimation() { return abilityAnimation; }

    public float GetCoolDownTime() { return coolDownTime; }

    public GameObject GetEffectOnSelf() { return effectOnSelf; }

    public GameObject GetEffectOnWeapon() { return effectOnWeapon; }

    public GameObject GetEffectOnEnemy() { return effectOnEnemy; }
}
