using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public HealthPotionConfig smallHealthPotion;
    public HealthPotionConfig largeHealthPotion;
    public ManaPotionConfig smallManaPotion;
    public ManaPotionConfig largeManaPotion;    
    public int cheatPotionAddAmount = 10;
    public WeaponConfig legendaryWeapon;
    public WeaponConfig masterWeapon;
    public GameObject enemyManager;
    public Transform tempObjects;
    public GameObject cheatScene;


    [HideInInspector]  public string ENEMY_UI = "Enemy Canvas";
    public float PARTICLE_CLEAN_UP_DELAY = 10f;

    public static GameManager instance;

    bool isPaused;
    PlayerControl player;

    void Start ()
    {
        player = FindObjectOfType<PlayerControl>();
        instance = this;
        DontDestroyOnLoad(this);
	}

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.BackQuote))
        {
            PauseGame();
        }
        if(Input.GetKeyUp(KeyCode.BackQuote))
        {
            UnPauseGame();
        }
        if(isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                RestoreFullHealth();
            }
            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                CheatRestoreFullMana();
            }
            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                RestoreFullHealth();
                bool isPlayerInvincible = player.GetComponent<HealthSystem>().GetInvincibility();
                player.GetComponent<HealthSystem>().SetInvincible(!isPlayerInvincible);
            }
            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                CheatRestoreFullMana();
                bool isPlayerUnlimitedPower = player.GetComponent<EnergySystem>().GetPowerLimit();
                player.GetComponent<EnergySystem>().SetLimitPower(!isPlayerUnlimitedPower);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                CheatGetLegendaryWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                CheatGetMasterWeapon();
            }
            if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                CheatAddAllPotion();
            }
            if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                CheatDisableAllMobEnemy();
            }
        }
    }

    private void PauseGame()
    {
        cheatScene.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
        player.GetComponent<PlayerControl>().enabled = false;
    }

    private void UnPauseGame()
    {
        cheatScene.SetActive(false);
        isPaused = false;
        Time.timeScale = 1;
        player.GetComponent<PlayerControl>().enabled = true;
    }

    private void RestoreFullHealth()
    {
        player.GetComponent<HealthSystem>().RestorePercentage(100);
    }

    private void CheatRestoreFullMana()
    {
        player.GetComponent<EnergySystem>().RestorePercentage(100);
    }

    private void CheatGetLegendaryWeapon()
    {
        if(legendaryWeapon!=null)
            player.GetComponent<InventorySystem>().PickUpNewWeapon(legendaryWeapon);
    }

    private void CheatGetMasterWeapon()
    {
        if (masterWeapon != null)
            player.GetComponent<InventorySystem>().PickUpNewWeapon(masterWeapon);
    }

    private void CheatAddAllPotion()
    {
        player.GetComponent<InventorySystem>().AmountOfSmallHPotion += cheatPotionAddAmount;
        player.GetComponent<InventorySystem>().AmountOfLargeHPotion += cheatPotionAddAmount;
        player.GetComponent<InventorySystem>().AmountOfSmallMPotion += cheatPotionAddAmount;
        player.GetComponent<InventorySystem>().AmountOfLargeMPotion += cheatPotionAddAmount;
    }

    private void CheatDisableAllMobEnemy()
    {
        enemyManager.SetActive(!enemyManager.activeInHierarchy);
    }
}
