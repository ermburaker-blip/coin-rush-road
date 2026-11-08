using System;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private LaneMovement laneMovement;

    private int health = 3;

    public bool IsDead { get { return health == 0; } }
    public event Action Died;

    private void Awake()
    {
        if (healthText == null)
            Debug.LogError("PlayerHealth: healthText is not assigned.", this);
        if (gameOverText == null)
            Debug.LogError("PlayerHealth: gameOverText is not assigned.", this);
        if (laneMovement == null)
            Debug.LogError("PlayerHealth: laneMovement is not assigned.", this);

        UpdateHealthText();
        if (gameOverText != null)
            gameOverText.SetActive(false);
    }

    public void TakeDamage(int amount)
    {
        if (health == 0)
            return;

        if (amount < 0)
        {
            Debug.LogWarning("Damage amount cannot be negative.");
            return;
        }

        health = Mathf.Max(0, health - amount);
        Debug.Log("Remaining health: " + health);
        UpdateHealthText();

        if (health == 0)
        {
            if (gameOverText != null)
                gameOverText.SetActive(true);
            if (laneMovement != null)
                laneMovement.enabled = false;
            if (Died != null)
                Died();
        }
    }

    public void Revive()
    {
        if (!IsDead)
        {
            Debug.LogWarning("PlayerHealth: cannot revive a living player.", this);
            return;
        }

        health = 1;
        UpdateHealthText();
        if (gameOverText != null)
            gameOverText.SetActive(false);
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = "Can: " + health;
    }
}
