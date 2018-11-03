using UnityEngine;
[RequireComponent(typeof(HealthSystem))]
[RequireComponent(typeof(Animator))]

/*
 * This class use to get damage from player 
 */
public class WyvernDamge : MonoBehaviour
{
    [SerializeField] private float wingDamage = 20f;
    [SerializeField] private float legDamge = 15f;
    [SerializeField] private float tailDamage = 10f;
    [SerializeField] private float headDamge = 30f;
    [SerializeField] private float foreDamage = 15f;

    private HealthSystem wyvernHealth;

    private Animator wyvernAnimator;

    private PlayerControl player;
    // Use this for initialization
    void Start()
    {
        wyvernHealth = GetComponent<HealthSystem>();
        wyvernAnimator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            // decision what ani will act?
            wyvernAnimator.SetTrigger("GetHit");
            var bodyTag = this.gameObject.tag;
            Debug.Log("Tag of Wyvern: " + bodyTag);

            if (bodyTag == "WyvernHead")
            {
                wyvernHealth.TakeDamage(headDamge);
            }
            else if (bodyTag == "WyvernWing")
            {
                wyvernHealth.TakeDamage(wingDamage);
            }
            else if (bodyTag == "WyvernLeg")
            {
                wyvernHealth.TakeDamage(legDamge);
            }
            else if (bodyTag == "WyvernForearm")
            {
                wyvernHealth.TakeDamage(foreDamage);
            }
            else if (bodyTag == "WyvernTail")
            {
                wyvernHealth.TakeDamage(tailDamage);
            }
        }
    }


}
