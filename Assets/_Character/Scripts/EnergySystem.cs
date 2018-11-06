using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergySystem : MonoBehaviour
{
    [SerializeField] Image energyBar;
    [SerializeField] float maxEnergyPoints = 100f;
    [SerializeField] float regenPointsPerSecond = 1f;   

    float currentEnergyPoints;
    bool unlimitedPower;

    float EnergyAsPercent{get{return currentEnergyPoints / maxEnergyPoints;}}

    public float GetCurrentEnergyPoints() { return currentEnergyPoints; }
    public bool GetPowerLimit() { return unlimitedPower; }
    public void SetLimitPower(bool value) { unlimitedPower = value; }

    void Start ()
    {
        currentEnergyPoints = maxEnergyPoints;
        UpdateEnergyBar();
    }
	
	void Update ()
    {
        if (currentEnergyPoints < maxEnergyPoints)
        {
            RegenEnergyPoints();
            UpdateEnergyBar();
        }
    }

    private void RegenEnergyPoints()
    {
        var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
        currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
    }

    public void ConsumeEnergy(float amount)
    {
        if (unlimitedPower)
            return;

        float newEnergyPoints = currentEnergyPoints - amount;
        currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints);
        UpdateEnergyBar();
    }

    public void RestoreAmount(float amount)
    {
        currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + amount, 0f, maxEnergyPoints);
        UpdateEnergyBar();
    }

    public void RestorePercentage(int percentage)
    {
        float restoreAmount = maxEnergyPoints / 100 * percentage;
        RestoreAmount(restoreAmount);
    }

    private void UpdateEnergyBar()
    {
        energyBar.fillAmount = EnergyAsPercent;
    }
}
