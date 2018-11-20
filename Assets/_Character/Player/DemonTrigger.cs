using UnityEngine;
using UnityEngine.AI;

public class DemonTrigger : MonoBehaviour
{
    [SerializeField] GameObject humanForm;
    [SerializeField] GameObject demonForm;
    [SerializeField] KeyCode demonTriggerKey = KeyCode.R;
    [SerializeField] AnimatorOverrideController demonAnimatorOC;
    [SerializeField] Avatar humanAvatar;
    [SerializeField] Avatar demonAvatar;
    [SerializeField] float humanRunSpeed = 1f;
    [SerializeField] float demonRunSpeed = 4f;
    [SerializeField] WeaponConfig replaceMeleeWeapon;
    [SerializeField] WeaponConfig replaceRangedWeapon;
    [SerializeField] ProjectileConfig replaceProjectileConfig;
    public GameObject normalMeleeAttackEffect;
    public float normalMeleeAOERadius = 3f;
    public float regenHealthPerSec = 1;

    const string DEMON_TRIGGER = "DemonTrigger";

    Animator animator;
    PlayerControl player;
    NavMeshAgent agent;
    InventorySystem inventory;
    RageSystem rage;

    public AnimatorOverrideController GetOverrideController()
    {
        return demonAnimatorOC;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerControl>();
        agent = GetComponent<NavMeshAgent>();
        inventory = GetComponent<InventorySystem>();
        rage = GetComponent<RageSystem>();
    }

    void Update ()
    {
        if (Input.GetKeyDown(demonTriggerKey) && player.GetComponent<HealthSystem>().HealthAsPercentage > 0)
        {
            if(!player.isInDemonForm && rage.RagePointAsPercent == 1)
            {
                PlayAnimationTrigger();
            }
            if (player.isInDemonForm)
            {
                TurnBackToHumanForm();
            }
        }
    }

    public void TurnBackToHumanForm()
    {
        player.StopCurrentAction();
        player.StopMoving();
        rage.isActived = false;
        player.isInDemonForm = false;
        animator.runtimeAnimatorController = GetComponent<Character>().GetOverrideController();
        ChangeForm();
        ChangeWeapon();
        bool isMeleeWeapon = GetComponent<WeaponSystem>().GetCurrentWeapon().IsMeleeWeapon();
        GetComponent<SpecialAbilities>().LockMeleeAbilites(!isMeleeWeapon);
        animator.Play("Grounded");
        GetComponent<WeaponSystem>().SetAttackAnimation();
    }

    private void PlayAnimationTrigger()
    {
        player.StopCurrentAction();
        player.StopMoving();
        animator.SetTrigger(DEMON_TRIGGER);
    }

    private void TurnIntoDemonForm()
    {
        animator.runtimeAnimatorController = demonAnimatorOC;
        player.isInDemonForm = true;
        ChangeForm();
        ChangeWeapon();
        rage.isActived = true;
        GetComponent<SpecialAbilities>().LockAllAbilities();
    }

    private void ChangeForm()
    {
        humanForm.SetActive(!player.isInDemonForm);
        demonForm.SetActive(player.isInDemonForm);
        player.StopMoving();
        if (player.isInDemonForm)
        {
            animator.avatar = demonAvatar;
            agent.speed = demonRunSpeed;
        }
        else
        {
            animator.avatar = humanAvatar;
            agent.speed = humanRunSpeed;
        }      
    }

    private void ChangeWeapon()
    {
        WeaponConfig tempWeapon = inventory.GetEquippedMeleeWeapon();
        inventory.PickUpNewWeapon(replaceMeleeWeapon);
        replaceMeleeWeapon = tempWeapon;
        tempWeapon = inventory.GetEquippedRangedWeapon();
        inventory.PickUpNewWeapon(replaceRangedWeapon);
        replaceRangedWeapon = tempWeapon;
        ProjectileConfig tempProjectile = GetComponent<WeaponSystem>().GetCurrentProjectileConfig();
        GetComponent<WeaponSystem>().SetCurrentProjectileConfig(replaceProjectileConfig);
        replaceProjectileConfig = tempProjectile;
    }

    //public void PlayTriggerEffect(GameObject effectPrefab, float effectLiveTime)
    //{
    //    var particleObject = Instantiate
    //    (
    //        effectPrefab,
    //        transform.position,
    //        effectPrefab.transform.rotation
    //    );
    //    particleObject.transform.parent = transform;
    //    particleObject.GetComponent<ParticleSystem>().Play();
    //    particleObject.transform.parent = GameManager.instance.tempObjects;
    //    StartCoroutine(DestroyParticleAfterFinishedSec(particleObject, effectLiveTime));
    //}

    //IEnumerator DestroyParticleAfterFinishedSec(GameObject effectPrefab, float effectLiveTime)
    //{
    //    while (effectPrefab.GetComponent<ParticleSystem>().isPlaying)
    //    {
    //        yield return new WaitForSeconds(effectLiveTime);
    //    }
    //    Destroy(effectPrefab);
    //}
}
