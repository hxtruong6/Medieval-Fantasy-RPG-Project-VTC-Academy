using System.Collections;
using UnityEngine;

public class SkeletonArea : MonoBehaviour
{

    // Use this for initialization
    //public float spawnRadius = 8f;
    public GameObject skeletonPrefab;
    public GameObject[] listOfSpawn;
    public int numberSpawn = 1;



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
                    skeletonClone.transform.SetParent(listOfSpawn[i].transform);

                    //var animatorDummny = skeletonClone.GetComponentInChildren<Animator>();
                    // StartCoroutine(spawnWeapon(skeletonClone, animatorDummny.runtimeAnimatorController.animationClips[0].length));
                }
            }
        }
    }

    IEnumerator spawnWeapon(GameObject skeletonClone, float clipLength)
    {
        yield return new WaitForSeconds(clipLength);




    }
}