using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private TMP_Text distanceValueText;
    [SerializeField] private TMP_Text obstacleIntervalValueText;

    private const string RoadDistanceKey = "RoadDistance";
    private const int DefaultRoadDistance = 120;
    private const string ObstacleIntervalKey = "ObstacleSpawnInterval";
    private const float DefaultObstacleInterval = 1f;

    public static float GetObstacleInterval()
    {
        float seconds = PlayerPrefs.GetFloat(ObstacleIntervalKey, DefaultObstacleInterval);
        if (seconds == 2f || seconds == 1f || seconds == 0.5f)
            return seconds;

        Debug.LogWarning("MainMenuController: invalid saved obstacle interval; using 2 seconds.");
        return DefaultObstacleInterval;
    }

    private static string FormatObstacleInterval(float seconds)
    {
        return seconds.ToString("0.#", System.Globalization.CultureInfo.InvariantCulture) + " s";
    }

    public static int GetRoadDistance()
    {
        int meters = PlayerPrefs.GetInt(RoadDistanceKey, DefaultRoadDistance);
        if (meters == 120 || meters == 160 || meters == 200)
            return meters;

        Debug.LogWarning("MainMenuController: invalid saved road distance; using 120 m.");
        return DefaultRoadDistance;
    }

    public void OpenSettings()
    {
        if (settingsPanel == null || distanceValueText == null || obstacleIntervalValueText == null)
        {
            Debug.LogError("MainMenuController: assign settingsPanel, distanceValueText, and obstacleIntervalValueText.", this);
            return;
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        distanceValueText.text = GetRoadDistance() + " m";
        obstacleIntervalValueText.text = FormatObstacleInterval(GetObstacleInterval());
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        } 
        
        if (settingsPanel == null)
        {
            Debug.LogError("MainMenuController: settingsPanel is not assigned.", this);
            return;
        }

        settingsPanel.SetActive(false);
    }

    public void SetRoadDistance(int meters)
    {
        if (meters != 120 && meters != 160 && meters != 200)
        {
            Debug.LogWarning("MainMenuController: road distance must be 120, 160, or 200 m.", this);
            return;
        }

        if (distanceValueText == null)
        {
            Debug.LogError("MainMenuController: distanceValueText is not assigned.", this);
            return;
        }

        PlayerPrefs.SetInt(RoadDistanceKey, meters);
        PlayerPrefs.Save();
        distanceValueText.text = meters + " m";
    }

    public void SetObstacleInterval(float seconds)
    {
        if (seconds != 2f && seconds != 1f && seconds != 0.5f)
        {
            Debug.LogWarning("MainMenuController: obstacle interval must be 2, 1, or 0.5 seconds.", this);
            return;
        }

        if (obstacleIntervalValueText == null)
        {
            Debug.LogError("MainMenuController: obstacleIntervalValueText is not assigned.", this);
            return;
        }

        PlayerPrefs.SetFloat(ObstacleIntervalKey, seconds);
        PlayerPrefs.Save();
        obstacleIntervalValueText.text = FormatObstacleInterval(seconds);
    }

    public void PlayRace()
    {
        SceneManager.LoadScene("RoadPrototype");
    }
}