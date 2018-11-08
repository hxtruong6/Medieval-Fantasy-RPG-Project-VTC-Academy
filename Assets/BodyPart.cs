using UnityEngine;

public enum BodyPartType
{
    Head = 0,
    Wing,
    Leg,
    Forearm,
    Tail,
    Body

}
public class BodyPart : MonoBehaviour
{
    public BodyPartType partType;
    public float headDamage = 30f;
    public float wingDamage = 15f;
    public float legDamage = 15f;
    public float forearmDamage = 15f;
    public float tailDamage = 20f;
    public float bodyDamage = 25f;

    void Start()
    {
      
        //GetComponentInParent<Animator>()
    }
    public void OnCollisionEnter(Collision collision)
    {
        // TODO: weapon of player
        if (collision.gameObject.GetComponent<PlayerControl>())
        {
            var damageController = GetComponentInParent<WyvernDamge>();
            damageController.OnBodyPartHit(partType);
        }
    }
}
