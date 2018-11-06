using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropLoot : MonoBehaviour
{
    const int DROP_ITEM_LAYER = 10;

    [Range(.1f, 1.0f)] [SerializeField] float dropWeaponChance = 0.1f;
    [SerializeField] WeaponConfig[] weaponList;
    [SerializeField] HealthPotionConfig[] healthPotionList;
    [SerializeField] ManaPotionConfig[] manaPotionList;
    [SerializeField] int minHealthDrop;
    [SerializeField] int maxHealthDrop;
    [SerializeField] int minManaDrop;
    [SerializeField] int maxManaDrop;

    List<WeaponConfig> _weaponList;

    void Start()
    {
        _weaponList = new List<WeaponConfig>();
        for (int i = 0; i < weaponList.Length; i++)
        {
            _weaponList.Add(weaponList[i]);
        }
    }

    public void DropWeaponAndItem()
    {
        DropWeapon();
        DropHealthPotion();
        DropManaPotion();
    }

    private void CreateWeaponLoot(WeaponConfig weaponConfig)
    {
        var weaponPrefab = weaponConfig.GetWeaponPrefab();
        var dropPos = gameObject.transform.position - Random.insideUnitSphere;
        dropPos.y = weaponConfig.GetWeaponPrefab().transform.position.y;

        var dropItemObject = Instantiate(weaponPrefab,
                    dropPos,
                    weaponPrefab.transform.rotation,
                    GameManager.instance.tempObjects);

        //dropItemObject.layer = DROP_ITEM_LAYER;
        dropItemObject.AddComponent<LootItem>();
        dropItemObject.GetComponent<LootItem>().SetDropWeaponConfig(weaponConfig);

        var dropEffectPrefab = weaponConfig.GetDropParticlePrefab();

        if (dropEffectPrefab == null)
            return;

        Instantiate(dropEffectPrefab,
            dropItemObject.transform.position,
            dropEffectPrefab.transform.rotation,
            dropItemObject.transform);
    }

    private void DropWeapon()
    {
        if (weaponList.Length <= 0) { return; }

        bool isDropWeapon = Random.Range(0f, 1f) <= dropWeaponChance;

        if(isDropWeapon)
        {
            int weaponIndex = Random.Range(0, _weaponList.Count);
            CreateWeaponLoot(_weaponList[weaponIndex]);
            _weaponList.Remove(_weaponList[weaponIndex]);
        }
    }

    private void CreateHPotionLoot(HealthPotionConfig potionConfig)
    {
        var itemPrefab = potionConfig.GetPotionPrefab();
        var dropPos = gameObject.transform.position - Random.insideUnitSphere;
        dropPos.y = potionConfig.GetPotionPrefab().transform.position.y;

        var dropItemObject = Instantiate(itemPrefab,
                    dropPos,
                    itemPrefab.transform.rotation,
                    GameManager.instance.tempObjects);

        dropItemObject.layer = DROP_ITEM_LAYER;
        dropItemObject.AddComponent<LootItem>();
        dropItemObject.GetComponent<LootItem>().SetDropItemConfig(potionConfig);

        var dropEffectPrefab = potionConfig.GetDropParticlePrefab();

        if (dropEffectPrefab != null)
        {
            Instantiate(dropEffectPrefab,
                dropItemObject.transform.position,
                dropEffectPrefab.transform.rotation,
                dropItemObject.transform);
        }
    }

    private void DropHealthPotion()
    {
        if (healthPotionList.Length <= 0) { return; }

        int numberOfHealthDrop = Random.Range(minHealthDrop, maxHealthDrop);

        for (int i = 0; i < numberOfHealthDrop; i++)
        {
            int itemIndex = Random.Range(0, healthPotionList.Length);
            CreateHPotionLoot(healthPotionList[itemIndex]);
        }
    }

    private void CreateMPotionLoot(ManaPotionConfig potionConfig)
    {
        var itemPrefab = potionConfig.GetPotionPrefab();
        var dropPos = gameObject.transform.position - Random.insideUnitSphere;
        dropPos.y = potionConfig.GetPotionPrefab().transform.position.y;

        var dropItemObject = Instantiate(itemPrefab,
                    dropPos,
                    itemPrefab.transform.rotation,
                    GameManager.instance.tempObjects);

        dropItemObject.layer = DROP_ITEM_LAYER;
        dropItemObject.AddComponent<LootItem>();
        dropItemObject.GetComponent<LootItem>().SetDropItemConfig(potionConfig);

        var dropEffectPrefab = potionConfig.GetDropParticlePrefab();

        if (dropEffectPrefab != null)
        {
            Instantiate(dropEffectPrefab,
                dropItemObject.transform.position,
                dropEffectPrefab.transform.rotation,
                dropItemObject.transform);
        }
    }

    private void DropManaPotion()
    {
        if (manaPotionList.Length <= 0) { return; }

        int numberOfManaDrop = Random.Range(minManaDrop, maxManaDrop);

        for (int i = 0; i < numberOfManaDrop; i++)
        {
            int itemIndex = Random.Range(0, manaPotionList.Length);
            CreateMPotionLoot(manaPotionList[itemIndex]);
        }
    }
}
