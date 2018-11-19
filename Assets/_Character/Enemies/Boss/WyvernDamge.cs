using System;
using UnityEngine;
[RequireComponent(typeof(HealthSystem))]
/*
 * This class use to get damage from player 
 */
public class WyvernDamge : MonoBehaviour
{
    [SerializeField] private float wingDamage = 20f;
    [SerializeField] private float legDamge = 15f;
    [SerializeField] private float tailDamage = 10f;
    [SerializeField] private float headDamge = 30f;
    [SerializeField] private float foreDamage = 15f;
    [SerializeField] private float bodyDamage = 15f;

    private HealthSystem wyvernHealth;

    private Animator wyvernAnimator;
    private float lastHealthPercentage = 0f;

    // Use this for initialization
    void Start()
    {
        wyvernHealth = GetComponent<HealthSystem>();
        wyvernAnimator = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        if (Math.Abs(wyvernHealth.HealthAsPercentage - lastHealthPercentage) >= 50)
        {
            Debug.Log("HIIIITTT");
            wyvernAnimator.SetTrigger("GetHit");
        }

        lastHealthPercentage = wyvernHealth.HealthAsPercentage;
    }

    public void OnBodyPartHit(BodyPartType partType)
    {
        switch (partType)
        {
            case BodyPartType.Head:
                wyvernHealth.TakeDamage(headDamge);
                break;
            case BodyPartType.Wing:
                wyvernHealth.TakeDamage(wingDamage);
                break;
            case BodyPartType.Leg:
                wyvernHealth.TakeDamage(legDamge);
                break;
            case BodyPartType.Forearm:
                wyvernHealth.TakeDamage(foreDamage);
                break;
            case BodyPartType.Tail:
                wyvernHealth.TakeDamage(tailDamage);
                break;
            case BodyPartType.Body:
                wyvernHealth.TakeDamage(bodyDamage);
                break;
            default:
                throw new ArgumentOutOfRangeException("partType", partType, null);
        }
    }
}
