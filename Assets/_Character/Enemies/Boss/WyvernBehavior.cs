using System.Collections;
using UnityEngine;


public class WyvernBehavior : MonoBehaviour
{
    private enum CurrentState
    {
        Idle = 0,
        Attacking,
        Flying,
        Falling
    }

    [SerializeField] private float attackingRadius = 10f;
    [SerializeField] private float fireAttackingRadius = 20f;
    [SerializeField] private float timeOnPlanLimited = 10f; // if it is out of this time, it will fly on the air

    //[SerializeField] private float fallingSpeed = 10f;
    [SerializeField] private float timeForFireShooting = 0.1f;
    [SerializeField] private float maxFlyingHeight = 10f;
    [SerializeField] private float timeToFlying = 0.5f;
    [SerializeField] private CurrentState currentState;
    //[SerializeField] private float flyTimeLimited = 10f;


    private WyvernSkeletonSpawn wyvernSkeletonSpawn;
    private WyvernAttacking wyvernAttacking;
    private WyvernFireProjectile wyvernFireProjectile;
    private Animator animator;

    private HealthSystem wyvernHealth;

    private PlayerControl player;

    private Vector3 wingLeftPos, wingRighPos, headPos;

    private float distanceToPlayer;
    private float timeOnPlan;

    private float flyTime;
    //private float currentFallingSpeed;
    private float flyingSpeed = 10f;



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
        wyvernSkeletonSpawn = GetComponent<WyvernSkeletonSpawn>();

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
                    wyvernSkeletonSpawn.DisplaySkeletonSpawn();
                    EnableFlying();
                    StartCoroutine(FlyingBehaviour());
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
                //if all of skeleton died
                if (wyvernSkeletonSpawn.IsCurrentSkeletonGroupDie())
                {
                    flyTime = 0f;
                    currentState = CurrentState.Falling;
                    StartCoroutine(FallingBehaviour());
                }
                flyingSpeed += Time.deltaTime / 2;
                //currentFallingSpeed -= Time.deltaTime;
                //Debug.Log("Is flying. Waiting player kill all of skeleton");
                //flyTime += Time.deltaTime;
                //if (flyTime >= flyTimeLimited)
                //{
                //    flyTime = 0f;
                //    currentState = CurrentState.Falling;
                //    StartCoroutine(FallingBehaviour());
                //}
                break;
            case CurrentState.Falling:
                flyingSpeed += Time.deltaTime * 0.75f;
                //if (flyingSpeed > timeToFlying*2) flyingSpeed -= timeToFlying;
                if (Vector3.Distance(animator.transform.position, transform.position) <= 1f)
                {
                    StopAllCoroutines();
                    currentState = CurrentState.Idle;
                    DisableFlying();
                    animator.transform.position = Vector3.Lerp(animator.transform.position, transform.position, 0.2f);
                }
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

    private IEnumerator FallingBehaviour()
    {
        flyingSpeed = 0;
        while (Vector3.Distance(animator.transform.position, transform.position) > 1f)
        {
            yield return new WaitForSeconds(timeToFlying);
            var newPos = animator.transform.position;
            newPos.y -= Mathf.Pow(1.1f, flyingSpeed);
            Debug.Log("Dis: " + Vector3.Distance(animator.transform.position, transform.position));
            //animator.transform.position = newPos;
            animator.transform.position = Vector3.Lerp(animator.transform.position, newPos, flyingSpeed);
        }

    }

    IEnumerator FlyingBehaviour()
    {
        flyingSpeed = 0;
        while (Vector3.Distance(animator.transform.position, transform.position) <= maxFlyingHeight)
        {
            yield return new WaitForSeconds(timeToFlying);
            var newPos = animator.transform.position;
            newPos.y += Mathf.Pow(1.1f, flyingSpeed);
            //animator.transform.position = newPos;
            animator.transform.position = Vector3.Lerp(animator.transform.position, newPos, flyingSpeed);
            yield return new WaitForSeconds(timeToFlying / 2);
            newPos = animator.transform.position;
            newPos.y -= Time.deltaTime;
            animator.transform.position = newPos;
        }
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
        //Debug.Log("Fire is shooting....");
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
