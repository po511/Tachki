using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("FOV")]
    public Camera cam;
    public float baseFov = 60f;
    public float maxFov = 75f;
    public float fovSmoothness = 5f;

    [Header("References")]
    public SimpleCarController carController;

    void Start()
    {
        if (cam == null) cam = GetComponent<Camera>();
    }

    void Update()
    {
        if (carController == null) return;

        float speed = Mathf.Abs(carController.currentSpeed);
        float t = speed / carController.MaxSpeed;

        float targetFov = Mathf.Lerp(baseFov, maxFov, t);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * fovSmoothness);
    }
}
