using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialAbilities : MonoBehaviour
{
    [SerializeField] AudioClip outOfEnergy;
    [Header("Melee Power Attack")]
    [SerializeField] AbilityConfig meleePAConfig;
    [SerializeField] Image meleePAIcon;
    [Header("Ranged Power Attack")]
    [SerializeField] AbilityConfig rangedPAConfig;
    [SerializeField] Image rangedPAIcon;
    [Header("Melee AOE Attack")]
    [SerializeField] AbilityConfig meleeAOEConfig;
    [SerializeField] Image meleeAOEIcon;
    [Header("Ranged AOE Attack")]
    [SerializeField] AbilityConfig rangedAOEConfig;
    [SerializeField] Image rangedAOEIcon;

    AudioSource audioSource;
    EnergySystem energySystem;

    AbilityConfig[] abilities;
    Image[] abilityIcons;
    GameObject target;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        energySystem = GetComponent<EnergySystem>();

        AssignAbilityIndex();//Tam thoi set cung
        AssignAbilityIconIndex();

        AttachInitialAbilities();

        LockMeleeAbilites(false);
    }

    public int GetNumberOfAbilities() { return abilities.Length; }

    public void LockMeleeAbilites(bool value)
    {
        abilityIcons[0].GetComponent<Button>().interactable = !value;
        abilityIcons[2].GetComponent<Button>().interactable = !value;

        abilityIcons[1].GetComponent<Button>().interactable = value;
        abilityIcons[3].GetComponent<Button>().interactable = value;
    }

    private void AssignAbilityIndex()
    {
        abilities = new AbilityConfig[4];

        abilities[0] = meleePAConfig;
        abilities[1] = rangedPAConfig;
        abilities[2] = meleeAOEConfig;
        abilities[3] = rangedAOEConfig;
    }

    private void AssignAbilityIconIndex()
    {
        abilityIcons = new Image[4];

        abilityIcons[0] = meleePAIcon;
        abilityIcons[1] = rangedPAIcon;
        abilityIcons[2] = meleeAOEIcon;
        abilityIcons[3] = rangedAOEIcon;
    }

    private void AttachInitialAbilities()
    {
        for (int abilityIndex = 0; abilityIndex < GetNumberOfAbilities(); abilityIndex++)
        {
            abilities[abilityIndex].AttachAbilityTo(gameObject);
        }
    }

    IEnumerator DisableIconForAPeriod(Image icon, float time)
    {
        icon.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(time);
        icon.GetComponent<Button>().interactable = true;
    }

    public void SetSkillTarget(GameObject targetToSet)
    {
        target = targetToSet;
    }

    public void AttemptSpecialAbility(int abilityIndex)
    {
        var energyCost = abilities[abilityIndex].GetEnergyCost();

        if (energyCost <= energySystem.GetCurrentEnergyPoints())
        {
            var characterDamage = GetComponent<Character>().GetBaseDamage();
            var weaponDamage = GetComponent<WeaponSystem>().GetWeaponDamage();

            var abilityParams = new AbilityUseParams(target, characterDamage + weaponDamage);

            if (abilities[abilityIndex].Use(abilityParams))
            {              
                var skillCoolDown = abilities[abilityIndex].GetCoolDownTime();
                var skillIcon = abilityIcons[abilityIndex];
                StartCoroutine(DisableIconForAPeriod(skillIcon, skillCoolDown));
            }
        }
        else
        {
            if (audioSource.isPlaying && outOfEnergy != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(outOfEnergy);
            }
        }
    }
}
