using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    [Header("Настройки")]
    public float maxSpeed = 30f;
    public float acceleration = 12f;
    public float turnSpeed = 100f;
    public float brakeForce = 9f;
    [SerializeField] IsGroundCar IsGroundCar;

    [Header("Визуальные колёса")]
    public Transform wheelFL, wheelFR, wheelRL, wheelRR;
    
    [Header("Фейковая подвеска")]
    float baseSuspensionHeight = 0f; // Оставь 0, если кузов уже стоит правильно
    float suspensionSmoothness = 0.1f;
    float bumpReaction = 0.0f;
    float maxSuspensionOffset = 0.10f;
    public Transform carBodyMesh;
    
    private Rigidbody rb;
    public float currentSpeed = 0f;
    private float currentSteerAngle = 0f;
    
    private float gasInput = 0f;
    private float steerInput = 0f;
    
    // Сохраняем начальную позицию и поворот кузова!
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    public int playerIndex = 1; // 1 = WASD, 2 = Стрелочки (меняется в Unity!)

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // ВАЖНО: Запоминаем, где кузов стоял изначально!
        if (carBodyMesh != null)
        {
            initialLocalPosition = carBodyMesh.localPosition;
            initialLocalRotation = carBodyMesh.localRotation;
        }
    }

    void FixedUpdate()
    {
        // === НОВАЯ СИСТЕМА ВВОДА ===
        float gasInput = 0f;
        float steerInput = 0f;
        if (playerIndex == 2)
        {
            if (Input.GetKey(KeyCode.W)) gasInput = 1f;
            else if (Input.GetKey(KeyCode.S)) gasInput = -1f;
            if (Input.GetKey(KeyCode.A)) steerInput = -1f;
            else if (Input.GetKey(KeyCode.D)) steerInput = 1f;
        }
        else if (playerIndex == 1)
        {
            if (Input.GetKey(KeyCode.UpArrow)) gasInput = 1f;
            else if (Input.GetKey(KeyCode.DownArrow)) gasInput = -1f;
            if (Input.GetKey(KeyCode.LeftArrow)) steerInput = -1f;
            else if (Input.GetKey(KeyCode.RightArrow)) steerInput = 1f;
        }
        

        // Разгон и торможение
        if (gasInput > 0)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.fixedDeltaTime, maxSpeed);
        }
        else if (gasInput < 0)
        {
            currentSpeed = Mathf.Max(currentSpeed - brakeForce * Time.fixedDeltaTime, -maxSpeed / 2f);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, acceleration * 0.5f * Time.fixedDeltaTime);
        }

        // Применение скорости
        Vector3 targetVelocity = transform.forward * currentSpeed;
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;

        // Поворот
        if (Mathf.Abs(currentSpeed) > 0.5f)
        {
            float rawGrip = 1f - (Mathf.Abs(currentSpeed) / maxSpeed);
            float gripFactor = Mathf.Clamp(rawGrip, 0.3f, 1f); // Не ниже 0.3!

            float turnAmount = steerInput * turnSpeed * Time.fixedDeltaTime * gripFactor;
            if (currentSpeed < 0) turnAmount = -turnAmount;
            transform.Rotate(Vector3.up, turnAmount, Space.World);
        }

        // Анимация колёс
        RotateWheels(currentSpeed);
        SteerWheels(steerInput);
    }

    void Update()
    {
        if (carBodyMesh != null)
        {
            // 1. БАЗОВАЯ ПОЗИЦИЯ = начальная позиция + небольшое смещение вверх
            Vector3 targetLocalPos = initialLocalPosition + Vector3.up * baseSuspensionHeight;

            // 2. Реакция на вертикальную скорость (с ограничением!)
            float verticalOffset = -rb.linearVelocity.y * bumpReaction;
            verticalOffset = Mathf.Clamp(verticalOffset, -maxSuspensionOffset, maxSuspensionOffset);
            targetLocalPos.y += verticalOffset;

            // 3. Плавное применение позиции
            carBodyMesh.localPosition = Vector3.Lerp(
                carBodyMesh.localPosition, 
                targetLocalPos, 
                Time.deltaTime * suspensionSmoothness
            );

            // 4. Крен и клёв (ОТНОСИТЕЛЬНО начального поворота!)
            float targetRoll = -steerInput * 3f * (Mathf.Abs(currentSpeed) / maxSpeed); // Уменьшил до 3
            float targetPitch = 0f;
            
            if (gasInput > 0) targetPitch = -2f * (currentSpeed / maxSpeed); // Уменьшил до 2
            else if (gasInput < 0) targetPitch = 2f; // Уменьшил до 2

            // Создаём поворот ОТНОСИТЕЛЬНО начального положения!
            Quaternion targetRotation = initialLocalRotation * Quaternion.Euler(targetPitch, 0, targetRoll);
            
            carBodyMesh.localRotation = Quaternion.Lerp(
                carBodyMesh.localRotation, 
                targetRotation, 
                Time.deltaTime * 5f
            );
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