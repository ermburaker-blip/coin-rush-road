using UnityEngine;

public class ObstacleMovement : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    private bool dealtDamage;

    public float Speed { get { return speed; } }

    private void Update()
    {
        Vector3 position = transform.position;
        position.y -= speed * Time.deltaTime;
        transform.position = position;

        if (position.y < -6f)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled || dealtDamage)
            return;

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null)
            return;

        dealtDamage = true;
        playerHealth.TakeDamage(1);
        Destroy(gameObject);
    }
}
