using UnityEngine;

public class LapCounter : MonoBehaviour
{
    [Header("Настройки гонки")]
    public int totalLapsToWin = 1; // Сколько кругов нужно для победы
    public int totalCheckpointsOnTrack = 7; // Сколько чекпоинтов на трассе (не считая финиш)

    [Header("Состояние гонки")]
    public int currentLap = 0;
    public int nextRequiredCheckpointID = 1;
    public Transform lastCheckpoint;
    private bool hasFinished = false;


    void OnTriggerEnter(Collider other)
    {
        if (hasFinished) return; // Если гонка окончена, игнорируем

        // 1. Если врезались в обычный чекпоинт
        if (other.CompareTag(ConsTextKey.TegCheckpoint))
        {
            Checkpoint cp = other.GetComponent<Checkpoint>();
            
            // Проверяем, тот ли это чекпоинт, который мы ждём
            if (cp.checkpointID == nextRequiredCheckpointID)
            {
                lastCheckpoint = cp.transform;
                nextRequiredCheckpointID++;
            }
            else
            {
                Debug.Log("Неверный чекпоинт! Вы срезали путь или едете не туда.");
            }
        }

        // 2. Если врезались в финишную черту
        else if (other.CompareTag(ConsTextKey.TegFinishLine))
        {
            // Круг засчитывается ТОЛЬКО если мы проехали ВСЕ чекпоинты
            if (nextRequiredCheckpointID > totalCheckpointsOnTrack)
            {
                currentLap++;
                Debug.Log("КРУГ " + currentLap + " ЗАВЕРШЁН!");
                
                // Сбрасываем счётчик чекпоинтов для следующего круга
                nextRequiredCheckpointID = 1;

                // Проверка на победу
                if (currentLap > totalLapsToWin)
                {
                    hasFinished = true;
                    Debug.Log("ИГРОК " + GetComponent<SimpleCarController>().playerIndex + " ПОБЕДИЛ!");
                    // Здесь потом можно добавить паузу игры и экран победы
                }
            }
            else
            {
                Debug.Log("Финиш не засчитан! Вы не прошли все чекпоинты или едете задом.");
            }
        }
    }
}