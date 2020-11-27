using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CustomEvents;

public class GameManager : MonoBehaviour
{
    public GameObject brickWall;
    public GameObject steelWall;

    public GameObject pauseMenu;
    public Text livesText;

    public int numberEnemyToDestroy = 6;
    public int playerLives = 3;

    public float timePowerCoreWallRecovering = 5.0f;

    private bool isGameOver = false;
    private bool isGamePaused = false;
    private bool goToNextStage = false;

    private Player playerCs;
    private PowerCore powerCoreCs;
    private SpawnManager spawnManagerCs;
    private InputManager inputManagerCs;

    private float freezingEnemyTimer = 0.0f;
    private bool isFreezingEnemies = false;

    private float powerCoreWallRecoveringTimer = 0.0f;
    private bool isPowerCoreWallRecovering = false;

    // For demo only
    private bool isDemoOver = false;

    // Start is called before the first frame update
    void Start()
    {
        steelWall.SetActive(false);

        powerCoreCs = GameObject.Find("PowerCore").GetComponent<PowerCore>();
        spawnManagerCs = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        inputManagerCs = GameObject.Find("InputManager").GetComponent<InputManager>();

        powerCoreCs.OnDestroyed += OnGameOver;
        spawnManagerCs.OnPlayerSpawned += SetPlayer;
        inputManagerCs.OnGamePauseResumed += GamePauseResume;

        livesText.text = "x " + playerLives;
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnManagerCs.GetEnemyDestroyedCount() == numberEnemyToDestroy)
        {
            if (!isGameOver || !isDemoOver)
                StartCoroutine(GameOver());
            else if (!goToNextStage)
                StartCoroutine(NextStage());
        }
        else
        {
            if (spawnManagerCs.GetTotalEnemyCount() == numberEnemyToDestroy)
            {
                spawnManagerCs.StopSpawn();
            }

            if (isFreezingEnemies)
            {
                freezingEnemyTimer -= Time.deltaTime;

                if (freezingEnemyTimer < 0.0f)
                {
                    UnFreezingEnemies();
                }
            }

            if (isPowerCoreWallRecovering)
            {
                powerCoreWallRecoveringTimer -= Time.deltaTime;

                if (powerCoreWallRecoveringTimer < 0.0f)
                {
                    isPowerCoreWallRecovering = false;

                    steelWall.SetActive(false);
                    brickWall.GetComponent<BrickWallController>().Recover();
                    brickWall.SetActive(true);
                }
            }
        }
    }

    private IEnumerator NextStage()
    {
        goToNextStage = true;

        yield return new WaitForSeconds(5);

        SessionData.Instance.isDemoOver = true;

        SceneManager.LoadScene("LoadingScene");
    }

    private IEnumerator GameOver()
    {
        isGameOver = true;
        isDemoOver = true;

        yield return new WaitForSeconds(5);

        SessionData.Instance.isDemoOver = true;

        SceneManager.LoadScene("LoadingScene");
    }

    private void OnGameOver()
    {
        StartCoroutine(GameOver());
    }

    private void GamePauseResume()
    {
        SoundManager.Instance.GamePauseResume();

        playerCs.ToggleSound();

        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0 : 1.0f;

        pauseMenu.SetActive(isGamePaused);
    }

    private void TryRespawnPlayer(GameObject player)
    {
        SoundManager.Instance.PlayerDestroyed();

        ChangeLives(-1);

        if (playerLives > 0)
        {
            // Super cheat to reset player's level
            playerCs.ChangeLevel(-99);

            spawnManagerCs.MovePlayerBackToSpawnPoint();
        }
        else
        {
            StartCoroutine(GameOver());
        }
    }

    public void SetPlayer(GameObject player)
    {
        playerCs = player.GetComponent<Player>();
        playerCs.OnDestroyed += TryRespawnPlayer;
    }

    public void DestroyAllEnemies()
    {
        spawnManagerCs.DestroyAllEnemies();
    }

    public void ChangeLives(int amount)
    {
        playerLives += amount;

        livesText.text = "x " + playerLives;
    }

    public void FreezingEnemies(float timeFreezing)
    {
        isFreezingEnemies = true;
        freezingEnemyTimer = timeFreezing;

        spawnManagerCs.FreezingEnemies();
    }

    public void UnFreezingEnemies()
    {
        isFreezingEnemies = false;

        spawnManagerCs.UnFreezingEnemies();
    }

    public void PowerCoreRecoverWall()
    {
        brickWall.SetActive(false);
        steelWall.SetActive(true);

        isPowerCoreWallRecovering = true;
        powerCoreWallRecoveringTimer = timePowerCoreWallRecovering;
    }

    // choice = 1: retry, choice = 0: main menu
    public void PauseMenuSelection(int choice)
    {
        Time.timeScale = 1;

        if (choice == 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (choice == 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
