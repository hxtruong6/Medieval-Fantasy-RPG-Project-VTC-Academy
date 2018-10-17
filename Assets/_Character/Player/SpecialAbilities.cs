using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAbilities : MonoBehaviour
{
    [SerializeField] AbilityConfig[] abilities;
    [SerializeField] Image energyBar;
    [SerializeField] float maxEnergyPoints = 100f;
    [SerializeField] float regenPointsPerSecond = 1f;
    [SerializeField] AudioClip outOfEnergy;

    float currentEnergyPoints;
    AudioSource audioSource;

    float energyAsPercent
    {
        get
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        AttachInitialAbilities();
        currentEnergyPoints = maxEnergyPoints;
        UpdateEnergyBar();
    }

    void Update()
    {
        if (currentEnergyPoints < maxEnergyPoints)
        {
            AddEnergyPoints();
            UpdateEnergyBar();
        }
    }

    public int GetNumberOfAbilities()
    {
        return abilities.Length;
    }

    void AttachInitialAbilities()
    {
        for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
        {
            abilities[abilityIndex].AttachAbilityTo(gameObject);
        }
    }

    void AddEnergyPoints()
    {
        var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
        currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
    }

    public void ConsumeEnergy(float amount)
    {
        float newEnergyPoints = currentEnergyPoints - amount;
        currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
        UpdateEnergyBar();
    }

    private void UpdateEnergyBar()
    {
        energyBar.fillAmount = energyAsPercent;
    }

    public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
    {
        var energyComponent = GetComponent<SpecialAbilities>();
        var energyCost = abilities[abilityIndex].GetEnergyCost();

        if (energyCost <= currentEnergyPoints)
        {
            ConsumeEnergy(energyCost);
            abilities[abilityIndex].Use(target);
        }
        else
        {
            if (audioSource.isPlaying && outOfEnergy != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(outOfEnergy);
            }
        }
    }
}
