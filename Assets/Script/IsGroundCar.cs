using UnityEngine;

public class IsGroundCar : MonoBehaviour
{
    [SerializeField] int delay = 5;
    [SerializeField] int delayCurent;
    public Transform TransformCar;
    public bool restartPosition;
    public KeyCode flipKey = KeyCode.F;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(ConsTextKey.TegGround)) 
        {
            restartPosition = false;
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(ConsTextKey.TegGround)) 
        {
            restartPosition = true;
        }
    }

    void Update()
    {
        if (restartPosition)
        {
            if (Input.GetKeyDown(flipKey) && delayCurent <= 0)
            {
                delayCurent = delay;
                InvokeRepeating(nameof(DelayInvoke), 0, 1);
                TransformCar.position += new Vector3(0, 3, 0);
                TransformCar.transform.rotation = Quaternion.identity;
            }
        }
    }
    void DelayInvoke()
    {
        delayCurent -= 1;
        if(delayCurent <= 0)
        {
            CancelInvoke();
        }
    }
}