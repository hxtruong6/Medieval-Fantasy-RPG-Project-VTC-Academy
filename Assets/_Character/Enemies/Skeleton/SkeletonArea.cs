using System.Collections;
using UnityEngine;

public class SkeletonArea : MonoBehaviour
{

    // Use this for initialization
    //public float spawnRadius = 8f;
    public GameObject skeletonPrefab;
    public GameObject[] listOfSpawn;
    public int numberSpawn = 1;
    public int selectedWeapon = 0;

    public WeaponConfig[] listOfWeapon;

    [ExecuteInEditMode]
    void OnValidate()
    {
        selectedWeapon = Mathf.Clamp(selectedWeapon, 0, listOfWeapon.Length - 1);
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //gameObject.GetComponent<Collider>()
    }

    void OnTriggerEnter(Collider other)
    {
        if (numberSpawn-- > 0)
        {
            if (other.GetComponent<PlayerControl>())
            {
                for (int i = 0; i < listOfSpawn.Length; i++)
                {
                    GameObject skeletonClone = Instantiate(skeletonPrefab);
                    skeletonClone.transform.position = listOfSpawn[i].transform.position;
                    skeletonClone.transform.SetParent(this.transform);
                    var animatorDummny = skeletonClone.GetComponentInChildren<Animator>();
                    StartCoroutine(spawnWeapon(skeletonClone, animatorDummny.runtimeAnimatorController.animationClips[0].length));
                    //setSkeletonWeapon(skeletonPrefab);
                }
            }
        }
    }

    IEnumerator spawnWeapon(GameObject skeletonClone, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);
        var weapon = skeletonClone.GetComponentInChildren<WeaponSystem>();
        weapon.PutWeaponInHand(listOfWeapon[selectedWeapon]);
    }

    //private void setSkeletonWeapon(GameObject skeletonPrefab)
    //{
    //    var gameObjs = skeletonPrefab.GetComponentsInChildren<WeaponSystem>();
    //    for (int i = 0; i < gameObjs.Length; i++)
    //    {
    //        if (gameObjs[i] is WeaponSystem)
    //        {
    //            //WeaponSystem weapon = gameObjs[i] as WeaponSystem;
    //            //weapon.SetCurrentWeapon(listOfWeapon[selectedWeapon]);
    //        }
    //    }

    //}
}
