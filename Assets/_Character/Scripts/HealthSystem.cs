using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]

public class HealthSystem : MonoBehaviour
{
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] Image healthBar;
    [SerializeField] AudioClip[] damageSounds;
    [SerializeField] AudioClip[] deathSounds;
    [SerializeField] float deadVanishAfter = 2f;

    DamageTextSpawner damageTextSpawner;

    const string DEATH_TRIGGER = "Death";
    const string ENEMY_UI = "Enemy Canvas";
    const string TEMP_OBJECTS = "TempObjects";

    Animator animator;
    AudioSource audioSource;

    float currentHealthPoints;
    float flashTime = 2f;

    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        damageTextSpawner = GetComponent<DamageTextSpawner>();

        SetCurrentMaxHealth();
    }

    private void SetCurrentMaxHealth()
    {
        currentHealthPoints = maxHealthPoints;
    }

    void Update()
    {
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar)
        {
            healthBar.fillAmount = healthAsPercentage;
        }
    }

    public void TakeDamage(float damage)
    {
        print("Hello");
        FlashEnemyHealthBar();

        bool characterDies = (currentHealthPoints - damage) <= 0;
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        //play sound
        //damageTextSpawner.Create(damage, transform.position);
        var clip = damageSounds[Random.Range(0, damageSounds.Length)];
        audioSource.PlayOneShot(clip);

        

        if (characterDies)
        {
            StartCoroutine(KillCharacter());
        }
    }

    private void FlashEnemyHealthBar()
    {
        if (!GetComponent<Enemy>())
            return;

        StopAllCoroutines();
        var healthBar = transform.Find(ENEMY_UI);
        StartCoroutine(FlashHealthBar(healthBar.gameObject));        
    }

    IEnumerator FlashHealthBar(GameObject healthBar)
    {
        healthBar.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        if(GetComponent<InteractiveEnemy>().isSelected != true)
            healthBar.SetActive(false);
    }

    IEnumerator KillCharacter()
    {
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;//TODO check with designer, player dont need it

        animator.SetTrigger(DEATH_TRIGGER);
        var playerComponent = GetComponent<PlayerControl>();
        audioSource.Play();
        if(playerComponent)
        {
            playerComponent.Killed();
        }

        if(GetComponent<DropItem>())
        {
            GetComponent<DropItem>().DropLoot();
        }

        yield return new WaitForSecondsRealtime(deadVanishAfter);

        if (playerComponent && playerComponent.isActiveAndEnabled)
        {
            Destroy(playerComponent);
            Destroy(GetComponent<Character>());
            SceneManager.LoadScene(0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayCriticalHitParticle(GameObject effectPrefab, float effectLiveTime)
    {
        var particleObject = Instantiate
        (
            effectPrefab,
            transform.position,
            effectPrefab.transform.rotation
        );
        particleObject.transform.parent = transform;
        particleObject.transform.parent = GameObject.FindGameObjectWithTag(TEMP_OBJECTS).transform;
        particleObject.GetComponent<ParticleSystem>().Play();
        particleObject.transform.parent = GameObject.FindGameObjectWithTag(TEMP_OBJECTS).transform;
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
