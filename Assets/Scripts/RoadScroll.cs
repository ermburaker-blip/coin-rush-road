using UnityEngine;

public class RoadScroll : MonoBehaviour
{
    [SerializeField] private LevelProgress levelProgress;

    private const float LowerLimit = -6f;
    private const float LoopLength = 12f;
    private float previousDistance;

    private void Start()
    {
        if (levelProgress == null)
        {
            Debug.LogError("RoadScroll: levelProgress is not assigned.", this);
            enabled = false;
            return;
        }

        previousDistance = levelProgress.Distance;
    }

    private void LateUpdate()
    {
        float currentDistance = levelProgress.Distance;
        float travelled = currentDistance - previousDistance;
        previousDistance = currentDistance;

        if (travelled <= 0f)
            return;

        foreach (Transform marker in transform)
        {
            Vector3 position = marker.position;
            position.y -= travelled;
            if (position.y < LowerLimit)
                position.y += Mathf.Ceil((LowerLimit - position.y) / LoopLength) * LoopLength;
            marker.position = position;
        }
    }
}
