using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("FOV")]
    public Camera cam;
    public float baseFov = 60f;
    public float maxFov = 75f;
    public float fovSmoothness = 5f;

    [Header("Shake")]
    public float shakeIntensity = 0.05f;
    public float shakeSpeed = 10f;

    [Header("References")]
    public SimpleCarController carController;

    private Vector3 initialPos;
    private float noiseOffset;

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
        initialPos = transform.localPosition;
        noiseOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (carController == null) return;

        float speed = Mathf.Abs(carController.currentSpeed);
        float t = speed / carController.maxSpeed;

        float targetFov = Mathf.Lerp(baseFov, maxFov, t);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * fovSmoothness);

        float shake = t * shakeIntensity;
        float ox = (Mathf.PerlinNoise(noiseOffset, Time.time * shakeSpeed) * 2f - 1f) * shake;
        float oy = (Mathf.PerlinNoise(noiseOffset + 10f, Time.time * shakeSpeed) * 2f - 1f) * shake;

        transform.localPosition = initialPos + new Vector3(ox, oy, 0);
    }
}
