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

    float energyAsPercent
    {
        get
        {
            return currentEnergyPoints / maxEnergyPoints;
        }
    }

    public float GetCurrentEnergyPoints() { return currentEnergyPoints; }

    void Start ()
    {
        currentEnergyPoints = maxEnergyPoints;
        UpdateEnergyBar();
    }
	
	void Update ()
    {
        if (currentEnergyPoints < maxEnergyPoints)
        {
            AddEnergyPoints();
            UpdateEnergyBar();
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
}
