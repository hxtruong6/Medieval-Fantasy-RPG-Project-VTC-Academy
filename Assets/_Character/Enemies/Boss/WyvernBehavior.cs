using System.Collections;
using UnityEngine;


public class WyvernBehavior : MonoBehaviour
{
    private enum CurrentState
    {
        Idle = 0,
        Attacking,
        Flying
    }

    [SerializeField] private float attackingRadius = 10f;
    [SerializeField] private float fireAttackingRadius = 20f;
    [SerializeField] private float timeOnPlanLimited = 10f; // if it is out of this time, it will fly on the air
    [SerializeField] private float flyingSpeed = 10f;
    [SerializeField] private float timeForFireShooting = 0.1f;

    [SerializeField] private GameObject skeletonGroup;
    //private bool isAlive = true;
    private WyvernAttacking wyvernAttacking;
    private WyvernFireProjectile wyvernFireProjectile;
    private Animator animator;

    private HealthSystem wyvernHealth;

    private PlayerControl player;

    private Vector3 wingLeftPos, wingRighPos, headPos;

    private float distanceToPlayer;
    private float timeOnPlan;
    [SerializeField] private CurrentState currentState;
    [SerializeField] private float flyTimeLimited = 10f;
    private float flyTime;


    [ExecuteInEditMode]
    void OnValidate()
    {
        fireAttackingRadius = Mathf.Clamp(fireAttackingRadius, attackingRadius + 5f, attackingRadius + 40f);
    }
    // Use this for initialization
    void Start()
    {
        wyvernAttacking = GetComponent<WyvernAttacking>();
        wyvernHealth = GetComponent<HealthSystem>();
        wyvernFireProjectile = GetComponent<WyvernFireProjectile>();
        animator = GetComponentInChildren<Animator>();
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
        //FlyingBehaviour();
        switch (currentState)
        {
            case CurrentState.Attacking:
                if (timeOnPlan >= timeOnPlanLimited)
                {
                    currentState = CurrentState.Flying;
                    timeOnPlan = 0;
                    EnableFlying();
                    FlyingBehaviour();
                }
                else if (distanceToPlayer >= fireAttackingRadius)
                {
                    currentState = CurrentState.Idle;
                }
                else if (distanceToPlayer >= attackingRadius)
                {
                    FireAttackingPlayer();
                }
                else
                {
                    AttackingPlayer();
                }

                timeOnPlan += Time.deltaTime;
                break;
            case CurrentState.Flying:
                // TODO: if all of skeleton died
                Debug.Log("Is flying. Waiting player kill all of skeleton");
                flyTime += Time.deltaTime;
                if (flyTime >= flyTimeLimited)
                {
                    currentState = CurrentState.Idle;
                    DisableFlying();
                    flyTime = 0f;
                }
                //if (skeletonGroup.isAllKilled())
                //{
                //    currentState = CurrentState.Idle;
                //    DisableFlying();
                //}
                break;
            case CurrentState.Idle:
                if (distanceToPlayer <= fireAttackingRadius
                    || distanceToPlayer <= attackingRadius)
                {
                    currentState = CurrentState.Attacking;
                }
                //else if (timeOnPlan >= timeOnPlanLimited && )

                break;
        }

    }


    private void FlyingBehaviour()
    {
        // TODO: before flying spawn skeleton
        var newPos = animator.transform.position;
        newPos.y = 50;
        animator.transform.position = newPos;
    }

    private void EnableFlying()
    {
        //Debug.Log("Start flying");
        animator.SetBool("enableFlying", true);
    }
    private void DisableFlying()
    {
        //Debug.Log("End flying");
        animator.SetBool("enableFlying", false);
    }

    private void FireAttackingPlayer()
    {
        transform.LookAt(player.transform.position);
        wyvernAttacking.FireAttacking();
        // TODO: shooting fire
        Debug.Log("Fire is shooting....");
        StartCoroutine(FireSpawnLoop());
    }

    IEnumerator FireSpawnLoop()
    {
        while (currentState == CurrentState.Attacking &&
               distanceToPlayer >= attackingRadius && distanceToPlayer <= fireAttackingRadius)
        {
            yield return new WaitForSeconds(timeForFireShooting);
            wyvernFireProjectile.fireShooting();
        }
    }

    void AttackingPlayer()
    {
        var distanceHead = Vector3.Distance(headPos, player.transform.position);
        var distanceLeftWing = Vector3.Distance(wingLeftPos, player.transform.position);
        var distanceRightWing = Vector3.Distance(wingRighPos, player.transform.position);

        transform.LookAt(player.transform.position);
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
        // Draw fire attaking
        Gizmos.color = new Color(100f, 100f, 0, .5f);
        Gizmos.DrawWireSphere(transform.position, fireAttackingRadius);
    }
#endif
}
