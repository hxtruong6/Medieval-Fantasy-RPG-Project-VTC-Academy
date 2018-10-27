using UnityEngine;
using UnityEngine.Assertions;

public class SkeletonSpawn : MonoBehaviour
{
    [SerializeField] WeaponConfig[] listOfWeapon;
    [SerializeField] private int selectedWeapon = 0;

    private Enemy enemy;
    // Use this for initialization;
    private WeaponSystem weaponSystem;
    public float spawnRadius = 10f;

    [ExecuteInEditMode]
    void OnValidate()
    {
        selectedWeapon = Mathf.Clamp(selectedWeapon, 0, listOfWeapon.Length - 1);
    }

    void Start()
    {
        enemy = FindObjectOfType<Enemy>();
        enemy.fleeing = false;

        Assert.IsFalse(listOfWeapon.Length == 0, "Them Weapon cho Skeleton");
        Assert.IsFalse(selectedWeapon < 0 || selectedWeapon >= listOfWeapon.Length, "Select index weapon is invalid.");

        weaponSystem = FindObjectOfType<WeaponSystem>();
        weaponSystem.SetCurrentWeapon(listOfWeapon[selectedWeapon]);


    }

    // Update is called once per frame
    void Update()
    {

    }
}
