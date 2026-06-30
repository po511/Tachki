using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    [Header("Настройки")]
    public float MaxSpeed = 30f;
    public float Acceleration = 12f;
    public float turnSpeed = 100f;
    public float BrakeForce = 9f;
    [SerializeField] IsGroundCar IsGroundCar;

    [Header("Физика")]
    public float centerOfMassOffset = -0.3f;

    [Header("Ватность (инерция)")]
    public float velocitySmooth = 8f;       // плавность скорости (меньше = ватнее)
    public float steerSmooth = 3f;          // плавность руля (меньше = ватнее)

    [Header("Дрифт")]
    public float maxDriftAngle = 30f;
    public float driftAngleSpeed = 3f;

    [Header("Визуальные колёса")]
    public Transform wheelFL, wheelFR, wheelRL, wheelRR;

    [Header("Фейковая подвеска")]
    float baseSuspensionHeight = 0f;
    float suspensionSmoothness = 0.1f;
    float bumpReaction = 0.0f;
    float maxSuspensionOffset = 0.10f;
    public Transform carBodyMesh;

    private Rigidbody rb;
    public float currentSpeed = 0f;
    private float currentSteerAngle = 0f;

    public float gasInput = 0f;
    private float rawSteerInput = 0f;
    private float smoothSteer = 0f;
    private float driftAngle = 0f;
    private float totalForwardRotation = 0f;


    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    public int PlayerIndex = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.centerOfMass = new Vector3(0, centerOfMassOffset, 0);

        if (carBodyMesh != null)
        {
            initialLocalPosition = carBodyMesh.localPosition;
            initialLocalRotation = carBodyMesh.localRotation;
        }
    }

    void FixedUpdate()
    {
        gasInput = 0f;
        rawSteerInput = 0f;

        // --- ввод 1P (стрелки) и 2P (WASD) ---
        if (PlayerIndex == 2)
        {
            if (Input.GetKey(KeyCode.W)) gasInput = 1f;
            else if (Input.GetKey(KeyCode.S)) gasInput = -1f;
            if (Input.GetKey(KeyCode.A)) rawSteerInput = -1f;
            else if (Input.GetKey(KeyCode.D)) rawSteerInput = 1f;
        }
        else if (PlayerIndex == 1)
        {
            if (Input.GetKey(KeyCode.UpArrow)) gasInput = 1f;
            else if (Input.GetKey(KeyCode.DownArrow)) gasInput = -1f;
            if (Input.GetKey(KeyCode.LeftArrow)) rawSteerInput = -1f;
            else if (Input.GetKey(KeyCode.RightArrow)) rawSteerInput = 1f;
        }

        // --- плавный руль (инерция поворота) ---
        smoothSteer = Mathf.Lerp(smoothSteer, rawSteerInput, Time.fixedDeltaTime * steerSmooth);

        // --- разгон / торможение ---
        if (gasInput > 0)
            currentSpeed = Mathf.Min(currentSpeed + Acceleration * Time.fixedDeltaTime, MaxSpeed);
        else if (gasInput < 0)
            currentSpeed = Mathf.Max(currentSpeed - BrakeForce * Time.fixedDeltaTime, -MaxSpeed / 2f);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, Acceleration * 0.5f * Time.fixedDeltaTime);

        // --- дрифт-угол ---
        float targetDriftAngle = 0f;

        if (Mathf.Abs(smoothSteer) > 0.1f && Mathf.Abs(currentSpeed) > 5f)
            targetDriftAngle = -smoothSteer * maxDriftAngle * 0.4f;

        driftAngle = Mathf.Lerp(driftAngle, targetDriftAngle, Time.fixedDeltaTime * driftAngleSpeed);

        // --- движение с инерцией ---
        Vector3 moveDir = Quaternion.AngleAxis(driftAngle, Vector3.up) * transform.forward;
        Vector3 targetVel = moveDir * currentSpeed;
        targetVel.y = rb.linearVelocity.y;

        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVel, Time.fixedDeltaTime * velocitySmooth);

        // --- поворот кузова ---
        if (Mathf.Abs(currentSpeed) > 0.5f)
        {
            float understeer = 1f - (Mathf.Abs(currentSpeed) / MaxSpeed) * 0.5f;
            understeer = Mathf.Clamp(understeer, 0.4f, 1f);

            float turn = smoothSteer * turnSpeed * Time.fixedDeltaTime * understeer;

            if (currentSpeed < 0) turn = -turn;
            transform.Rotate(Vector3.up, turn, Space.World);
        }
        UpdateAllWheels(currentSpeed, smoothSteer);
    }

    void Update()
    {
        if (carBodyMesh != null)
        {
            // подвеска (визуальная)
            Vector3 targetPos = initialLocalPosition + Vector3.up * baseSuspensionHeight;
            float offset = -rb.linearVelocity.y * bumpReaction;
            targetPos.y += Mathf.Clamp(offset, -maxSuspensionOffset, maxSuspensionOffset);
            carBodyMesh.localPosition = Vector3.Lerp(carBodyMesh.localPosition, targetPos, Time.deltaTime * suspensionSmoothness);

            // крен кузова в повороте
            float roll = -smoothSteer * 5f * (Mathf.Abs(currentSpeed) / MaxSpeed);

            // клёв при газе / торможении
            float pitch = 0f;
            if (gasInput > 0) pitch = -2f * (currentSpeed / MaxSpeed);
            else if (gasInput < 0) pitch = 2f;

            Quaternion targetRot = initialLocalRotation * Quaternion.Euler(pitch, 0, roll);
            carBodyMesh.localRotation = Quaternion.Lerp(carBodyMesh.localRotation, targetRot, Time.deltaTime * 5f);
        }
    }

    public void ResetCar(Vector3 position, Quaternion rotation)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        RigidbodyInterpolation prev = rb.interpolation;
        rb.interpolation = RigidbodyInterpolation.None;

        transform.SetPositionAndRotation(position, rotation);
        rb.position = position;
        rb.rotation = rotation;

        rb.interpolation = prev;
        currentSpeed = 0;
    }

    void UpdateAllWheels(float speed, float steerInput)
    {
        // Накопление угла вращения (абсолютный угол)
        totalForwardRotation += speed * Time.fixedDeltaTime * 100f;

        // Плавный руль
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, steerInput * 30f, Time.fixedDeltaTime * steerSmooth);

        // Задние колёса — только вращение вокруг своей оси X
        if (wheelRL) wheelRL.localEulerAngles = new Vector3(totalForwardRotation, 0, 0);
        if (wheelRR) wheelRR.localEulerAngles = new Vector3(totalForwardRotation, 0, 0);

        // Передние колёса — вращение + поворот руля вокруг оси Y
        if (wheelFL) wheelFL.localEulerAngles = new Vector3(totalForwardRotation, currentSteerAngle, 0);
        if (wheelFR) wheelFR.localEulerAngles = new Vector3(totalForwardRotation, currentSteerAngle, 0);
    }
}
