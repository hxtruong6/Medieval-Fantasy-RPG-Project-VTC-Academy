using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonWeapon : MonoBehaviour
{

    // Use this for initialization
    public int selectedWeapon = 0;
    public bool randomWeapon = true;

    public WeaponConfig[] listOfWeapon;

    public AnimatorOverrideController aniNormal, aniHanded;

    [ExecuteInEditMode]
    void OnValidate()
    {
        selectedWeapon = Mathf.Clamp(selectedWeapon, 0, listOfWeapon.Length - 1);
    }

    void Start()
    {
        WeaponSystem currentWeaponSystem = GetComponent<WeaponSystem>();

        var weaponIndex = randomWeapon ? Random.Range(0, listOfWeapon.Length - 1) : selectedWeapon;

        currentWeaponSystem.PutWeaponInHand(listOfWeapon[weaponIndex]);

        Character skeleton = GetComponent<Character>();
        skeleton.GetComponent<Enemy>().UpdateCurrentWeaponRange();

        if (listOfWeapon[weaponIndex].name == "SkeletonHandedSword" || weaponIndex == 2)
            skeleton.GetComponent<Character>().setAnimatorOverideController(aniHanded);
        else
            skeleton.GetComponent<Character>().setAnimatorOverideController(aniNormal);
    }

    // Update is called once per frame
    void Update()
    {

    }


}