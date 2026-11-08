using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private ObstacleSpawner obstacleSpawner;

    private const float SpawnInterval = 3f;
    private readonly List<CoinPickup> spawnedCoins = new List<CoinPickup>();
    private readonly List<GameObject> spawnedCoinObjects = new List<GameObject>();
    private float timeUntilSpawn = SpawnInterval;
    private bool stopped;

    public bool CanResume
    {
        get
        {
            return isActiveAndEnabled && playerHealth != null && obstacleSpawner != null &&
                coinPrefab != null && coinPrefab.GetComponent<CoinPickup>() != null &&
                obstacleSpawner.CanResume;
        }
    }

    private void OnEnable()
    {
        if (playerHealth == null || obstacleSpawner == null || coinPrefab == null)
        {
            Debug.LogError("CoinSpawner: assign coinPrefab, playerHealth, and obstacleSpawner.", this);
            enabled = false;
            return;
        }

        if (coinPrefab.GetComponent<CoinPickup>() == null || obstacleSpawner.ObstacleTemplate == null)
        {
            Debug.LogError("CoinSpawner: coin prefab needs CoinPickup and obstacle prefab needs ObstacleMovement.", this);
            enabled = false;
            return;
        }

        playerHealth.Died += StopSpawning;
        if (playerHealth.IsDead)
            StopSpawning();
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.Died -= StopSpawning;
    }

    private void Update()
    {
        if (stopped)
            return;

        timeUntilSpawn -= Time.deltaTime;
        if (timeUntilSpawn > 0f)
            return;

        float x;
        if (!obstacleSpawner.TryGetRandomLane(out x))
        {
            Debug.LogError("CoinSpawner: obstacle spawner has no configured lanes.", this);
            enabled = false;
            return;
        }

        ObstacleMovement template = obstacleSpawner.ObstacleTemplate;
        if (template == null)
        {
            Debug.LogError("CoinSpawner: obstacle prefab has no ObstacleMovement.", this);
            enabled = false;
            return;
        }

        GameObject coin = Instantiate(coinPrefab, new Vector3(x, 6f, 0f), Quaternion.identity, transform);
        CoinPickup pickup = coin.GetComponent<CoinPickup>();
        pickup.Initialize(template);
        spawnedCoins.RemoveAll(item => item == null);
        spawnedCoins.Add(pickup);
        spawnedCoinObjects.RemoveAll(item => item == null);
        spawnedCoinObjects.Add(coin);
        timeUntilSpawn = SpawnInterval;
    }

    public void StopSpawning()
    {
        stopped = true;
        foreach (CoinPickup coin in spawnedCoins)
        {
            if (coin != null)
                coin.enabled = false;
        }
    }

    public void ResumeSpawning()
    {
        foreach (GameObject coin in spawnedCoinObjects)
        {
            if (coin != null)
                Destroy(coin);
        }
        spawnedCoinObjects.Clear();
        spawnedCoins.Clear();
        timeUntilSpawn = SpawnInterval;
        stopped = false;
    }
}
