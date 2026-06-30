using System.Reflection;
using Unity.VisualScripting;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UI;

public class Ui : MonoBehaviour
{
    [Header("Canvas")]
    public GameObject CanvasHud;
    public GameObject CanvasMenu;
    public GameObject CanvasCarDealership;
    public GameObject CanvasResults;

    [SerializeField] SimpleCarController SimpleCarController1;
    [SerializeField] SimpleCarController SimpleCarController2;
    [SerializeField] LapCounter LapCounter1;
    [SerializeField] LapCounter LapCounter2;
    [SerializeField] Respawn Respawn1;
    [SerializeField] Respawn Respawn2;
    int baseValueLapForwWin;


    [Header("Hud")]
    public Text LapText1Hud;
    public Text LapText2Hud;
    public Text CheckpointText1Hud;
    public Text CheckpointText2Hud;
    public Text CurrentTimeText1Hud;
    public Text CurrentTimeText2Hud;
    public Text BestTimeText1Hud;
    public Text BestTimeText2Hud;
    public Image Speedometr1;
    public Image Speedometr2;
    float SpeedometrfillAmount1;
    float SpeedometrfillAmount2;

    [Header("Результаты")]
    public Text BestTimeText1Results;
    public Text BestTimeText2Results;
    public Text LowTimeText1Results;
    public Text LowTimeText2Results;

    public Text St1TextResults;
    public Text St2TextResults;


    [Header("Настройки количества кругов")]
    public Slider LapSlider;
    public Text LapForWin;

    [Header("Автосалон")]

    [Header("1 car")]
    public Slider Speed1Slider;
    public Slider Acceleration1Slider;
    public Slider BrakeForce1Slider;

    public Text Speed1;
    public Text Acceleration1;
    public Text BrakeForce1;

    [Header("2 car")]
    public Slider Speed2Slider;
    public Slider Acceleration2Slider;
    public Slider BrakeForce2Slider;

    public Text Speed2;
    public Text Acceleration2;
    public Text BrakeForce2;

    void Awake()
    {
        AudioListener.pause = false;
        StartCanvas();
        if (LapSlider != null)
        {
            LapSlider.wholeNumbers = true;
            LapSlider.minValue = 1;
            LapSlider.value = 1;
        }
    }

