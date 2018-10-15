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

    private WeaponConfig ChangeWeaponType(WeaponConfig currentWeapon)
    {
        if (currentWeapon.IsMeleeWeapon())
        {
            currentWeapon = rangedWeaponConfig;
        }
        else
        {
            currentWeapon = meleeWeaponConfig;
        }
        return currentWeapon;
    }

    public void SwitchWeapon()
    {
        if (canSwitchWeapon)
        {
            switchWeaponText.SetActive(false);
            weaponSystem.CancleAction();
            weaponSystem.DestroyWeaponObject();

            var currentWeapon = weaponSystem.GetCurrentWeapon();
            currentWeapon = ChangeWeaponType(currentWeapon);           
            weaponSystem.PutWeaponInHand(currentWeapon);

            SwitchIconImages();
            canSwitchWeapon = false;
            lastSwitchWeaponTime = Time.time;
        }
        else
        {
            ShowSwitchWeaponText();
        }
    } 
    
    public void PickUpNewWeapon(DropItem dropItem)
    {
        var newWeapon = dropItem.GetComponent<DropItem>().GetDropItemWeaponConfig();
        bool matchCurrentWeaponType = weaponSystem.GetCurrentWeapon().IsMeleeWeapon() == newWeapon.IsMeleeWeapon();

        if (newWeapon.IsMeleeWeapon())
        {
            meleeWeaponConfig = newWeapon;
        }
        else
        {
            rangedWeaponConfig = newWeapon;
        }

        if(matchCurrentWeaponType)
        {
            weaponSystem.PutWeaponInHand(newWeapon);
        }
    }
}
