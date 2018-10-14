using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    [SerializeField] WeaponConfig meleeWeaponConfig;
    [SerializeField] WeaponConfig rangedWeaponConfig;
    [SerializeField] float switchWeaponCoolDownTime = 3f;
    [SerializeField] GameObject switchWeaponText;
    [SerializeField] Image currentWeaponImage;
    [SerializeField] Image nextWeaponImage;

    WeaponSystem weaponSystem;
    bool canSwitchWeapon = true;
    float lastSwitchWeaponTime;
    float hideSwitchWeaponTextTime;
    Sprite tempWeaponImage;

    public WeaponConfig GetEquippedMeleeWeapon()
    {
        return meleeWeaponConfig;
    }

    public WeaponConfig GetEquippedRangedWeapon()
    {
        return rangedWeaponConfig;
    }

    void Start()
    {
        weaponSystem = GetComponent<WeaponSystem>();
    }

    void Update()
    {
        if (Time.time >= hideSwitchWeaponTextTime)
        {
            switchWeaponText.SetActive(false);
        }

        if(!canSwitchWeapon && 
           Time.time - lastSwitchWeaponTime >= switchWeaponCoolDownTime)
        {
            canSwitchWeapon = true;
            currentWeaponImage.GetComponent<Button>().interactable = true;
            nextWeaponImage.GetComponent<Button>().interactable = true;
        }
    }

    public void SwitchWeapon()
    {
        if (canSwitchWeapon)
        {
            switchWeaponText.SetActive(false);
            weaponSystem.CancleAction();
            weaponSystem.DestroyWeaponObject();

            var currentWeapon = weaponSystem.GetCurrentWeapon();
            if (currentWeapon.IsMeleeWeapon())
            {
                currentWeapon = rangedWeaponConfig;
            }
            else
            {
                currentWeapon = meleeWeaponConfig;
            }

            SwitchIconImages();
            weaponSystem.PutWeaponInHand(currentWeapon);
            canSwitchWeapon = false;
            lastSwitchWeaponTime = Time.time;
        }
        else
        {
            ShowSwitchWeaponText();
        }
    }

    private void SwitchIconImages()
    {
        tempWeaponImage = currentWeaponImage.sprite;
        currentWeaponImage.sprite = nextWeaponImage.sprite;
        nextWeaponImage.sprite = tempWeaponImage;

        currentWeaponImage.GetComponent<Button>().interactable = false;
        nextWeaponImage.GetComponent<Button>().interactable = false;
    }

    private void ShowSwitchWeaponText()
    {
        switchWeaponText.SetActive(true);
        hideSwitchWeaponTextTime = Time.time + switchWeaponCoolDownTime;
    }
}
