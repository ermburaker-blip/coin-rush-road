using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private TMP_Text distanceText;
    [SerializeField] private GameObject levelCompleteText;
    [SerializeField] private GameObject finishLine;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private LaneMovement laneMovement;
    [SerializeField] private ObstacleSpawner obstacleSpawner;
    [SerializeField] private CoinSpawner coinSpawner;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private float targetDistance = 100f;

    private const int ContinueCost = 6;
    private float distance;
    private float startPlayerY;
    private bool stopped;
    private bool completed;
    private bool continued;
    private bool initialized;
    private bool restarting;

    public float Distance { get { return distance; } }

    private void Awake()
    {
        if (distanceText == null || levelCompleteText == null || finishLine == null ||
            playerHealth == null || laneMovement == null || obstacleSpawner == null || coinSpawner == null ||
            continueButton == null || restartButton == null || coinWallet == null)
        {
            Debug.LogError("LevelProgress: assign all Inspector references.", this);
            enabled = false;
            return;
        }

        if (targetDistance <= 0f || float.IsNaN(targetDistance) || float.IsInfinity(targetDistance))
        {
            Debug.LogError("LevelProgress: target distance must be positive and finite.", this);
            enabled = false;
            return;
        }

        startPlayerY = laneMovement.transform.position.y;
        Vector3 position = finishLine.transform.position;
        position.y = startPlayerY + targetDistance;
        finishLine.transform.position = position;
        levelCompleteText.SetActive(false);
        continueButton.SetActive(false);
        restartButton.SetActive(false);
        UpdateDistanceText();
        initialized = true;
    }

    private void OnEnable()
    {
        if (!initialized)
            return;

        playerHealth.Died += StopProgress;
        if (playerHealth.IsDead)
            StopProgress();
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.Died -= StopProgress;
    }

    private void Update()
    {
        if (stopped || playerHealth.IsDead)
            return;

        ObstacleMovement template = obstacleSpawner.ObstacleTemplate;
        if (template == null || template.Speed <= 0f ||
            float.IsNaN(template.Speed) || float.IsInfinity(template.Speed))
        {
            Debug.LogError("LevelProgress: obstacle prefab needs an ObstacleMovement with positive finite speed.", this);
            enabled = false;
            return;
        }

        distance = Mathf.Min(targetDistance, distance + template.Speed * Time.deltaTime);
        Vector3 position = finishLine.transform.position;
        position.y = startPlayerY + targetDistance - distance;
        finishLine.transform.position = position;
        UpdateDistanceText();

        if (distance >= targetDistance)
            CompleteLevel();
    }

    private void CompleteLevel()
    {
        stopped = true;
        completed = true;
        continueButton.SetActive(false);
        restartButton.SetActive(true);
        laneMovement.enabled = false;
        obstacleSpawner.StopSpawning();
        coinSpawner.StopSpawning();
        levelCompleteText.SetActive(true);
    }

    private void StopProgress()
    {
        stopped = true;
        continueButton.SetActive(!completed && !continued && coinWallet.Balance >= ContinueCost);
        restartButton.SetActive(true);
    }

    public void ContinueRun()
    {
        if (!initialized || !isActiveAndEnabled || restarting || completed || continued || !playerHealth.IsDead ||
            coinWallet.Balance < ContinueCost)
            return;

        if (!obstacleSpawner.CanResume || !coinSpawner.CanResume)
        {
            Debug.LogError("LevelProgress: spawners must be active and correctly configured to continue.", this);
            return;
        }

        if (!coinWallet.TrySpend(ContinueCost))
            return;

        continued = true;
        obstacleSpawner.ResumeSpawning();
        coinSpawner.ResumeSpawning();
        playerHealth.Revive();
        continueButton.SetActive(false);
        restartButton.SetActive(false);
        laneMovement.enabled = true;
        stopped = false;
    }

    public void RestartRun()
    {
        if (!initialized || !isActiveAndEnabled || restarting || (!completed && !playerHealth.IsDead))
            return;

        Scene scene = SceneManager.GetActiveScene();
        if (scene.buildIndex < 0 || !Application.CanStreamedLevelBeLoaded(scene.buildIndex))
        {
            Debug.LogError("LevelProgress: active scene must be enabled in Build Settings to restart.", this);
            return;
        }

        restarting = true;
        SceneManager.LoadScene(scene.buildIndex);
    }

    private void UpdateDistanceText()
    {
        distanceText.text = $"Distance: {Mathf.FloorToInt(distance)} m / {Mathf.FloorToInt(targetDistance)} m";
    }
}
