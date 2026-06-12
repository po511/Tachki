using UnityEngine;

public class Spoiler : MonoBehaviour
{
    [Header("Настройки")]
    public int playerIndex = 1;       // 1 = WASD, 2 = Стрелочки (должно совпадать с машиной!)
    public float lerpSpeed = 8f;      // Скорость движения спойлера (чем больше, тем резче)

    // Используем float для плавности
    private float currentRotationX = -90f; 
    private float targetRotationX = -90f;  

    void Update()
    {
        // 1. Определяем целевой угол в зависимости от ввода
        if (playerIndex == 2) // Игрок 1: WASD
        {
            if (Input.GetKey(KeyCode.W)) 
                targetRotationX = -90f; // Газ: спойлер прижат (исходное положение)
            else if (Input.GetKey(KeyCode.S)) 
                targetRotationX = -60f; // Тормоз: спойлер поднят вверх (воздушный тормоз!)
            else 
                targetRotationX = -90f; // Если ничего не нажато, возвращаем в исходное
        }
        else if (playerIndex == 1) // Игрок 2: Стрелочки
        {
            if (Input.GetKey(KeyCode.UpArrow)) 
                targetRotationX = -90f; 
            else if (Input.GetKey(KeyCode.DownArrow)) 
                targetRotationX = -60f; 
            else 
                targetRotationX = -90f; 
        }

        // 2. Плавно приближаем текущий угол к целевому
        currentRotationX = Mathf.Lerp(currentRotationX, targetRotationX, Time.deltaTime * lerpSpeed);

        // 3. Применяем вращение
        // Quaternion.Euler(X, Y, Z) задаёт точный угол. 
        // Мы используем localRotation, чтобы спойлер крутился относительно своей точки крепления (Pivot), а не всей сцены.
        transform.localRotation = Quaternion.Euler(currentRotationX, 0f, 0f);
    }
}