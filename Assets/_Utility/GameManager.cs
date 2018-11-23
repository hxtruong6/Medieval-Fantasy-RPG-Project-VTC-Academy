using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] KeyCode openCheatPanelKey = KeyCode.BackQuote;
    [SerializeField] KeyCode openHelpPanel = KeyCode.H;
    [SerializeField] KeyCode showLootItemsKey = KeyCode.LeftAlt;
    [SerializeField] KeyCode pauseKey = KeyCode.Escape;

    public HealthPotionConfig smallHealthPotion;
    public HealthPotionConfig largeHealthPotion;
    public ManaPotionConfig smallManaPotion;
    public ManaPotionConfig largeManaPotion;
    public int cheatPotionAddAmount = 10;
    public WeaponConfig legendaryWeapon;
    public WeaponConfig masterWeapon;
    public GameObject enemyManager;
    public Transform tempObjects;
    public int playerChances = 2;
    public GameObject cheatScene;
    public GameObject helpScene;
    public Color enemyDmgTextColor;
    public Color playerDmgTextColor;
    public GameObject continuePanel;
    public Text continueQuestionText;
    public Button continueYesButton;
    public Button continueNoButton;
    public Text textMessage;
    public GameObject pauseMenu;
    public GameObject gameCanvas;
    public GameObject loadingCanvas;

    [HideInInspector] public string ENEMY_UI = "Enemy Canvas";
    public float PARTICLE_CLEAN_UP_DELAY = 10f;

    public static GameManager instance;

    bool isPaused;
    PlayerControl player;
    bool cheatSceneOn;

    void Start()
    {
        player = FindObjectOfType<PlayerControl>();
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKey(pauseKey) && !isPaused)
        {
            pauseMenu.SetActive(true);
            gameCanvas.SetActive(false);
            PauseGame();
        }

        if (Input.GetKeyDown(showLootItemsKey))
        {
            if(tempObjects.GetComponentsInChildren<LootItem>().Length > 0)
            {
                for (int i = 0; i < tempObjects.GetComponentsInChildren<LootItem>().Length; i++)
                {
                    tempObjects.GetComponentsInChildren<LootItem>()[i].GetComponent<InfoItem>().HighLight(true);
                }
            }
        }
        if (Input.GetKeyUp(showLootItemsKey))
        {
            for (int i = 0; i < tempObjects.GetComponentsInChildren<LootItem>().Length; i++)
            {
                tempObjects.GetComponentsInChildren<LootItem>()[i].GetComponent<InfoItem>().HighLight(false);
            }
        }

        if (Input.GetKeyDown(openCheatPanelKey) && !isPaused)
        {
            cheatScene.SetActive(true);
            PauseGame();
            cheatSceneOn = true;
        }
        if (Input.GetKeyUp(openCheatPanelKey) && cheatSceneOn)
        {
            cheatScene.SetActive(false);
            UnPauseGame();
            cheatSceneOn = false;
        }
        if (Input.GetKeyDown(openHelpPanel) && !isPaused)
        {
            OpenHelpScene();
        }
        if (Input.GetKeyUp(openHelpPanel) && helpScene.activeInHierarchy)
        {
            CloseHelpScene();
        }
        if (isPaused && cheatSceneOn)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                RestoreFullHealth();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                CheatRestoreFullMana();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                RestoreFullHealth();
                bool isPlayerInvincible = player.GetComponent<HealthSystem>().GetInvincibility();
                player.GetComponent<HealthSystem>().SetInvincible(!isPlayerInvincible);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
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
            if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                CheatUnlimitedRage();
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                player.GetComponent<HealthSystem>().TakeDamage(999999);
            }
        }
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        player.GetComponent<PlayerControl>().enabled = false;
    }

    private void UnPauseGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        player.GetComponent<PlayerControl>().enabled = true;
    }

    private void OpenHelpScene()
    {
        helpScene.SetActive(true);
        isPaused = true;
        Time.timeScale = 0;
        player.GetComponent<PlayerControl>().enabled = false;
    }

    private void CloseHelpScene()
    {
        helpScene.SetActive(false);
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
        if (player.isInDemonForm)
            return;
        if (legendaryWeapon != null)
            player.GetComponent<InventorySystem>().PickUpNewWeapon(legendaryWeapon);
    }

    private void CheatGetMasterWeapon()
    {
        if (player.isInDemonForm)
            return;
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

    private void CheatUnlimitedRage()
    {
        var maxRagePoints = player.GetComponent<RageSystem>().GetMaxRagePoints();
        player.GetComponent<RageSystem>().GainRagePoints(maxRagePoints);
        player.GetComponent<RageSystem>().isUnlimited = !player.GetComponent<RageSystem>().isUnlimited;
    }

    public void PlayerContinueCheck()
    {
        if (GameObject.FindObjectOfType<DragonBoss>() != null) { DragonBoss.PlayerDeath(); }
        continueQuestionText.text = "YOU DIED\n" + playerChances + " CHANCES LEFT.\nCONTINUE?";
        if (playerChances == 0)
            continueYesButton.interactable = false;

        continuePanel.SetActive(true);
    }

    private void PlayerChooseContinue()
    {
        playerChances--;
        player.GetComponent<HealthSystem>().RestorePercentage(100);
        player.GetComponent<EnergySystem>().RestorePercentage(100);
        player.GetComponent<RageSystem>().currentRagePoints = 0;
        player.GetComponent<RageSystem>().UpdateRageBar();

        player.GetComponent<DemonTrigger>().humanForm.SetActive(false);

        player.isAlive = true;

        if (player.isInDemonForm)
            player.GetComponent<DemonTrigger>().TurnBackToHumanForm();

        player.GetComponent<Animator>().Play("Grounded");

        continuePanel.SetActive(false);

        player.GetComponent<DemonTrigger>().humanForm.SetActive(true);
        if (GameObject.FindObjectOfType<DragonBoss>() != null) { DragonBoss.PlayerAlive(); }
    }

    public void PlayerChooseQuit()
    {
        continuePanel.SetActive(false);
        UnPauseGame();
        loadingCanvas.SetActive(true);
        SceneManager.LoadScene("MainLobby");
    }

    public void DisplayMessage(string message)
    {
        StopAllCoroutines();
        textMessage.text = message;
        StartCoroutine(CloseMessage());
    }

    IEnumerator CloseMessage()
    {
        yield return new WaitForSeconds(3f);
        textMessage.text = "";
    }

    public void ButtonContinue()
    {
        gameCanvas.SetActive(true);
        pauseMenu.SetActive(false);
        UnPauseGame();
    }

    public void ButtonBackToMenu()
    {
        loadingCanvas.SetActive(true);
        pauseMenu.SetActive(false);
        UnPauseGame();
        SceneManager.LoadScene("MainLobby");
    }

    public void ButtonExit()
    {
        Application.Quit();
    }
}
