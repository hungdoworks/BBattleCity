using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomEvents;

public class SpawnPointData
{
    public Vector3 position;
    public GameObject ownedObject;
}

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance = null;

    public event GameObjectEventHandler OnPlayerSpawned;

    [Space(4)]
    public GameObject playerPrefab;
    public GameObject[] enemyPrefabs;
    public GameObject[] powerUpPrefabs;

    [Space(8)]
    public GameObject playerSpawnPoint;
    public GameObject[] enemySpawnPoints;
    public GameObject[] powerUpSpawnPoints;

    [Space(8)]
    public ParticleSystem explosive1;

    [Space(8)]
    public int maxEnemies = 4;
    public float enemySpawnDelay = 2.0f;
    public int maxPowerUpInStage = 4;
    public int dropRate = 0;

    private GameObject player;
    private GameObject powerUps;

    private List<PowerUp> spawnedPowerUp = new List<PowerUp>();
    private List<Enemy> spawnedEnemies = new List<Enemy>();
    private List<SpawnPointData> powerUpSpawnPointDatas = new List<SpawnPointData>();

    private int totalEnemyCount = 0;
    private int enemyCount = 0;
    private int enemyDestroyedCount = 0;

    private int spawnedPowerUpCount = 0;

    private float enemySpawnTimer = 0.0f;
    private int enemySpawnPosition = 0;

    private bool keepSpawning = true;
    private bool isFreezingMode = false;
    private bool isFirstUpdate = true;

    void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        powerUps = GameObject.Find("PowerUps");
    }

    // Update is called once per frame
    void Update()
    {
        if (isFirstUpdate)
        {
            isFirstUpdate = false;

            SetupPowerUpSpawnPoints();
            PreparePowerUps();
            SpawnPlayer();
        }

        if (!keepSpawning)
        {

        }
        else if (enemyCount < maxEnemies)
        {
            enemySpawnTimer -= Time.deltaTime;

            if (enemySpawnTimer < 0.0f)
            {
                SpawnEnemy();
            }
        }
    }

    private void OnEnemyTankDestroyed(GameObject enemy)
    {
        SoundManager.Instance.EnemyDestroyed();

        enemyCount--;
        enemyDestroyedCount++;

        Enemy enemyCs = enemy.GetComponent<Enemy>();
        if (enemyCs.dropItemOnDead)
        {
            SpawnPowerUp();
        }
    }

    private void OnPowerUpDestroyed(GameObject powerUp)
    {
        foreach (SpawnPointData pointData in powerUpSpawnPointDatas)
        {
            if (Mathf.Approximately(powerUp.transform.position.x, pointData.position.x) &&
                Mathf.Approximately(powerUp.transform.position.y, pointData.position.y))
            {
                pointData.ownedObject = null;

                break;
            }
        }
    }

    private void SetupPowerUpSpawnPoints()
    {
        foreach (GameObject point in powerUpSpawnPoints)
        {
            SpawnPointData spawnPoint = new SpawnPointData();
            spawnPoint.position = point.transform.position;
            spawnPoint.ownedObject = null;

            powerUpSpawnPointDatas.Add(spawnPoint);
        }
    }

    private void PreparePowerUps()
    {
        foreach (GameObject item in powerUpPrefabs)
        {
            GameObject powerUp = Instantiate(item, Vector3.zero, Quaternion.identity);
            PowerUp powerUpCs = powerUp.GetComponent<PowerUp>();

            powerUpCs.OnTimeOut += OnPowerUpDestroyed;

            spawnedPowerUp.Add(powerUpCs);

            powerUp.transform.SetParent(powerUps.transform);
            powerUp.SetActive(false);
        }
    }

    private void SpawnPowerUp()
    {
        if (spawnedPowerUpCount < maxPowerUpInStage)
        {
            SoundManager.Instance.PowerUpAppeared();

            spawnedPowerUpCount++;

            int index = Random.Range(0, 100) % spawnedPowerUp.Count;
            int locationIndex = Random.Range(0, 100) % powerUpSpawnPointDatas.Count;

            if (!CanSpawnAtLocation(powerUpSpawnPointDatas[locationIndex].position, LayerMask.GetMask("Player")))
            {
                locationIndex++;

                if (locationIndex >= powerUpSpawnPoints.Length)
                    locationIndex = 0;
            }

            if (powerUpSpawnPointDatas[locationIndex].ownedObject != null)
            {
                powerUpSpawnPointDatas[locationIndex].ownedObject.SetActive(false);
                powerUpSpawnPointDatas[locationIndex].ownedObject = null;
            }

            powerUpSpawnPointDatas[locationIndex].ownedObject = spawnedPowerUp[index].gameObject;

            spawnedPowerUp[index].gameObject.transform.position = powerUpSpawnPointDatas[locationIndex].position;
            spawnedPowerUp[index].MakeAvailable();
        }
    }

    private bool CanSpawnAtLocation(Vector2 location, int layerMask)
    {
        RaycastHit2D hit = Physics2D.BoxCast
        (
            location,
            new Vector2(2.0f, 2.0f),
            0.0f,
            Vector2.right,
            0,
            layerMask
        );

        return hit.collider == null;
    }

    private void SpawnEnemy()
    {
        if (CanSpawnAtLocation(
                enemySpawnPoints[enemySpawnPosition].transform.position,
                LayerMask.GetMask("Player") | LayerMask.GetMask("Enemy")))
        {
            enemySpawnTimer = enemySpawnDelay;
            enemyCount++;
            totalEnemyCount++;

            int enemyIndex = Random.Range(0, enemyPrefabs.Length - 1);
            GameObject enemy = Instantiate(enemyPrefabs[enemyIndex], enemySpawnPoints[enemySpawnPosition].transform.position, Quaternion.identity);
            Enemy enemyCs = enemy.GetComponent<Enemy>();

            enemyCs.dropItemOnDead = Random.Range(0, 100) <= dropRate;
            enemyCs.OnDestroyed += OnEnemyTankDestroyed;

            if (isFreezingMode)
                enemyCs.Freezing();

            spawnedEnemies.Add(enemyCs);

            enemySpawnPosition++;
            if (enemySpawnPosition == enemySpawnPoints.Length)
                enemySpawnPosition = 0;
        }
    }

    private void SpawnPlayer()
    {
        player = Instantiate(playerPrefab, playerSpawnPoint.transform.position, Quaternion.identity);

        if (OnPlayerSpawned != null)
            OnPlayerSpawned(player);
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(explosive1.main.duration);

        player.transform.position = playerSpawnPoint.transform.position;

		player.GetComponent<Player>().Reset();
    }

    public void SpawnExplosiveAt(Vector2 position)
    {
        Instantiate(explosive1, position, Quaternion.identity);
    }

    public void MovePlayerBackToSpawnPoint()
    {
        StartCoroutine(RespawnPlayer());
    }

    public int GetTotalEnemyCount()
    {
        return totalEnemyCount;
    }

    public int GetEnemyDestroyedCount()
    {
        return enemyDestroyedCount;
    }

    public void StopSpawn()
    {
        keepSpawning = false;
    }

    public void DestroyAllEnemies()
    {
        foreach (Enemy enemy in spawnedEnemies)
        {
            enemy.ChangeHealth(-10);
        }

        spawnedEnemies.Clear();
    }

    public void FreezingEnemies()
    {
        isFreezingMode = true;

        foreach (Enemy enemy in spawnedEnemies)
        {
            enemy.Freezing();
        }
    }

    public void UnFreezingEnemies()
    {
        isFreezingMode = false;

        foreach (Enemy enemy in spawnedEnemies)
        {
            enemy.UnFreezing();
        }

    }
}
