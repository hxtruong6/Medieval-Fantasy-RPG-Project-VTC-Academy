using UnityEngine;

public class FireBall : MonoBehaviour
{

    [SerializeField] private float fireDamage = 3f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.tag);
        //var layerCollidedWith = collision.gameObject.layer;
        //if (shooter && layerCollidedWith != shooter.layer)
        //{
        if (other.tag == "Player")
            DealDamage(other.gameObject);
        //}
    }


    private void DealDamage(GameObject objectBeingHit)
    {
        if (!objectBeingHit.GetComponent<HealthSystem>() ||
            objectBeingHit.GetComponent<HealthSystem>().HealthAsPercentage < 0)
        {
            return;
        }
        objectBeingHit.GetComponent<HealthSystem>().TakeDamage(fireDamage);
        Destroy(gameObject);
    }
}
