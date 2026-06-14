using UnityEngine;
using System.Collections.Generic;

public class SoundList : MonoBehaviour
{
    public List<AudioClip> clips;
    public AudioSource audioSource;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void Play(int index)
    {
        if (audioSource == null || index < 0 || index >= clips.Count) return;

        audioSource.clip = clips[index];
        audioSource.Play();
    }

    public void PlayRandom()
    {
        if (audioSource == null || clips.Count == 0) return;

        audioSource.clip = clips[Random.Range(0, clips.Count)];
        audioSource.Play();
    }

    public void Stop()
    {
        if (audioSource != null)
            audioSource.Stop();
    }
}
