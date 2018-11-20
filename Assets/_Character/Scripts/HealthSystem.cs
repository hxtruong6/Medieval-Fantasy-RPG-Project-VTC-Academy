using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]

public class HealthSystem : MonoBehaviour
{
    public float maxHealthPoints = 100f;
    [SerializeField] Image healthBar;
    [SerializeField] AudioClip[] damageSounds;
    [SerializeField] AudioClip[] deathSounds;
    [SerializeField] float deadVanishAfter = 2f;

    DamageTextSpawner damageTextSpawner;

    const string DEATH_TRIGGER = "Death";

    Animator animator;
    AudioSource audioSource;

    public float currentHealthPoints;
    float flashTime = 2f;
    bool isInvincible;

    public float HealthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    public bool GetInvincibility() { return isInvincible; }

    public void SetInvincible(bool value) { isInvincible = value; }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        damageTextSpawner = FindObjectOfType<DamageTextSpawner>();

        SetCurrentMaxHealth();
    }

    private void SetCurrentMaxHealth()
    {
        currentHealthPoints = maxHealthPoints;
    }

    void Update()
    {
        if (GetComponent<PlayerControl>() && GetComponent<PlayerControl>().isInDemonForm)
        {
            RegenHealth();
        }
        UpdateHealthBar();
    }

    public void SetAnimator(Animator ani)
    {
        animator = ani;
    }

    void UpdateHealthBar()
    {
        if (healthBar)
        {
            healthBar.fillAmount = HealthAsPercentage;
        }
    }

    private void RegenHealth()
    {
        if (currentHealthPoints == 0)
            return;
        var pointsToAdd = GetComponent<DemonTrigger>().regenHealthPerSec * Time.deltaTime;
        currentHealthPoints = Mathf.Clamp(currentHealthPoints + pointsToAdd, 0, maxHealthPoints);
    }

    public void TakeDamage(float damage)
    {
        if (GetComponent<PlayerControl>())
        {
            if (isInvincible)
                return;
            if (!GetComponent<PlayerControl>().isInDemonForm)
                GetComponent<RageSystem>().GainRagePoints(GetComponent<RageSystem>().attackedGain);
        }
        if (GetComponent<Enemy>())
        {
            FlashEnemyHealthBar();
        }

        Color textColor = GameManager.instance.enemyDmgTextColor;

        if (GetComponent<PlayerControl>())
            textColor = GameManager.instance.playerDmgTextColor;

        damageTextSpawner.Create(damage, transform.position, textColor);

        bool characterDies = (currentHealthPoints - damage) <= 0;
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        //play sound

        if (damageSounds.Length > 0)
        {
            var clip = damageSounds[Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip);
        }

        UpdateHealthBar();

        if (characterDies)
        {
            StartCoroutine(KillCharacter());
        }
    }

    public void RestoreAmount(float amount)
    {
        currentHealthPoints = Mathf.Clamp(currentHealthPoints + amount, 0f, maxHealthPoints);
    }

    public void RestorePercentage(int percentage)
    {
        float healAmount = maxHealthPoints / 100 * percentage;
        RestoreAmount(healAmount);
    }

    public void FlashEnemyHealthBar()
    {
        if (!GetComponent<Enemy>())
            return;

        StopAllCoroutines();
        var healthBar = transform.Find(GameManager.instance.ENEMY_UI);
        StartCoroutine(FlashHealthBar(healthBar.gameObject));
    }

    IEnumerator FlashHealthBar(GameObject healthBar)
    {
        healthBar.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        if (GetComponent<InteractiveEnemy>().isSelected != true)
            healthBar.SetActive(false);
    }

    IEnumerator KillCharacter()
    {
        var playerComponent = GetComponent<PlayerControl>();

        animator.SetTrigger(DEATH_TRIGGER);

        audioSource.Play();

        if (playerComponent)
        {
            playerComponent.StopAllCoroutines();
            playerComponent.StopMoving();
            playerComponent.gameObject.transform.position = playerComponent.gameObject.transform.position;
            playerComponent.Killed();
        }
        else
        {
            EnemyKilledAndDropLoot();
        }

        yield return new WaitForSecondsRealtime(deadVanishAfter);

        if (playerComponent)
        {
            GameManager.instance.PlayerContinueCheck();
        }
    }

    private void EnemyKilledAndDropLoot()
    {
        GetComponent<CapsuleCollider>().enabled = false;

        if (GetComponent<BoxCollider>())
            GetComponent<BoxCollider>().enabled = false;
        if (GetComponent<DropLoot>())
            GetComponent<DropLoot>().DropWeaponAndItem();
        if (GetComponent<Enemy>() != null)
            GetComponent<Enemy>().StartCoroutine(GetComponent<Enemy>().Kill(deadVanishAfter));
    }

    public void PlayHitEffect(GameObject effectPrefab, float effectLiveTime)
    {
        var particleObject = Instantiate
        (
            effectPrefab,
            transform.position,
            effectPrefab.transform.rotation
        );
        particleObject.transform.parent = transform;
        particleObject.GetComponent<ParticleSystem>().Play();
        //particleObject.transform.parent = GameManager.instance.tempObjects;
        StartCoroutine(DestroyParticleAfterFinishedSec(particleObject, effectLiveTime));
    }

    IEnumerator DestroyParticleAfterFinishedSec(GameObject effectPrefab, float effectLiveTime)
    {
        while (effectPrefab.GetComponent<ParticleSystem>().isPlaying)
        {
            yield return new WaitForSeconds(effectLiveTime);
        }
        Destroy(effectPrefab);
    }

}