    void Update()
    {
        PauseControllUpdate();
        CarsTextStatusUpdate();
        // Текст в результатах гонки про лучшее время круга и худшее время круга
        BestTimeText1Results.text = LapCounter1.bestLapTime.ToString(); 
        BestTimeText2Results.text = LapCounter2.bestLapTime.ToString();
        LowTimeText1Results.text = LapCounter1.lowTimeLap.ToString();
        LowTimeText2Results.text = LapCounter2.lowTimeLap.ToString();

        // 1 Спидометр
        SpeedometrfillAmount1 = Mathf.Abs(SimpleCarController1.currentSpeed) / SimpleCarController1.MaxSpeed;
        Speedometr1.fillAmount = SpeedometrfillAmount1;
        // 2 Спидометр
        SpeedometrfillAmount2 = Mathf.Abs(SimpleCarController2.currentSpeed) / SimpleCarController2.MaxSpeed;
        Speedometr2.fillAmount = SpeedometrfillAmount2;
        // Кругов проехали из всего нужного количества
        LapText1Hud.text = $"Laps {LapCounter1.currentLap}/{LapCounter1.totalLapsToWin}";
        LapText2Hud.text = $"Laps {LapCounter2.currentLap}/{LapCounter2.totalLapsToWin}";
        // Чекпоинтов проехали из всего нужного количества
        CheckpointText1Hud.text = $"Checkpoints {LapCounter1.nextRequiredCheckpointID - 1}/{LapCounter1.totalCheckpointsOnTrack}";
        CheckpointText2Hud.text = $"Checkpoints {LapCounter2.nextRequiredCheckpointID - 1}/{LapCounter2.totalCheckpointsOnTrack}";

        if (LapSlider != null)
        {
            LapCounter1.totalLapsToWin = (int)LapSlider.value;
            LapCounter2.totalLapsToWin = (int)LapSlider.value;
            if (LapForWin != null)
                LapForWin.text = $"Laps {(int)LapSlider.value}";
        }
        // Показатели лучшего времени и текущего времени круга
        CurrentTimeText1Hud.text = $"Time {FormatTime(LapCounter1.currentLapTime)}";
        CurrentTimeText2Hud.text = $"Time {FormatTime(LapCounter2.currentLapTime)}";
        BestTimeText1Hud.text = $"Best {FormatTime(LapCounter1.bestLapTime)}";
        BestTimeText2Hud.text = $"Best {FormatTime(LapCounter2.bestLapTime)}";
        
    }
    string FormatTime(float t)
    {
        if (float.IsInfinity(t) || t <= 0f) return "-:--.---";
        int minutes = Mathf.FloorToInt(t / 60f);
        float sec = t % 60f;
        return $"{minutes}:{sec:00.000}";
    }
    public void SaveCarsStatus()
    {
        SimpleCarController1.MaxSpeed = Speed1Slider.value;
        SimpleCarController1.Acceleration = Acceleration1Slider.value;
        SimpleCarController1.BrakeForce = BrakeForce1Slider.value;
        







        SimpleCarController2.Acceleration = Acceleration2Slider.value;
        SimpleCarController2.BrakeForce = BrakeForce2Slider.value;
        SimpleCarController2.MaxSpeed = Speed2Slider.value;
    }
    public void CarsTextStatusUpdate()
    {
        Speed1.text = $"Max speed {Speed1Slider.value.ToString()}";
        Acceleration1.text = $"Acceleration {Acceleration1Slider.value.ToString()}";
        BrakeForce1.text = $"BrakeForce {BrakeForce1Slider.value.ToString()}";

        Speed2.text = $"Max speed {Speed2Slider.value.ToString()}";
        Acceleration2.text =  $"Acceleration {Acceleration2Slider.value.ToString()}";
        BrakeForce2.text =  $"BrakeForce {BrakeForce2Slider.value.ToString()}";
    }
    public void Finish(int idPlayer)
    {
        AudioListener.pause = true;
        if(idPlayer == 1)
        {
            St1TextResults.text = "Second player";
            St2TextResults.text = "First player";
        }
        else
        {
            St1TextResults.text = "First player";
            St2TextResults.text = "Second player";
        }
        CanvasResults.SetActive(true);
        CanvasHud.SetActive(false);
        CanvasMenu.SetActive(false);
    }


    void PauseControllUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !CanvasResults.activeInHierarchy && !CanvasCarDealership.activeInHierarchy)
        {
            if (CanvasMenu.activeInHierarchy)
            {
                CanvasMenu.SetActive(false);
                CanvasHud.SetActive(true);
                Time.timeScale = 1;
                RestartSceneGame();
            }
            else
            {
                baseValueLapForwWin = LapCounter1.totalLapsToWin;
                CanvasMenu.SetActive(true);
                CanvasHud.SetActive(false);
                Time.timeScale = 0;
            }
        }
        // Звуки
        if(CanvasMenu.activeInHierarchy || CanvasResults.activeInHierarchy || CanvasCarDealership.activeInHierarchy)
        {
            AudioListener.pause = true; // Если активен какой то интерфейс кроме Hud тогда выключаем звуки
        }
        else
        {
            AudioListener.pause = false;
        }
    }
    void StartCanvas()
    {
        CanvasHud.SetActive(true);
        CanvasMenu.SetActive(false);
        CanvasResults.SetActive(false);
        CanvasCarDealership.SetActive(false);
    }
    public void RestartSceneGame()
    {
        if(!LapCounter1.hasFinished && !LapCounter2.hasFinished && baseValueLapForwWin == LapCounter1.totalLapsToWin) return;

        SimpleCarController1.ResetCar(Respawn1.startPos, Respawn1.startRot);
        SimpleCarController2.ResetCar(Respawn2.startPos, Respawn2.startRot);

        LapCounter1.currentLap = 1;
        LapCounter1.nextRequiredCheckpointID = 1;
        LapCounter1.currentLapTime = 0;
        LapCounter1.bestLapTime = 0;
        LapCounter1.lapStartTime = Time.time;
        LapCounter1.hasFinished = false;
        LapCounter1.lowTimeLap = 0;

        LapCounter2.currentLap = 1;
        LapCounter2.nextRequiredCheckpointID = 1;
        LapCounter2.currentLapTime = 0;
        LapCounter2.bestLapTime = 0;
        LapCounter2.lapStartTime = Time.time;
        LapCounter2.hasFinished = false;
        LapCounter2.lowTimeLap = 0;
    }
}
