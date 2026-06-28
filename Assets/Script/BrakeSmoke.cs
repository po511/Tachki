using UnityEngine;

public class BrakeSmoke : MonoBehaviour
{
    public ParticleSystem smokePS;

    private SimpleCarController carController;

    void Start()
    {
        carController = GetComponent<SimpleCarController>();
    }

    void Update()
    {
        if (smokePS == null || carController == null) return;

        float minSpeed = carController.MaxSpeed * 0.5f;
        bool brakingForward = carController.gasInput < 0f && carController.currentSpeed > minSpeed;
        bool brakingReverse = carController.gasInput > 0f && carController.currentSpeed < -minSpeed;

        var emission = smokePS.emission;
        emission.enabled = brakingForward || brakingReverse;
    }
}
