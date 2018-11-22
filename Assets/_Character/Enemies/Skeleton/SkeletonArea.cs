using System.Collections;
using UnityEngine;

public class SkeletonArea : MonoBehaviour
{

    // Use this for initialization
    //public float spawnRadius = 8f;
    public GameObject skeletonPrefab;
    public GameObject[] listOfSpawn;
    public int numberSpawn = 1;
   

    public void AutoFindDestrouy()
    {
            if (transform.childCount == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (gameObject.transform.GetChild(i).transform.childCount == 0)
                    {
                        Destroy(gameObject.transform.GetChild(i).gameObject);
                    }
                }
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