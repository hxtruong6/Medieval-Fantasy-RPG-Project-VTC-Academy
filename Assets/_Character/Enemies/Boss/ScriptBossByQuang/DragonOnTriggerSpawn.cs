using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonOnTriggerSpawn : MonoBehaviour {
    public GameObject dragon;
    public GameObject particle;
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerControl>())
        {
            //var playerCurrentMeleeWeapon = FindObjectOfType<PlayerControl>().GetComponent<InventorySystem>().GetEquippedMeleeWeapon();
            //playerCurrentMeleeWeapon.SetAttackRange(playerCurrentMeleeWeapon.GetMaxAttackRange() * 2);
            //WeaponConfig demonCurrentMeleeWeapon = FindObjectOfType<PlayerControl>().GetComponent<DemonTrigger>().replaceMeleeWeapon;
            //demonCurrentMeleeWeapon.SetAttackRange(demonCurrentMeleeWeapon.GetMaxAttackRange() * 2);
            dragon.gameObject.SetActive(true);
            Destroy(this.gameObject.GetComponent<BoxCollider>());
            Destroy(particle);
        }
    }
}
