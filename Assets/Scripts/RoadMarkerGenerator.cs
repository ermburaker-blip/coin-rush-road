using UnityEngine;

public class RoadMarkerGenerator : MonoBehaviour
{
    [SerializeField] private GameObject roadMarkerPrefab;

    private void Start()
    {
        if (transform.childCount > 0)
            return;

        if (roadMarkerPrefab == null)
        {
            Debug.LogError("RoadMarkerGenerator: roadMarkerPrefab is not assigned.", this);
            return;
        }

        for (int x = -5; x <= 3; x += 2)
        {
            for (int y = -5; y <= 5; y += 2)
            {
                Instantiate(roadMarkerPrefab, new Vector3(x, y, 0f), Quaternion.identity, transform);
            }
        }
    }
}
