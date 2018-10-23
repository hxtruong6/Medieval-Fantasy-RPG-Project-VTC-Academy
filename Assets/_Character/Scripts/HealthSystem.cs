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

    const string DEATH_TRIGGER = "Death";
    const string ENEMY_UI = "Enemy Canvas";
    Animator animator;
    AudioSource audioSource;
    Character character;

    float currentHealthPoints;
    float flashTime = 2f;

    public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; } }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        character = GetComponent<Character>();

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
        FlashEnemyHealthBar();

        bool characterDies = (currentHealthPoints - damage) <= 0;
        currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
        //play sound
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
        healthBar.SetActive(false);
    }

    IEnumerator KillCharacter()
    {
        GetComponent<CapsuleCollider>().isTrigger = true;
        animator.SetTrigger(DEATH_TRIGGER);
        var playerComponent = GetComponent<PlayerControl>();
        audioSource.Play();
        if(playerComponent)
        {
            playerComponent.Killed();
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

}
