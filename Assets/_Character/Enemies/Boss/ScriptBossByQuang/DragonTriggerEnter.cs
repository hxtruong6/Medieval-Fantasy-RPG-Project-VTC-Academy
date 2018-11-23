using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonTriggerEnter : MonoBehaviour
{
    private float timer;
    public float timeActive;
    public bool onDamage;
    public int damage;
    private int damageDelay;
    // Use this for initialization
    void Start()
    {
        timer = timeActive;
        damageDelay = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer <= 0)
        {
            this.gameObject.SetActive(false);
            timer = timeActive;
            damageDelay = 1;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (onDamage)
        {
            if (other.GetComponent<PlayerControl>())
            {
                other.GetComponent<HealthSystem>().TakeDamage(damage);
            }
        }
        else
        {
            if (other.GetComponent<PlayerControl>() && damageDelay == 1)
            {
                damageDelay--;
                other.GetComponent<HealthSystem>().TakeDamage(damage);
            }
        }
    }

}
