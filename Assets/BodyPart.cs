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
    // TODO: add part of damage;
    public float headDamge = 30f;
    public BodyPartType partType;

    void Start()
    {
        // TODO: if enough damage -> show animation
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
