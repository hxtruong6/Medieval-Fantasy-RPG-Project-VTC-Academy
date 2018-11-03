using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootItem : MonoBehaviour
{
    [SerializeField] ScriptableObject potionConfig;
    [SerializeField] WeaponConfig weaponConfig;
    [HideInInspector] public bool isPicked = false;

    public void SetDropItemConfig(ScriptableObject configToSet)
    {
        potionConfig = configToSet;
    }

    public ScriptableObject GetDropPotionConfig()
    {
        return potionConfig;
    }

    public void SetDropWeaponConfig(WeaponConfig configToSet)
    {
        weaponConfig = configToSet;
    }

    public WeaponConfig GetDropWeaponConfig()
    {
        return weaponConfig;
    }

    public void DestroyDropEffect()
    {
        Destroy(GetComponentInChildren<ParticleSystem>());
    }
}
