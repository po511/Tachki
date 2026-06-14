using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    [Header("Настройки")]
    public float maxSpeed = 30f;
    public float acceleration = 12f;
    public float turnSpeed = 100f;
    public float brakeForce = 9f;
    [SerializeField] IsGroundCar IsGroundCar;

    [Header("Физика")]
    public float centerOfMassOffset = -0.3f;

    [Header("Дрифт")]
    public float maxDriftAngle = 30f;       // макс. угол заноса (градусы)
    public float driftAngleSpeed = 3f;      // как быстро меняется угол
    public float handbrakeTurn = 1.5f;      // острота руля в дрифте
    public KeyCode handbrakeKey = KeyCode.Space;
    public KeyCode handbrakeKey2 = KeyCode.LeftShift;

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
    private float steerInput = 0f;
    private bool isHandbraking = false;
    private float driftAngle = 0f;
    public bool IsDrifting => isHandbraking && Mathf.Abs(currentSpeed) > 5f;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    public int playerIndex = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
        steerInput = 0f;
        isHandbraking = false;

        // --- ввод 1P (стрелки + Space) и 2P (WASD + Shift) ---
        if (playerIndex == 2)
        {
            if (Input.GetKey(KeyCode.W)) gasInput = 1f;
            else if (Input.GetKey(KeyCode.S)) gasInput = -1f;
            if (Input.GetKey(KeyCode.A)) steerInput = -1f;
            else if (Input.GetKey(KeyCode.D)) steerInput = 1f;
            if (Input.GetKey(handbrakeKey2)) isHandbraking = true;
        }
        else if (playerIndex == 1)
        {
            if (Input.GetKey(KeyCode.UpArrow)) gasInput = 1f;
            else if (Input.GetKey(KeyCode.DownArrow)) gasInput = -1f;
            if (Input.GetKey(KeyCode.LeftArrow)) steerInput = -1f;
            else if (Input.GetKey(KeyCode.RightArrow)) steerInput = 1f;
            if (Input.GetKey(handbrakeKey)) isHandbraking = true;
        }

        // --- разгон / торможение ---
        if (gasInput > 0)
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);
        else if (gasInput < 0)
            currentSpeed = Mathf.Max(currentSpeed - brakeForce * Time.fixedDeltaTime, -maxSpeed / 2f);
        else
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * 0.5f * Time.fixedDeltaTime);

        // --- дрифт-угол: кузов смотрит в одну сторону, а машина едет под углом ---
        float targetDriftAngle = 0f;

        if (isHandbraking && Mathf.Abs(currentSpeed) > 3f)
            targetDriftAngle = -steerInput * maxDriftAngle;                // ручник: сильный занос
        else if (Mathf.Abs(steerInput) > 0.1f && Mathf.Abs(currentSpeed) > 5f)
            targetDriftAngle = -steerInput * maxDriftAngle * 0.4f;         // без ручника: лёгкий снос

        driftAngle = Mathf.Lerp(driftAngle, targetDriftAngle, Time.fixedDeltaTime * driftAngleSpeed);

        // машина движется в направлении, повёрнутом на driftAngle от кузова
        Vector3 moveDir = Quaternion.AngleAxis(driftAngle, Vector3.up) * transform.forward;
        Vector3 targetVel = moveDir * currentSpeed;
        targetVel.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVel;

        // --- поворот кузова ---
        if (Mathf.Abs(currentSpeed) > 0.5f)
        {
            // на скорости поворот слабее (надо притормаживать)
            float understeer = 1f - (Mathf.Abs(currentSpeed) / maxSpeed) * 0.5f;
            understeer = Mathf.Clamp(understeer, 0.4f, 1f);

            float turn = steerInput * turnSpeed * Time.fixedDeltaTime * understeer;
            if (IsDrifting) turn *= handbrakeTurn; // в дрифте руль острее

            if (currentSpeed < 0) turn = -turn;
            transform.Rotate(Vector3.up, turn, Space.World);
        }

        RotateWheels(currentSpeed);
        SteerWheels(steerInput);
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
            float roll = -steerInput * 3f * (Mathf.Abs(currentSpeed) / maxSpeed);
            if (IsDrifting) roll *= 2.5f; // в дрифте крен сильнее

            // клёв при газе / торможении
            float pitch = 0f;
            if (gasInput > 0) pitch = -2f * (currentSpeed / maxSpeed);
            else if (gasInput < 0) pitch = 2f;

            Quaternion targetRot = initialLocalRotation * Quaternion.Euler(pitch, 0, roll);
            carBodyMesh.localRotation = Quaternion.Lerp(carBodyMesh.localRotation, targetRot, Time.deltaTime * 5f);
        }
    }

    void RotateWheels(float speed)
    {
        float rotation = speed * Time.fixedDeltaTime * 100f;
        if (wheelFL) wheelFL.Rotate(rotation, 0, 0);
        if (wheelFR) wheelFR.Rotate(rotation, 0, 0);
        if (wheelRL) wheelRL.Rotate(rotation, 0, 0);
        if (wheelRR) wheelRR.Rotate(rotation, 0, 0);
    }

    void SteerWheels(float steer)
    {
        float targetAngle = steer * 30f;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetAngle, Time.deltaTime * 5f);
        if (wheelFL) wheelFL.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);
        if (wheelFR) wheelFR.localRotation = Quaternion.Euler(0, currentSteerAngle, 0);
    }
}