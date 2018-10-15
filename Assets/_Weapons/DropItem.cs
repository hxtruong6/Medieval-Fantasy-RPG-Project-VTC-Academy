using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    [SerializeField] WeaponConfig weaponConfig;


    public WeaponConfig GetDropItemWeaponConfig()
    {
        if (weaponConfig != null)
            return weaponConfig;
        return null;
    }
}
