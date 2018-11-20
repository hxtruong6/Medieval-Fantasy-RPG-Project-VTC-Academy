using UnityEngine;

public class WyvernAttacking : MonoBehaviour
{
    [SerializeField] private float biteDamage = 5f;

    [SerializeField] private float wingDamage = 6f;

    //[SerializeField] private float fireDamage = 7f;

    [SerializeField] private float swoopDamage = 8f;
    [SerializeField] private float areaDamage = 5f;

    [Header("Audio Attacking")]
    [SerializeField] private AudioClip wingAttack;
    [SerializeField] private AudioClip biteAttack;
    [SerializeField] private AudioClip fireAttack;
    [SerializeField] private AudioClip swoopAttack;
    [SerializeField] private GameObject wyvernEventAttack;
    private AudioSource wyvernAudio;
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
        wyvernAudio = GetComponent<AudioSource>();
    }
    public void BiteAttacking()
    {
        currentAttacking = CurrentAttacking.BiteAttacking;
        animator.SetTrigger("enableBiting");
        if (!wyvernAudio.isPlaying)
        {
            wyvernAudio.pitch = Random.Range(1f, 1.3f);
            wyvernAudio.PlayOneShot(biteAttack);
        }
        wyvernEventAttack.SetActive(true);

    }

    public bool AnimatorIsPlaying()
    {
        return animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
    }

    public void LeftAttacking()
    {

        currentAttacking = CurrentAttacking.WingAttacking;
        animator.SetTrigger("enableLeftAttacking");
        if (!wyvernAudio.isPlaying)
        {
            wyvernAudio.pitch = Random.Range(1f, 1.3f);
            wyvernAudio.PlayOneShot(wingAttack);
        }
        wyvernEventAttack.SetActive(true);
    }

    public void RightAttacking()
    {

        currentAttacking = CurrentAttacking.WingAttacking;
        animator.SetTrigger("enableRightAttacking");
        if (!wyvernAudio.isPlaying)
        {
            wyvernAudio.pitch = Random.Range(1f, 1.3f);
            wyvernAudio.PlayOneShot(wingAttack);
        }
        wyvernEventAttack.SetActive(true);
    }

    public void FireAttacking()
    {
        currentAttacking = CurrentAttacking.FireAttacking;
        //Debug.Log("Start fire attacking");
        animator.SetTrigger("enableFireAttacking");
        if (!wyvernAudio.isPlaying)
        {
            wyvernAudio.pitch = Random.Range(1f, 1.3f);
            wyvernAudio.PlayOneShot(fireAttack);
        }
    }

    public void SwoopClaw()
    {

        currentAttacking = CurrentAttacking.SwoopClaw;
        //Debug.Log(transform.rotation);
        animator.SetTrigger("enableSwoopClaw");
        //Debug.Log("2---> " +transform.rotation);
        if (!wyvernAudio.isPlaying)
        {
            wyvernAudio.pitch = Random.Range(1f, 1.3f);
            wyvernAudio.PlayOneShot(swoopAttack);
        }
        wyvernEventAttack.SetActive(true);
    }
    public void switchState()
    {
        currentAttacking = CurrentAttacking.None;
    }

    public void AreaAttacking(HealthSystem health)
    {
        //Debug.Log(currentAttacking);
        if (currentAttacking != CurrentAttacking.None)
            health.TakeDamage(0);
    }
    void OnCollisionEnter(Collision other)
    {
        return;
        //Debug.Log("Collider: " + other. gameObject.tag);
        if (other.gameObject.tag == "Player")
        {
            switch (currentAttacking)
            {
                case CurrentAttacking.BiteAttacking:
                    //Debug.Log("Bite");
                    other.gameObject.GetComponent<HealthSystem>().TakeDamage(biteDamage);
                    break;
                case CurrentAttacking.WingAttacking:
                    //Debug.Log("Wing");
                    other.gameObject.GetComponent<HealthSystem>().TakeDamage(wingDamage);
                    break;
                //case CurrentAttacking.FireAttacking:
                //    Debug.Log("Fire");
                //    other.gameObject.GetComponent<HealthSystem>().TakeDamage(fireDamage);
                //    break;
                case CurrentAttacking.SwoopClaw:
                    other.gameObject.GetComponent<HealthSystem>().TakeDamage(swoopDamage);
                    break;
            }
        }
    }


}
