using UnityEngine;

public class WyvernAttacking : MonoBehaviour
{
    [SerializeField] private float biteDamage = 5f;

    [SerializeField] private float wingDamage = 6f;

    [SerializeField] private float fireDamage = 7f;

    private enum CurrentAttacking
    {
        None = 0,
        WingAttacking,
        BiteAttacking,
        FireAttacking,
        SwoopClaw,
    }

    private CurrentAttacking currentAttacking = CurrentAttacking.None;

    // Use this for initialization
    private Animator animator;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }
    public void BiteAttacking()
    {
        currentAttacking = CurrentAttacking.BiteAttacking;
        animator.SetTrigger("enableBiting");
    }

    public void LeftAttacking()
    {
        currentAttacking = CurrentAttacking.WingAttacking;
        animator.SetTrigger("enableLeftAttacking");
    }

    public void RightAttacking()
    {
        currentAttacking = CurrentAttacking.WingAttacking;
        animator.SetTrigger("enableRightAttacking");
    }

    public void FireAttacking()
    {
        currentAttacking = CurrentAttacking.FireAttacking;
        //Debug.Log("Start fire attacking");
        animator.SetTrigger("enableFireAttacking");
    }

    public void SwoopClaw()
    {
        currentAttacking = CurrentAttacking.SwoopClaw;
        //Debug.Log(transform.rotation);
        animator.SetTrigger("enableSwoopClaw");
        //Debug.Log("2---> " +transform.rotation);
    }
    public void switchState()
    {
        currentAttacking = CurrentAttacking.None;
    }

    void OnCollisionEnter(Collision other)
    {
        //Debug.Log("Collider: " + other. gameObject.tag);
        if (other.gameObject.tag == "Player")
        {
            switch (currentAttacking)
            {
                case CurrentAttacking.BiteAttacking:
                    Debug.Log("Bite");
                    other.gameObject.GetComponent<HealthSystem>().TakeDamage(biteDamage);
                    break;
                case CurrentAttacking.WingAttacking:
                    Debug.Log("Wing");
                    other.gameObject.GetComponent<HealthSystem>().TakeDamage(wingDamage);
                    break;
                case CurrentAttacking.FireAttacking:
                    Debug.Log("Fire");
                    other.gameObject.GetComponent<HealthSystem>().TakeDamage(fireDamage);
                    break;
            }
        }
    }


}
