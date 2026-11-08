using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private ObstacleMovement obstacleTemplate;
    private bool collected;

    public void Initialize(ObstacleMovement template)
    {
        obstacleTemplate = template;
    }

    private void Start()
    {
        if (obstacleTemplate == null)
        {
            Debug.LogError("CoinPickup: obstacle template is not assigned by CoinSpawner.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        Vector3 position = transform.position;
        position.y -= obstacleTemplate.Speed * Time.deltaTime;
        transform.position = position;

        if (position.y < -6f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled || collected)
            return;

        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if (health == null || health.IsDead)
            return;

        CoinWallet wallet = other.GetComponent<CoinWallet>();
        if (wallet == null)
        {
            Debug.LogError("CoinPickup: player has no CoinWallet component.", other);
            return;
        }

        collected = true;
        wallet.AddCoin();
        Destroy(gameObject);
    }
}
