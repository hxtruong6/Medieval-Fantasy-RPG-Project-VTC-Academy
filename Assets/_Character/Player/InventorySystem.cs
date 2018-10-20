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
    float? lastSwitchWeaponTime;
    float hideSwitchWeaponTextTime;

    public WeaponConfig GetEquippedMeleeWeapon() { return meleeWeaponConfig; }

    public WeaponConfig GetEquippedRangedWeapon() { return rangedWeaponConfig; }

    void Start()
    {
        weaponSystem = GetComponent<WeaponSystem>();
        UpdateWeaponIcons();
    }

    void Update()
    {
        if (Time.time >= hideSwitchWeaponTextTime)
        {
            switchWeaponText.SetActive(false);
        }
    }

    private void UpdateWeaponIcons()
    {
        var currentUseWeapon = weaponSystem.GetCurrentWeapon();
        currentWeaponImage.sprite = currentUseWeapon.GetWeaponIcon();

        if (currentUseWeapon.IsMeleeWeapon())
            nextWeaponImage.sprite = rangedWeaponConfig.GetWeaponIcon();
        else
            nextWeaponImage.sprite = meleeWeaponConfig.GetWeaponIcon();
    }

    private void ShowSwitchWeaponText()
    {
        switchWeaponText.SetActive(true);
        hideSwitchWeaponTextTime = Time.time + switchWeaponCoolDownTime;
    }

    private WeaponConfig ChangeWeaponType(WeaponConfig currentWeapon)
    {
        var abilites = GetComponent<SpecialAbilities>();

        if (currentWeapon.IsMeleeWeapon())
        {
            currentWeapon = rangedWeaponConfig;
            abilites.LockMeleeAbilites(true);
        }
        else
        {
            currentWeapon = meleeWeaponConfig;
            abilites.LockMeleeAbilites(false);
        }
        return currentWeapon;
    }

    IEnumerator SwitchWeaponCoolDown()
    {
        currentWeaponImage.GetComponent<Button>().interactable = false;
        nextWeaponImage.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(switchWeaponCoolDownTime);
        currentWeaponImage.GetComponent<Button>().interactable = true;
        nextWeaponImage.GetComponent<Button>().interactable = true;
    }

    public void SwitchWeapon()
    {
        if (lastSwitchWeaponTime == null ||
            Time.time - lastSwitchWeaponTime >= switchWeaponCoolDownTime)
        {
            switchWeaponText.SetActive(false);
            weaponSystem.CancleAction();
            weaponSystem.DestroyWeaponObject();

            var currentWeapon = weaponSystem.GetCurrentWeapon();
            currentWeapon = ChangeWeaponType(currentWeapon);           
            weaponSystem.PutWeaponInHand(currentWeapon);

            UpdateWeaponIcons();
            StartCoroutine(SwitchWeaponCoolDown());
            lastSwitchWeaponTime = Time.time;
        }
        else
        {
            ShowSwitchWeaponText();
        }
    } 
    
    public void PickUpNewWeapon(DropItem dropItem)
    {
        var newWeapon = dropItem.GetDropItemWeaponConfig();
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
        UpdateWeaponIcons();
        Destroy(dropItem.gameObject);
    }
}
