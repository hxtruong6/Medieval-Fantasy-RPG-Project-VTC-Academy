using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonBoss : MonoBehaviour {
    public Rigidbody rb;
    public float attackRange;
    public Image healthBar;
    public GameObject canvasUI;
    public GameObject attackR;
    public GameObject attackL;
    public GameObject attackC;
    public GameObject attackB;
    public GameObject attackICE;
    public GameObject attackAOE;
    public GameObject bossLockProgress;
    private float timer;
    private Animator anim;
    private PlayerControl player;
    private bool isDead;

    public Transform rightLeg;
    public Transform leftLeg;
    //private HealthSystem dragonHealth;
    static DragonBoss dragonBoss;
    // Use this for initialization
    void Awake()
    {
        dragonBoss = this;
    }
    void Start () {
        player = FindObjectOfType<PlayerControl>();
        //dragonHealth = GetComponent<HealthSystem>();
        anim = GetComponent<Animator>();
        timer = 3;
    }
	
	// Update is called once per frame
	void Update () {
        //PlayerOrEnemyAliveToContinue();
        if (player != null)
        {
            Vector3 targetPos = player.transform.position;
            Vector3 lookPlayer = new Vector3(targetPos.x, this.transform.position.y, targetPos.y);
            Vector3 lookPos = targetPos - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime);
            float distance = Vector3.Distance(transform.position, targetPos);
            if (healthBar.fillAmount > 0.7)
            {
                if (distance < attackRange)
                {
                    anim.SetBool("attackrange", true);
                }
                else
                {
                    anim.SetBool("attackrange", false);
                }
            }
            else if (healthBar.fillAmount > 0.3 && healthBar.fillAmount <= 0.7)
            {
                anim.SetBool("state2", true);
                if (distance < attackRange)
                {
                    anim.SetBool("attackrange", true);
                }
                else
                {
                    anim.SetBool("attackrange", false);
                }
            }
            else if (healthBar.fillAmount > 0 && healthBar.fillAmount <= 0.3)
            {
                anim.SetBool("state3", true);
            }
            else
            {
                if(isDead==false)
                {
                    anim.SetTrigger("death");
                    attackAOE.gameObject.SetActive(false);
                    canvasUI.gameObject.SetActive(false);
                    isDead = true;
                }
            }
        }
	}

    //public void ChangePlayerMeleeAttackRange(GameObject player)
    //{
    //    player.GetComponent<InventorySystem>().GetEquippedMeleeWeapon().SetAttackRange();
    //}

    public static void Attacking(int attackType)
    {
        if (attackType == 1)
        {
            dragonBoss.attackR.gameObject.SetActive(true);
        }
        if (attackType == 2)
        {
            dragonBoss.attackL.gameObject.SetActive(true);
        }
        if (attackType == 3)
        {
            dragonBoss.attackC.gameObject.SetActive(true);
        }
        if (attackType == 4)
        {
            dragonBoss.attackB.gameObject.SetActive(true);
        }
        if (attackType == 5)
        {
            dragonBoss.attackICE.gameObject.SetActive(true);
        }
        if (attackType == 6)
        {
            dragonBoss.attackAOE.gameObject.SetActive(true);
        }
    }
    //public void PlayerOrEnemyAliveToContinue()
    //{
    //    if (player.gameObject.GetComponent<HealthSystem>().HealthAsPercentage <= 0 ||
    //       dragonHealth.HealthAsPercentage <= 0)
    //    {
    //        Destroy(this);//to stop enemies from continue moving even when died
    //    }
    //}

    private void DestroyBoss()
    {
        Destroy(gameObject);
        Destroy(bossLockProgress);
    }
}
