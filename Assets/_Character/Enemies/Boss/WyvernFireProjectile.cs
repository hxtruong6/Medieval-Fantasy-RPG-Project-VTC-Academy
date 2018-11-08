using System.Collections;
using UnityEngine;

public class WyvernFireProjectile : MonoBehaviour
{

    [SerializeField] private float fireProjectileSpeed;
    [SerializeField] private Rigidbody fireProjectile;
    [SerializeField] private Transform fireProjectPosition;
    [SerializeField] private float timeForProjectileDestroy;
    [SerializeField] private Transform tempObject;

    // Use this for initialization
    void Start()
    {
        if (tempObject == null)
        {
            //tempObject = FindObjectOfType<GameObject>().CompareTag("TempObjects");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void fireShooting()
    {
        Rigidbody instantiatedProjectile = Instantiate(fireProjectile, fireProjectPosition.position, fireProjectPosition.rotation);
        instantiatedProjectile.velocity = fireProjectPosition.TransformDirection(new Vector3(0, 0, fireProjectileSpeed));
        instantiatedProjectile.gameObject.transform.parent = tempObject;
        StartCoroutine(AutoDetroyFire(instantiatedProjectile));
    }

    private IEnumerator AutoDetroyFire(Rigidbody projectile)
    {
        while (projectile && projectile.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(timeForProjectileDestroy);
            Destroy(projectile.gameObject);
        }
    }
}
