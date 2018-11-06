using UnityEngine;


public class WyvernBehavior : MonoBehaviour
{
    private enum CurrentState
    {
        Idle = 0,
        Attacking,
        Chasing,
        Flying
    }

    [SerializeField] private float attackingRadius = 10f;
    [SerializeField] private float chasingRadius = 20f;    
    [SerializeField] private float timeOnPlanLimited = 10f; // if it is out of this time, it will fly on the air
    [SerializeField] private float timeFireAttacking = 10f;
    [SerializeField] private float flyingSpeed = 10f;


    [SerializeField] private GameObject skeletonGroup;
    private bool isAlive = true;
    private WyvernAttacking wyvernAttacking;
    private Animator animator;

    private HealthSystem wyvernHealth;

    private PlayerControl player;

    private Vector3 wingLeftPos, wingRighPos, headPos;

    private float distanceToPlayer;
    private float timeOnPlan;
    private CurrentState currentState;


    // Use this for initialization
    void Start()
    {
        wyvernAttacking = GetComponent<WyvernAttacking>();
        wyvernHealth = GetComponent<HealthSystem>();
        animator = GetComponent<Animator>();
        player = FindObjectOfType<PlayerControl>();
        timeOnPlan = 0f;
        currentState = CurrentState.Idle;


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
        FlyingBehaviour();
        switch (currentState)
        {
            case CurrentState.Attacking:
                break;
            case CurrentState.Chasing:
                break;
            case CurrentState.Flying:
                break;
            case CurrentState.Idle:
                if (distanceToPlayer <= chasingRadius){
                    // TODO: attack fire
                }
                
                break;
        }

        // if (distanceToPlayer < attackingRadius)
        // {
        //     this.transform.LookAt(player.transform.position);
        //     if (timeOnPlan >= timeOnPlanLimited)
        //     {
        //         if (currentState != CurrentState.Flying)
        //         {
        //             animator.SetBool("enableFlying", true);
        //             currentState = CurrentState.Flying;
        //             timeOnPlan = 0;
        //         }
        //         else
        //         {
        //             //TODO: condition to stop flying to go to plan
        //             FlyingBehaviour();
        //         }
        //     }
        //     else
        //     {
        //         timeOnPlan += Time.deltaTime;
        //         AttackingPlayer();
        //     }
        // }
    }

    private void FlyingBehaviour()
    {
        //transform.position.z += Time.deltaTime * flyingSpeed;
        Debug.Log("Wyvern fly");
        //gameObject.GetComponent<Rigidbody>().velocity +=  Vector3.up * flyingSpeed;
        //transform.position += transform.up * Time.deltaTime * flyingSpeed;
        transform.Translate(0,Time.deltaTime*flyingSpeed, 0);
        var rg = gameObject.GetComponent<Rigidbody>();
        rg.useGravity = false;
        //rg.MovePosition(rg.position + transform.up * Time.deltaTime * flyingSpeed );
    }

    void AttackingPlayer()
    {
        var distanceHead = Vector3.Distance(headPos, player.transform.position);
        var distanceLeftWing = Vector3.Distance(wingLeftPos, player.transform.position);
        var distanceRightWing = Vector3.Distance(wingRighPos, player.transform.position);

        if (distanceHead < distanceRightWing && distanceHead < distanceLeftWing)
        {
            // TODO: bite
            wyvernAttacking.BiteAttacking();
        }
        else if (distanceRightWing < distanceLeftWing)
        {
            // TODO: right attacking
            wyvernAttacking.RightAttacking();
        }
        else
        {
            // TODO: left attacking
            wyvernAttacking.LeftAttacking();
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
