using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = ("Medieval-Fantasy-RPG/Mana Potion"))]

public class ManaPotionConfig : ScriptableObject
{
    [SerializeField] int restoreAmount = 50;
    [SerializeField] int restorePercentage = 0;
    [SerializeField] GameObject potionPrefab;
    [SerializeField] GameObject dropParticlePrefab;
    [SerializeField] float useCoolDown = 1;

    public int GetRestoreAmount() { return restoreAmount; }

    public int GetRestorePercentage() { return restorePercentage; }

    public GameObject GetPotionPrefab() { return potionPrefab; }

    public GameObject GetDropParticlePrefab() { return dropParticlePrefab; }

    public float GetUseCoolDown() { return useCoolDown; }
}
