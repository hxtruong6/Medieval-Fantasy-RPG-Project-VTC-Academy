using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    const string TEMP_OBJECTS_TAG = "TempObjects";
    const int DROP_ITEM_LAYER = 10;

    [Range(.1f, 1.0f)] [SerializeField] float dropChance = 0.1f;
    [SerializeField] WeaponConfig[] weaponList;
    [SerializeField] GameObject[] itemList;
    [SerializeField] int minWeaponDrop;
    [SerializeField] int maxWeaponDrop;
    [SerializeField] int maxItemDrop;

    List<WeaponConfig> _weaponList;

    void Start()
    {
        _weaponList = new List<WeaponConfig>();
        for (int i = 0; i < weaponList.Length; i++)
        {
            _weaponList.Add(weaponList[i]);
        }
    }

    public void DropLoot()
    {
        bool isDrop = Random.Range(0f, 1f) <= dropChance;

        if (!isDrop)
            return;

        int numberOfWeaponDrop = Random.Range(minWeaponDrop, maxWeaponDrop);

        for (int i = 0; i < numberOfWeaponDrop; i++)
        {
            int weaponIndex = Random.Range(0, _weaponList.Count);           
            CreateWeaponLoot(_weaponList[weaponIndex], i);
            _weaponList.Remove(_weaponList[weaponIndex]);
        }       
    }

    private void CreateWeaponLoot(WeaponConfig weaponConfig, int weaponCount)
    {
        var weaponPrefab = weaponConfig.GetWeaponPrefab();
        var dropPos = gameObject.transform.position;
        dropPos.x += Random.Range(-1,1) * weaponCount;
        dropPos.z += Random.Range(-1, 1) * weaponCount;

        var dropItemObject = Instantiate(weaponPrefab,
                    dropPos,
                    weaponPrefab.transform.rotation,
                    GameObject.FindGameObjectWithTag(TEMP_OBJECTS_TAG).transform);

        dropItemObject.layer = DROP_ITEM_LAYER;
        dropItemObject.AddComponent<LootWeapon>();
        dropItemObject.GetComponent<LootWeapon>().SetWeaponConfig(weaponConfig);

        var dropEffectPrefab = weaponConfig.GetDropParticlePrefab();

        var dropEffect = Instantiate(dropEffectPrefab,
            dropItemObject.transform.position,
            dropEffectPrefab.transform.rotation,
            dropItemObject.transform);
    }
}
