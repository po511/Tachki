using UnityEngine;

public class CamerSwitchPlace : MonoBehaviour
{
    [Header("Targets")]
    public Transform FirstPerson;
    public Transform ThirdPerson;

    [Header("Car")]
    public SimpleCarController Car;

    [Header("Smooth")]
    public float baseSmoothSpeed = 4f;
    public float maxFollowSpeed = 15f;
    public float smoothRotationSpeed = 5f;

    bool isThirdPerson = true;

    void Start()
    {
        transform.position = ThirdPerson.position;
        transform.rotation = ThirdPerson.rotation;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && Car.PlayerIndex == 1)
            isThirdPerson = !isThirdPerson;

        if (Input.GetKeyDown(KeyCode.C) && Car.PlayerIndex == 2)
            isThirdPerson = !isThirdPerson;
    }

    void LateUpdate()
    {
        if (isThirdPerson)
        {
            float t = Mathf.Abs(Car.currentSpeed) / Car.MaxSpeed;
            float smoothSpeed = Mathf.Lerp(baseSmoothSpeed, maxFollowSpeed, t);
            float smoothRotationSpeed = Mathf.Lerp(baseSmoothSpeed, maxFollowSpeed, t);

            transform.position = Vector3.Lerp(transform.position, ThirdPerson.position, smoothSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, ThirdPerson.rotation, smoothRotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = FirstPerson.position;
            transform.rotation = FirstPerson.rotation;
        }
    }
}
