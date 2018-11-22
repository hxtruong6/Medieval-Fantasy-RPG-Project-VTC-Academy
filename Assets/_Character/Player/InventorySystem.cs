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
    [SerializeField] Image currentWeaponImage;
    [SerializeField] Image nextWeaponImage;
    [SerializeField] int amountOfSmallHealthPotion;
    [SerializeField] int amountOfLargeHealthPotion;
    [SerializeField] int amountOfSmallManaPotion;
    [SerializeField] int amountOfLargeManaPotion;
    [SerializeField] Image smallHealthImage;
    public Text textSmallHPotion;
    [SerializeField] Image largeHealthImage;
    public Text textLargeHPotion;
    [SerializeField] Image smallManaImage;
    public Text textSmallMPotion;
    [SerializeField] Image largeManaImage;
    public Text textLargeMPotion;


    WeaponSystem weaponSystem;
    float? lastSwitchWeaponTime;
    float hideSwitchWeaponTextTime;
    GameManager gameManager;

    public WeaponConfig GetEquippedMeleeWeapon() { return meleeWeaponConfig; }

    public WeaponConfig GetEquippedRangedWeapon() { return rangedWeaponConfig; }

    public int AmountOfSmallHPotion
    {
        get { return amountOfSmallHealthPotion; }

        set
        {
            amountOfSmallHealthPotion = value;
            textSmallHPotion.text = amountOfSmallHealthPotion.ToString();
        }
    }

    public int AmountOfLargeHPotion
    {
        get { return amountOfLargeHealthPotion; }

        set
        {
            amountOfLargeHealthPotion = value;
            textLargeHPotion.text = amountOfLargeHealthPotion.ToString();
        }
    }

    public int AmountOfSmallMPotion
    {
        get { return amountOfSmallManaPotion; }

        set
        {
            amountOfSmallManaPotion = value;
            textSmallMPotion.text = amountOfSmallManaPotion.ToString();
        }
    }

    public int AmountOfLargeMPotion
    {
        get { return amountOfLargeManaPotion; }

        set
        {
            amountOfLargeManaPotion = value;
            textLargeMPotion.text = amountOfLargeManaPotion.ToString();
        }
    }

    void Start()
    {
        weaponSystem = GetComponent<WeaponSystem>();
        gameManager = FindObjectOfType<GameManager>();

        UpdateWeaponIcons();
        textSmallHPotion.text = AmountOfSmallHPotion.ToString();
        textLargeHPotion.text = amountOfLargeHealthPotion.ToString();
        textSmallMPotion.text = amountOfSmallManaPotion.ToString();
        textLargeMPotion.text = amountOfLargeManaPotion.ToString();
    }

    void Update()
    {

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

        if (GetComponent<PlayerControl>().isInDemonForm)
            abilites.LockAllAbilities();

        return currentWeapon;
    }

    IEnumerator DisableIconForAPeriod(Image icon, float disableTime)
    {
        icon.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(disableTime);
        icon.GetComponent<Button>().interactable = true;
    }

    public void SwitchWeapon()
    {
        if (lastSwitchWeaponTime == null ||
            Time.time - lastSwitchWeaponTime >= switchWeaponCoolDownTime)
        {
            GameManager.instance.DisplayMessage("");
            weaponSystem.CancleAction();
            weaponSystem.DestroyWeaponObject();

            var currentWeapon = weaponSystem.GetCurrentWeapon();
            currentWeapon = ChangeWeaponType(currentWeapon);
            weaponSystem.PutWeaponInHand(currentWeapon);

            UpdateWeaponIcons();
            StartCoroutine(DisableIconForAPeriod(currentWeaponImage, switchWeaponCoolDownTime));
            StartCoroutine(DisableIconForAPeriod(nextWeaponImage, switchWeaponCoolDownTime));
            lastSwitchWeaponTime = Time.time;
        }
        else
        {
            GameManager.instance.DisplayMessage("You can't switch weapon again yet");
        }
    }

    public void PickUpNewItem(LootItem newItem)
    {
        if (newItem.isPicked)
            return;

        transform.LookAt(newItem.gameObject.transform);

        if (newItem.GetDropWeaponConfig())
        {
            PickUpNewWeapon(newItem.GetDropWeaponConfig());
        }
        else
        {
            if (newItem.GetDropPotionConfig() == gameManager.smallHealthPotion)
            {
                AmountOfSmallHPotion += 1;
            }
            if (newItem.GetDropPotionConfig() == gameManager.largeHealthPotion)
            {
                AmountOfLargeHPotion += 1;
            }
            if (newItem.GetDropPotionConfig() == gameManager.smallManaPotion)
            {
                AmountOfSmallMPotion += 1;
            }
            if (newItem.GetDropPotionConfig() == gameManager.largeManaPotion)
            {
                AmountOfLargeMPotion += 1;
            }
        }
        newItem.GetComponent<InfoItem>().RemoveInfo();
        newItem.isPicked = true;
        Destroy(newItem.gameObject);
    }

    public void PickUpNewWeapon(WeaponConfig newWeapon)
    {
        bool matchCurrentWeaponType = weaponSystem.GetCurrentWeapon().IsMeleeWeapon() == newWeapon.IsMeleeWeapon();

        if (newWeapon.IsMeleeWeapon())
        {
            meleeWeaponConfig = newWeapon;
        }
        else
        {
            rangedWeaponConfig = newWeapon;
        }

        if (matchCurrentWeaponType)
        {
            weaponSystem.PutWeaponInHand(newWeapon);
        }
        UpdateWeaponIcons();
    }

    public void UseSmallHealthPotion()
    {
        if (amountOfSmallHealthPotion <= 0 || !smallHealthImage.GetComponent<Button>().interactable)
        {
            GameManager.instance.DisplayMessage("You cannot use this potion yet");
            return;
        }

        var amount = gameManager.smallHealthPotion.GetRestoreAmount();
        GetComponent<HealthSystem>().RestoreAmount(amount);
        AmountOfSmallHPotion -= 1;
        StartCoroutine(DisableIconForAPeriod(smallHealthImage, gameManager.smallHealthPotion.GetUseCoolDown()));
    }

    public void UseLargeHealthPotion()
    {
        if (amountOfLargeHealthPotion <= 0 || !largeHealthImage.GetComponent<Button>().interactable)
        {
            GameManager.instance.DisplayMessage("You cannot use this potion yet");
            return;
        }

        var percentage = gameManager.largeHealthPotion.GetRestorePercentage();
        GetComponent<HealthSystem>().RestorePercentage(percentage);
        AmountOfLargeHPotion -= 1;
        StartCoroutine(DisableIconForAPeriod(largeHealthImage, gameManager.largeHealthPotion.GetUseCoolDown()));
    }

    public void UseSmallManaPotion()
    {
        if (amountOfSmallManaPotion <= 0 || !smallManaImage.GetComponent<Button>().interactable)
        {
            GameManager.instance.DisplayMessage("You cannot use this potion yet");
            return;
        }

        var amount = gameManager.smallManaPotion.GetRestoreAmount();
        GetComponent<EnergySystem>().RestoreAmount(amount);
        AmountOfSmallMPotion -= 1;
        StartCoroutine(DisableIconForAPeriod(smallManaImage, gameManager.smallManaPotion.GetUseCoolDown()));
    }

    public void UseLargeManaPotion()
    {
        if (amountOfLargeManaPotion <= 0 || !largeManaImage.GetComponent<Button>().interactable)
        {
            GameManager.instance.DisplayMessage("You cannot use this potion yet");
            return;
        }

        var percentage = gameManager.largeManaPotion.GetRestorePercentage();
        GetComponent<EnergySystem>().RestorePercentage(percentage);
        AmountOfLargeMPotion -= 1;
        StartCoroutine(DisableIconForAPeriod(largeManaImage, gameManager.largeManaPotion.GetUseCoolDown()));
    }
}
