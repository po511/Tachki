using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Ui : MonoBehaviour
{
    [SerializeField] SimpleCarController SimpleCarController1;
    [SerializeField] SimpleCarController SimpleCarController2;
    public Image Speedometr1;
    public Image Speedometr2;
    float fillAmount1;
    float fillAmount2;

    void Update()
    {
        fillAmount1 = SimpleCarController1.currentSpeed / SimpleCarController1.maxSpeed;
        Speedometr1.fillAmount = fillAmount1;

        fillAmount2 = SimpleCarController2.currentSpeed / SimpleCarController2.maxSpeed;
        Speedometr2.fillAmount = fillAmount2;
    }
}
