using TMPro;
using UnityEngine;

public class CoinWallet : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;

    private int coins = 0;

    public int Balance { get { return coins; } }

    private void Awake()
    {
        if (coinsText == null)
            Debug.LogError("CoinWallet: coinsText is not assigned.", this);

        UpdateCoinsText();
    }

    public void AddCoin()
    {
        coins += 2;
        UpdateCoinsText();
    }

    public bool TrySpend(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("CoinWallet: spend amount must be positive.", this);
            return false;
        }

        if (coins < amount)
            return false;

        coins -= amount;
        UpdateCoinsText();
        return true;
    }

    private void UpdateCoinsText()
    {
        if (coinsText != null)
            coinsText.text = "Coins: " + coins;
    }
}
