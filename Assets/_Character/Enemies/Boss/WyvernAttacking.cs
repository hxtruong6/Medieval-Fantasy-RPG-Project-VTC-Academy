using UnityEngine;

public class WyvernAttacking : MonoBehaviour
{
    [SerializeField] private float biteDamage = 5f;

    [SerializeField] private float wingDamage = 6f;

    [SerializeField] private float fireDamage = 7f;
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

    public float BiteAttacking()
    {
        animator.SetBool("isBiting", true);
        return biteDamage;
    }

    public float LeftAttacking()
    {
        animator.SetBool("isLeftAttacking", true);
        return wingDamage;
    }

    public float RightAttacking()
    {
        animator.SetBool("isRightAttacking", true);
        return wingDamage;
    }

    public float FireAttacking()
    {
        Debug.Log("endFlying and start fire attacking");
        animator.SetBool("enableFlying", false);
        animator.SetBool("enableFireAttacking", true);
        return fireDamage;
    }

    void endBiting()
    {
        Debug.Log("End Biting");
        animator.SetBool("isBiting", false);
    }
    void endLeftAttacking()
    {
        Debug.Log("endLeftAttacking");
        animator.SetBool("isLeftAttacking", false);
    }
    void endRightAttacking()
    {
        Debug.Log("endRightAttacking");
        animator.SetBool("isRightAttacking", false);
    }
    public void endFireAttacking()
    {
        Debug.Log("endFireAttacking");
        animator.SetBool("enableFireAttacking", false);
    }

}
