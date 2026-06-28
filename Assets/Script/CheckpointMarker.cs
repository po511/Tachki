using UnityEngine;

public class CheckpointMarker : MonoBehaviour
{
    public GameObject markerPrefab;
    public float floatHeight = 0.1f;
    public float floatSpeed = 1f;
    public float rotateSpeed = 0f;
    public float activeScale = 1.5f;

    private Checkpoint checkpoint;
    private LapCounter[] lapCounters;
    private GameObject visual;
    private Vector3 startPos;
    private float baseScale;

    void Start()
    {
        checkpoint = GetComponent<Checkpoint>();
        lapCounters = FindObjectsByType<LapCounter>(FindObjectsSortMode.None);

        if (markerPrefab != null)
        {
            visual = Instantiate(markerPrefab, transform);
            visual.transform.localPosition = Vector3.zero;
            startPos = visual.transform.localPosition;
            baseScale = visual.transform.localScale.x;
        }
    }

    void Update()
    {
        if (visual == null) return;

        visual.transform.localPosition = startPos + Vector3.up * Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        if (rotateSpeed != 0f)
            visual.transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        bool isActive = false;
        foreach (var lap in lapCounters)
        {
            if (lap.nextRequiredCheckpointID == checkpoint.checkpointID)
            {
                isActive = true;
                break;
            }
        }

        float target = isActive ? baseScale * activeScale : baseScale;
        float s = Mathf.Lerp(visual.transform.localScale.x, target, Time.deltaTime * 5f);
        visual.transform.localScale = Vector3.one * s;
    }
}
