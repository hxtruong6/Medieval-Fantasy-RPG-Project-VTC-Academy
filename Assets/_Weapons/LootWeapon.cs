using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootWeapon : MonoBehaviour
{
    [SerializeField] WeaponConfig weaponConfig;
    [HideInInspector] public bool isPicked = false;

    public void SetWeaponConfig(WeaponConfig configToSet)
    {
        weaponConfig = configToSet;
    }

    public WeaponConfig GetDropItemWeaponConfig()
    {
        if (weaponConfig != null)
            return weaponConfig;
        return null;
    }

    public void DestroyDropEffect()
    {
        Destroy(GetComponentInChildren<ParticleSystem>());
    }
}
