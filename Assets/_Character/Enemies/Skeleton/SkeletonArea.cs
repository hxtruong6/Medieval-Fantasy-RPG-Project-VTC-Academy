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
        //Debug.Log("Child: "+ gameObject.GetComponentInChildren<Transform>().gameObject.name );
        if (transform.childCount == 0)
        {
            Destroy(gameObject);
        }
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
                    GameObject skeletonClone = Instantiate(skeletonPrefab);
                    skeletonClone.transform.position = listOfSpawn[i].transform.position;
                    skeletonClone.transform.SetParent(listOfSpawn[i].transform);
                    Destroy(this.gameObject.GetComponent<SphereCollider>());
                }
            }
        }
    }

   
}