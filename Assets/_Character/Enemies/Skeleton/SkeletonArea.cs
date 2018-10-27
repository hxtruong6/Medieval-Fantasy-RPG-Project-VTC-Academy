using UnityEngine;

public class SkeletonArea : MonoBehaviour
{

    // Use this for initialization
    //public float spawnRadius = 8f;
    public GameObject skeleton;
    public GameObject[] listOfSpawn;
    public int numberSpawn = 1;

    void Start()
    {
        //listOfSpawn = gameObject.GetComponentsInParent<GameObject>();
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
                    var skeletonPrefab = Instantiate(skeleton);
                    skeletonPrefab.transform.position = listOfSpawn[i].transform.position;
                    skeletonPrefab.transform.SetParent(this.transform);
                }
            }
        }
    }
}
