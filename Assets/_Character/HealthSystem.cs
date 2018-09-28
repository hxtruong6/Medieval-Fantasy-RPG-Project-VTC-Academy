using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
        if (healthBar)//Enemies may not have health bar to update
        {
            healthBar.fillAmount = healthAsPercentage;
        }
    }
    //TODO Impliment TakeDamage()

    //TODO Impliment KillCharacter()
}
