using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] float maxHealthPoints = 100f;
    [SerializeField] Image healthBar;
    [SerializeField] AudioClip[] damageSounds;
    [SerializeField] AudioClip[] deathSounds;
    [SerializeField] float deadVanishAfter = 2f;

    const string DEATH_TRIGGER = "Death";
    Animator animator;
    AudioSource audioSource;
    Character character;

    float currentHealthPoints;

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

    IEnumerator KillCharacter()
    {
        character.Kill();
        animator.SetTrigger(DEATH_TRIGGER);
        var playerComponent = GetComponent<PlayerControl>();
        audioSource.Play();
        yield return new WaitForSecondsRealtime(deadVanishAfter);

        if (playerComponent && playerComponent.isActiveAndEnabled)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
