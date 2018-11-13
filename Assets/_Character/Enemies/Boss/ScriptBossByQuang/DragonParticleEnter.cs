using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DragonParticleEnter : MonoBehaviour {
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;
    public int damage = 4;
    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log("hit");
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        int i = 0;

        while (i < numCollisionEvents)
        {
            
                this.gameObject.GetComponent<HealthSystem>().TakeDamage(damage);
                //Vector3 pos = collisionEvents[i].intersection;
                //Vector3 force = collisionEvents[i].velocity * 10;
                //rb.AddForce(force);
            i++;
        }
    }
}
