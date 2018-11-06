using System.Collections;
using UnityEngine;

public class SpawnEnemyArea : MonoBehaviour {
    public GameObject[] enemyPrefab;
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
        if (numberSpawn > 0)
        {
            if (other.GetComponent<PlayerControl>())
            {
                numberSpawn--;
                for (int i = 0; i < listOfSpawn.Length; i++)
                {
                    GameObject enemyClone = Instantiate(enemyPrefab[Random.Range(0, enemyPrefab.Length)]);
                    enemyClone.gameObject.layer = 9;
                    enemyClone.transform.position = listOfSpawn[i].transform.position;
                    enemyClone.transform.SetParent(listOfSpawn[i].transform);
                    listOfSpawn[i].gameObject.SetActive(true);
                    Destroy(this.gameObject.GetComponent<SphereCollider>());
                    //var animatorDummny = skeletonClone.GetComponentInChildren<Animator>();
                    // StartCoroutine(spawnWeapon(skeletonClone, animatorDummny.runtimeAnimatorController.animationClips[0].length));
                }
            }
        }
    }
}
