using System.Collections;
using UnityEngine;

public class WyvernFireProjectile : MonoBehaviour
{

    [SerializeField] private float fireProjectileSpeed;
    [SerializeField] private Rigidbody fireProjectile;
    [SerializeField] private Transform fireProjectPosition;
    [SerializeField] private float timeForProjectileDestroy;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void fireShooting()
    {
        Rigidbody instantiatedProjectile = Instantiate(fireProjectile, fireProjectPosition.position, fireProjectPosition.rotation);
        instantiatedProjectile.velocity = fireProjectPosition.TransformDirection(new Vector3(0, 0, fireProjectileSpeed));
        instantiatedProjectile.gameObject.transform.parent = GameManager.instance.tempObjects;
        StartCoroutine(AutoDetroyFire(instantiatedProjectile));
    }

    private IEnumerator AutoDetroyFire(Rigidbody projectile)
    {
        yield return new WaitForSeconds(timeForProjectileDestroy);
        if (projectile && projectile.gameObject.activeSelf) Destroy(projectile.gameObject);
    }
}
