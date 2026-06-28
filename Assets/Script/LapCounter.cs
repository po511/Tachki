using System.Linq;
using UnityEngine;

public class LapCounter : MonoBehaviour
{
    public Ui Ui;
    [Header("Настройки гонки")]
    public int totalLapsToWin = 1;
    public int totalCheckpointsOnTrack = 7;

    [Header("Состояние гонки")]
    public int currentLap = 0;
    public int nextRequiredCheckpointID = 1;
    public Transform lastCheckpoint;
    private bool hasFinished = false;

    [Header("Время")]
    public float currentLapTime = 0f;
    public float bestLapTime;
    public float lastLapTime = 0f;
    private float lapStartTime = 0f;
    private bool lapStarted = false;
    public float lowTimeLap;

    void Start()
    {
        lapStartTime = Time.time;
    }

    void Update()
    {
        if (!hasFinished)
            currentLapTime = Time.time - lapStartTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasFinished) return;

        if (other.CompareTag(ConsTextKey.TegCheckpoint))
        {
            Checkpoint cp = other.GetComponent<Checkpoint>();

            if (cp.checkpointID == nextRequiredCheckpointID)
            {
                lastCheckpoint = cp.transform;
                nextRequiredCheckpointID++;
            }
        }

        else if (other.CompareTag(ConsTextKey.TegFinishLine))
        {
            if (nextRequiredCheckpointID > totalCheckpointsOnTrack)
            {
                    
                lastLapTime = Time.time - lapStartTime;
                if (lastLapTime < bestLapTime)
                    bestLapTime = lastLapTime;
                if(lastLapTime > lowTimeLap)
                    lowTimeLap = lastLapTime;
                currentLap++;
                lapStartTime = Time.time;
                currentLapTime = 0f;

                nextRequiredCheckpointID = 1;

                if (currentLap > totalLapsToWin)
                {
                    Time.timeScale = 0;
                    SimpleCarController CarFinish = GetComponent<SimpleCarController>();
                    hasFinished = true;
                    Ui.Finish(CarFinish.playerIndex);
                }
                    
            }
        }
    }
}