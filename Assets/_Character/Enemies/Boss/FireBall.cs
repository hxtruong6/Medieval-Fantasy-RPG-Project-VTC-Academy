using UnityEngine;

public class FireBall : MonoBehaviour
{

    [SerializeField] private float fireDamage = 3f;
    GameObject player;
    // Use this for initialization
    void Start()
    {
        if ((int)Random.Range(1, 11) > 4)
        {
            GetComponent<SphereCollider>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (player == null)
            {
                DealDamage(other.gameObject);
                player = other.gameObject;
            }
        }
        

    }


    private void DealDamage(GameObject objectBeingHit)
    {
        if (!objectBeingHit.GetComponent<HealthSystem>() ||
            objectBeingHit.GetComponent<HealthSystem>().HealthAsPercentage < 0)
        {
            return;
        }
        Debug.Log("Fire Hit");
        objectBeingHit.GetComponent<HealthSystem>().TakeDamage(fireDamage);
        Destroy(gameObject);
    }
}
