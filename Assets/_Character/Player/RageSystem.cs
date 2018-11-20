using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RageSystem : MonoBehaviour {
    [SerializeField] Image rageBar;
    [SerializeField] float maxRagePoints = 100f;
    [SerializeField] float decayPointsPerSecond = 20;

    public float attackedGain = 5;
    public float normalAttackGain = 5;
    public float powerAttackGain = 10;
    public float aoeAttackGain = 20;
    [HideInInspector] public bool isActived;
    [HideInInspector] public bool isUnlimited;
    [HideInInspector] public float currentRagePoints;

    public float RagePointAsPercent { get { return currentRagePoints / maxRagePoints; } }
    public float GetcurrentRagePoints() { return currentRagePoints; }
    public float GetMaxRagePoints() { return maxRagePoints; }
    DemonTrigger demonTrigger;

    void Start()
    {
        demonTrigger = GetComponent<DemonTrigger>();

        currentRagePoints = 0;
        UpdateRageBar();
    }

    void Update()
    {
        if (isActived && !isUnlimited)
        {
            DecayRagePoints();
            UpdateRageBar();
        }
    }

    private void DecayRagePoints()
    {
        var pointsToRemove = decayPointsPerSecond * Time.deltaTime;
        currentRagePoints = Mathf.Clamp(currentRagePoints - pointsToRemove, 0, maxRagePoints);

        if (currentRagePoints == 0)
        {
            if(GetComponent<PlayerControl>().isInDemonForm)
            {
                if (GetComponent<HealthSystem>().HealthAsPercentage > 0)
                {
                    GetComponent<PlayerControl>().isInDemonForm = false;
                    isActived = false;
                    demonTrigger.TurnBackToHumanForm();                   
                }             
            }
        }
    }

    public void GainRagePoints(float pointsToAdd)
    {
        if (isUnlimited)
            return;

        float newRagePoints = currentRagePoints + pointsToAdd;
        currentRagePoints = Mathf.Clamp(newRagePoints, 0, maxRagePoints);
        UpdateRageBar();
    }

    private void UpdateRageBar()
    {
        rageBar.fillAmount = RagePointAsPercent;
    }
}
