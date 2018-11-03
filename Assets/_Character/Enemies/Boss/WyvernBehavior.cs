using UnityEngine;


public class WyvernBehavior : MonoBehaviour
{
    [SerializeField] private float attackingRadius = 20f;
    [SerializeField] private float timeOnPlan = 10f; // if it is out of this time, it will fly on the air
    [SerializeField] private float timeFireAttacking = 10f;

    [SerializeField] private GameObject skeletonGroup;
    private bool isAlive = true;
    private WyvernAttacking wyvernAttacking;
    private Animator animator;

    private HealthSystem wyvernHealth;

    private PlayerControl player;

    private Vector3 wingLeftPos, wingRighPos, headPos;

    private float distanceToPlayer;

    // Use this for initialization
    void Start()
    {
        wyvernAttacking = GetComponent<WyvernAttacking>();
        wyvernHealth = GetComponent<HealthSystem>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerControl>();

        var partOfBody = GetComponentsInChildren<MainBody>();
        /*
         * A bit cheating here. if pos.z < 0 is right and opposite
         */
        for (int i = 0; i < partOfBody.Length; i++)
        {
            var posBody = partOfBody[i].gameObject.transform;
            if (partOfBody[i].gameObject.tag == "WyvernWing")
            {
                //Debug.Log("Wings local pos: " + posBody.localPosition);
                if (posBody.localPosition.z > 0)
                {
                    wingLeftPos = posBody.position;
                }
                else wingRighPos = posBody.position;
                //Debug.Log("Wings pos: " + posBody);

            }
            else if (partOfBody[i].gameObject.tag == "WyvernHead")
            {
                //Debug.Log("Head pos: " + posBody);
                headPos = posBody.position;
            }

        }
    }

    public void PlayerOrEnemyAliveToContinue()
    {
        if (player.gameObject.GetComponent<HealthSystem>().HealthAsPercentage <= 0 ||
           wyvernHealth.HealthAsPercentage <= 0)
        {
            Destroy(this);//to stop enemies from continue moving even when died
        }
    }

    // Update is called once per frame
    void Update()
    {
        PlayerOrEnemyAliveToContinue();
        distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);
        if (distanceToPlayer < attackingRadius)
        {
            if (timeOnPlan <= 0)
            {
                animator.SetBool("enableFyling", true);
                timeOnPlan = 0;
            }
            else
            {
                timeOnPlan += Time.deltaTime;
            }
        }
    }

    void AttackingPlayer()
    {
        var distanceHead = Vector3.Distance(headPos, player.transform.position);
        var distanceLeftWing = Vector3.Distance(wingLeftPos, player.transform.position);
        var distanceRightWing = Vector3.Distance(wingRighPos, player.transform.position);

        if (distanceHead < distanceRightWing && distanceHead < distanceLeftWing)
        {
            // TODO: bite
            player.GetComponent<HealthSystem>().TakeDamage(wyvernAttacking.BiteAttacking());
        }
        else if (distanceRightWing < distanceLeftWing)
        {
            // TODO: right attacking
            player.GetComponent<HealthSystem>().TakeDamage(wyvernAttacking.RightAttacking());
        }
        else
        {
            // TODO: left attacking
            player.GetComponent<HealthSystem>().TakeDamage(wyvernAttacking.LeftAttacking());
        }
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        // Draw attack sphere 
        Gizmos.color = new Color(255f, 0, 0, .5f);
        Gizmos.DrawWireSphere(transform.position, attackingRadius);
    }
#endif
}
