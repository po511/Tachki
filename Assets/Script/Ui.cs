using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class Ui : MonoBehaviour
{
    public GameObject CanvasHud;
    public GameObject CanvasMenu;
    [SerializeField] SimpleCarController SimpleCarController1;
    [SerializeField] SimpleCarController SimpleCarController2;
    [SerializeField] LapCounter LapCounter1;
    [SerializeField] LapCounter LapCounter2;
    public Image Speedometr1;
    public Image Speedometr2;
    public Text LapText1;
    public Text LapText2;
    public Text CheckpointText1;
    public Text CheckpointText2;
    float fillAmount1;
    float fillAmount2;
    public Slider speedSlider;
    bool isMenuOpen = false;
    public Text LapForWin;

    void Awake()
    {
        if (speedSlider != null)
            speedSlider.wholeNumbers = true;
        CloseMenu();
    }

    void Update()
    {
        fillAmount1 = Mathf.Abs(SimpleCarController1.currentSpeed) / SimpleCarController1.maxSpeed;
        Speedometr1.fillAmount = fillAmount1;

        fillAmount2 = Mathf.Abs(SimpleCarController2.currentSpeed) / SimpleCarController2.maxSpeed;
        Speedometr2.fillAmount = fillAmount2;

        LapText1.text = $"Laps {LapCounter1.currentLap - 1}/{LapCounter1.totalLapsToWin}";
        LapText2.text = $"Laps {LapCounter2.currentLap - 1}/{LapCounter2.totalLapsToWin}";

        CheckpointText1.text = $"Checkpoints {LapCounter1.nextRequiredCheckpointID - 1}/{LapCounter1.totalCheckpointsOnTrack}";
        CheckpointText2.text = $"Checkpoints {LapCounter2.nextRequiredCheckpointID - 1}/{LapCounter2.totalCheckpointsOnTrack}";

        if (speedSlider != null)
        {
            LapCounter1.totalLapsToWin = (int)speedSlider.value;
            LapCounter2.totalLapsToWin = (int)speedSlider.value;
            if (LapForWin != null)
                LapForWin.text = $"Laps {(int)speedSlider.value}";
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOpen)
                CloseMenu();
            else
                OpenMenu();
        }
        if(isMenuOpen)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
    void OpenMenu()
    {
        isMenuOpen = true;
        CanvasHud.SetActive(false);
        CanvasMenu.SetActive(true);
    }
    void CloseMenu()
    {
        isMenuOpen = false;
        CanvasHud.SetActive(true);
        CanvasMenu.SetActive(false);
    }
}
