using UnityEngine;
using System.Collections;

public class CarAudio : MonoBehaviour
{
    public SimpleCarController carController;
    public SoundList soundList;
    public AudioSource engineSource;
    public AudioSource fxSource;

    [Header("Engine")]
    public float minPitch = 0.5f;
    public float maxPitch = 2f;
    public float smoothSpeed = 5f;

    [Header("Backfire")]
    public float speedThreshold;
    public float cooldown = 0.3f;
    public int burstMax = 3;

    private float prevGasInput;
    private float cooldownTimer;

    void Start()
    {
        if (engineSource != null && soundList != null && soundList.clips.Count > 0)
        {
            engineSource.clip = soundList.clips[0];
            engineSource.loop = true;
            engineSource.Play();
        }
        speedThreshold = carController.MaxSpeed / 2; // порог скорости после которого торможение приносит выхлопной взрыв
    }

    void Update()
    {
        if (carController == null) return;

        EngineUpdate();
        BackfireUpdate();
    }

    void EngineUpdate()
    {
        if (engineSource == null) return;

        float t = Mathf.Abs(carController.currentSpeed) / carController.MaxSpeed;
        float target = Mathf.Lerp(minPitch, maxPitch, t);
        engineSource.pitch = Mathf.Lerp(engineSource.pitch, target, Time.deltaTime * smoothSpeed);
    }

    void BackfireUpdate()
    {
        if (fxSource == null || soundList == null || soundList.clips.Count < 2) return;

        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer <= 0f &&
            prevGasInput > 0.1f &&
            carController.gasInput <= 0f &&
            Mathf.Abs(carController.currentSpeed) > speedThreshold)
        {
            int burst = Random.Range(1, burstMax + 1);

            for (int i = 0; i < burst; i++)
                StartCoroutine(DelayedPop(i * Random.Range(0.05f, 0.15f)));

            cooldownTimer = cooldown;
        }

        prevGasInput = carController.gasInput;
    }

    IEnumerator DelayedPop(float delay)
    {
        yield return new WaitForSeconds(delay);

        int index = Random.Range(1, soundList.clips.Count);
        fxSource.clip = soundList.clips[index];
        fxSource.pitch = Random.Range(0.8f, 1.2f);
        fxSource.Play();
    }
}
