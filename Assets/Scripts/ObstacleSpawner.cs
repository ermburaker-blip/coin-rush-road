using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float[] lanePositions = { -6f, -4f, -2f, 0f, 2f, 4f };
    [SerializeField] private float spawnInterval = 2f;

    private readonly List<ObstacleMovement> spawnedMovements = new List<ObstacleMovement>();
    private readonly List<GameObject> spawnedObstacles = new List<GameObject>();
    private float timeUntilSpawn = 2f;
    private bool stopped;

    public ObstacleMovement ObstacleTemplate
    {
        get { return obstaclePrefab != null ? obstaclePrefab.GetComponentInChildren<ObstacleMovement>(true) : null; }
    }

    public bool CanResume
    {
        get
        {
            return isActiveAndEnabled && playerHealth != null && ObstacleTemplate != null &&
                lanePositions != null && lanePositions.Length > 0 &&
                spawnInterval > 0f && !float.IsNaN(spawnInterval) && !float.IsInfinity(spawnInterval);
        }
    }

    public bool TryGetRandomLane(out float x)
    {
        x = 0f;
        if (lanePositions == null || lanePositions.Length == 0)
            return false;

        x = lanePositions[Random.Range(0, lanePositions.Length)];
        return true;
    }

    private void OnEnable()
    {
        if (playerHealth == null)
        {
            Debug.LogError("ObstacleSpawner: playerHealth is not assigned.", this);
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

        if (obstaclePrefab == null || lanePositions == null || lanePositions.Length == 0 ||
            spawnInterval <= 0f || float.IsNaN(spawnInterval) || float.IsInfinity(spawnInterval))
        {
            Debug.LogError("ObstacleSpawner: assign a prefab, at least one lane, and a positive finite spawn interval.", this);
            enabled = false;
            return;
        }

        timeUntilSpawn -= Time.deltaTime;
        if (timeUntilSpawn > 0f)
            return;

        float x = lanePositions[Random.Range(0, lanePositions.Length)];
        GameObject obstacle = Instantiate(obstaclePrefab, new Vector3(x, 6f, 0f), Quaternion.identity, transform);
        ObstacleMovement[] movements = obstacle.GetComponentsInChildren<ObstacleMovement>(true);
        if (movements.Length == 0)
            Debug.LogError("ObstacleSpawner: spawned obstacle has no ObstacleMovement component.", obstacle);

        spawnedMovements.RemoveAll(movement => movement == null);
        spawnedMovements.AddRange(movements);
        spawnedObstacles.RemoveAll(obstacleObject => obstacleObject == null);
        spawnedObstacles.Add(obstacle);
        timeUntilSpawn = spawnInterval;
    }

    public void StopSpawning()
    {
        stopped = true;
        foreach (ObstacleMovement movement in spawnedMovements)
        {
            if (movement != null)
                movement.enabled = false;
        }
    }

    public void ResumeSpawning()
    {
        foreach (GameObject obstacle in spawnedObstacles)
        {
            if (obstacle != null)
                Destroy(obstacle);
        }
        spawnedObstacles.Clear();
        spawnedMovements.Clear();
        timeUntilSpawn = spawnInterval;
        stopped = false;
    }
}
