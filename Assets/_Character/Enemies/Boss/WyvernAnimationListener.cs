using System.Collections;
using UnityEngine;

public class WyvernAnimationListener : MonoBehaviour
{
    private WyvernAttacking wyvernAttacking;
    public AudioClip deathSound;
    public GameObject disableBlock;
    public Material dieMaterial;
    // Use this for initialization
    void Start()
    {
        wyvernAttacking = GetComponentInParent<WyvernAttacking>();
    }

    void switchState()
    {
        //Debug.Log("Switch");
        wyvernAttacking.switchState();
    }

    void deleteAllCollision()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        //Debug.Log("Collider "+ colliders.Length);
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    void dieEffect()
    {
        disableBlock.gameObject.SetActive(false);
        GetComponentInChildren<SkinnedMeshRenderer>().material = dieMaterial;
        var spawnEffect = GetComponentInChildren<SpawnEffect>();
        spawnEffect.enabled = true;
        StartCoroutine(DieDestroy(spawnEffect.spawnEffectTime));

        //var playerCurrentMeleeWeapon = FindObjectOfType<PlayerControl>().GetComponent<InventorySystem>().GetEquippedMeleeWeapon();
        //playerCurrentMeleeWeapon.SetAttackRange(playerCurrentMeleeWeapon.GetMaxAttackRange() /2);
        //WeaponConfig demonCurrentMeleeWeapon = FindObjectOfType<PlayerControl>().GetComponent<DemonTrigger>().replaceMeleeWeapon;
        //demonCurrentMeleeWeapon.SetAttackRange(demonCurrentMeleeWeapon.GetMaxAttackRange() / 2);
    }

    private IEnumerator DieDestroy(float timeEffect)
    {
        GetComponentInParent<AudioSource>().Stop();
        GetComponentInParent<AudioSource>().PlayOneShot(deathSound);
        yield return new WaitForSeconds(timeEffect);
        GameObject gb = GetComponentInParent<WyvernAttacking>().gameObject;
        if (gb)
        {
            Destroy(gb);
        }

    }
}
