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

    [SerializeField] private CurrentState currentState;
    [SerializeField] private Transform wingLeftPos, wingRighPos, headPos, legPos;
    [SerializeField] private GameObject areaAttacking;

    [SerializeField] private AudioClip flyAudioClip;
  


    private WyvernSkeletonSpawn wyvernSkeletonSpawn;
    private WyvernAttacking wyvernAttacking;
    private WyvernFireProjectile wyvernFireProjectile;
    private Animator animator;

    private HealthSystem wyvernHealth;

    private PlayerControl player;

    private float distanceToPlayer;
    private float timeOnPlan;

    private float flyingSpeed = 10f;

    private float timeToFlying = 0.5f;
    private float timeDirectUpdate;
    
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
        animator = GetComponentInChildren<Animator>();

        wyvernHealth.SetAnimator(animator);

        wyvernFireProjectile = GetComponent<WyvernFireProjectile>();
        player = FindObjectOfType<PlayerControl>();
        wyvernSkeletonSpawn = GetComponent<WyvernSkeletonSpawn>();

        timeOnPlan = 0f;
        currentState = CurrentState.Idle;

        //var partOfBody = GetComponentsInChildren<MainBody>();
        ///*
        // * A bit cheating here. if pos.z < 0 is right and opposite
        // */
        //for (int i = 0; i < partOfBody.Length; i++)
        //{
        //    var posBody = partOfBody[i].gameObject.transform;
        //    if (partOfBody[i].gameObject.tag == "WyvernWing")
        //    {
        //        //Debug.Log("Wings local pos: " + posBody.localPosition);
        //        if (posBody.localPosition.z > 0)
        //        {
        //            wingLeftPos = posBody.position;
        //        }
        //        else wingRighPos = posBody.position;
        //        //Debug.Log("Wings pos: " + posBody);

        //    }
        //    else if (partOfBody[i].gameObject.tag == "WyvernHead")
        //    {
        //        //Debug.Log("Head pos: " + posBody);
        //        headPos = posBody.position;
        //    }

        //}
    }

    public bool PlayerOrEnemyAliveToContinue()
    {
        //player = GameObject.FindObjectOfType<PlayerControl>();
        if (GetComponent<HealthSystem>().HealthAsPercentage <= 0)
        {

            this.enabled = false;//to stop enemies from continue moving even when died
            return false;
        }
        else if (player.GetComponent<HealthSystem>().HealthAsPercentage <= 0)
        {
            return false;
        }
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!PlayerOrEnemyAliveToContinue()) return;
        distanceToPlayer = Vector3.Distance(this.transform.position, player.transform.position);
        //FlyingBehaviour();
        switch (currentState)
        {
            case CurrentState.Attacking:
                if (timeOnPlan >= timeOnPlanLimited)
                {
                    currentState = CurrentState.Flying;
                    timeOnPlan = 0;
                    timeDirectUpdate = 0f;
                    wyvernSkeletonSpawn.DisplaySkeletonSpawn();
                    EnableFlying();
                    StopCoroutine(AttackingInArea());
                    StartCoroutine(FlyingBehaviour());
                    break;
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
                    //StartCoroutine(AttackingInArea());
                    areaAttacking.SetActive(true);
                    AttackingPlayer();
                }


                timeOnPlan += Time.deltaTime;
                timeDirectUpdate += Time.deltaTime;
                if (timeDirectUpdate > 1f)
                {
                    timeDirectUpdate = 0;
                    var ro = transform.rotation;
                    transform.LookAt(player.transform.position);
                    transform.rotation = ro;
                }
                break;
            case CurrentState.Flying:
                //if all of skeleton died
                if (wyvernSkeletonSpawn.IsCurrentSkeletonGroupDie())
                {
                    //Debug.Log("Falling :v");
                    currentState = CurrentState.Falling;
                    StartCoroutine(FallingBehaviour());
                }
                flyingSpeed += Time.deltaTime / 2;

                break;
            case CurrentState.Falling:
                flyingSpeed += Time.deltaTime * 0.75f;
                //if (flyingSpeed > timeToFlying*2) flyingSpeed -= timeToFlying;
                if (Vector3.Distance(animator.transform.position, transform.position) <= 1.2f)
                {
                    StopAllCoroutines();
                    GetComponentInParent<AudioSource>().Stop();
                    currentState = CurrentState.Idle;
                    // TODO: make sure player can't attack
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

    private IEnumerator AttackingInArea()
    {
        var healthPlayer = player.GetComponent<HealthSystem>();
        while (player && distanceToPlayer < attackingRadius)
        {
            yield return new WaitForSeconds(2f);
            wyvernAttacking.AreaAttacking(healthPlayer);
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
        GetComponent<AudioSource>().PlayOneShot(flyAudioClip);
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
        //Debug.Log("Head: " + headPos + "|Left" + wingLeftPos + "\n|Right: " + wingRighPos + "|Leg: " + transform.position);
        var distanceHead = Vector3.Distance(headPos.position, player.transform.position);
        var distanceLeftWing = Vector3.Distance(wingLeftPos.position, player.transform.position);
        var distanceRightWing = Vector3.Distance(wingRighPos.position, player.transform.position);
        var distanceSwoop = Vector3.Distance(legPos.position, player.transform.position);
        //Debug.Log("Head: " + distanceHead + "|Left" + distanceLeftWing + "\n|Right: " + distanceRightWing + "|Leg: " + distanceSwoop);
        //transform.LookAt(player.transform.position);
        if (distanceHead < distanceRightWing && distanceHead < distanceLeftWing && distanceHead < distanceSwoop)
        {
            // TODO: bite
            wyvernAttacking.BiteAttacking();
        }
        else if (distanceRightWing < distanceLeftWing && distanceHead < distanceSwoop)
        {
            // TODO: right attacking
            wyvernAttacking.RightAttacking();
        }
        else if (distanceLeftWing < distanceSwoop)
        {
            // TODO: left attacking
            wyvernAttacking.LeftAttacking();
        }
        else
        {
            wyvernAttacking.SwoopClaw();
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
