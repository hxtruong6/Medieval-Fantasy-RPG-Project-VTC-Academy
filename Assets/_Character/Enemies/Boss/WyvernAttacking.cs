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
        FireAttacking
    }

    private CurrentAttacking currentAttacking = CurrentAttacking.None;

    // Use this for initialization
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BiteAttacking()
    {
        currentAttacking = CurrentAttacking.BiteAttacking;
        animator.SetBool("isBiting", true);
    }

    public void LeftAttacking()
    {
        currentAttacking = CurrentAttacking.WingAttacking;
        animator.SetBool("isLeftAttacking", true);
    }

    public void RightAttacking()
    {
        currentAttacking = CurrentAttacking.WingAttacking;
        animator.SetBool("isRightAttacking", true);
    }


    public void FireAttacking()
    {
        currentAttacking = CurrentAttacking.FireAttacking;
        Debug.Log("endFlying and start fire attacking");
        animator.SetBool("enableFlying", false);
        animator.SetBool("enableFireAttacking", true);
    }

    void endBiting()
    {
        Debug.Log("End Biting");
        animator.SetBool("isBiting", false);
        currentAttacking = CurrentAttacking.None;
    }
    void endLeftAttacking()
    {
        Debug.Log("endLeftAttacking");
        animator.SetBool("isLeftAttacking", false);
        currentAttacking = CurrentAttacking.None;
    }
    void endRightAttacking()
    {
        Debug.Log("endRightAttacking");
        animator.SetBool("isRightAttacking", false);
        currentAttacking = CurrentAttacking.None;
    }
    public void endFireAttacking()
    {
        Debug.Log("endFireAttacking");
        animator.SetBool("enableFireAttacking", false);
        currentAttacking = CurrentAttacking.None;
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collider: " + other. gameObject.tag);
        if (other.gameObject.tag == "Player")
        {
            switch (currentAttacking)
            {
                case CurrentAttacking.BiteAttacking:
                    Debug.Log("Bite");
                    other.gameObject.GetComponent<HealthSystem>().TakeDamage(biteDamage);
                    break;
                case CurrentAttacking.WingAttacking:
                    other.gameObject.GetComponent<HealthSystem>().TakeDamage(wingDamage);
                    break;
                case CurrentAttacking.FireAttacking:
                    other.gameObject.GetComponent<HealthSystem>().TakeDamage(fireDamage);
                    break;
            }
        }
    }


}
