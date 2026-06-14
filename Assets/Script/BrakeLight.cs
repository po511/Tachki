using UnityEngine;

public class BrakeLight : MonoBehaviour
{
    public Material normalMaterial;
    public Material brakeMaterial;
    public SimpleCarController carController;
    private Renderer r;

    void Start()
    {
        r = GetComponent<Renderer>();
    }

    void Update()
    {
        if (carController == null || r == null) return;

        bool isBraking = carController.currentSpeed < -0.5f || carController.gasInput < 0f;
        r.material = isBraking ? brakeMaterial : normalMaterial;
    }
}
