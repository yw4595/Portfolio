using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileSound : MonoBehaviour
{
    AudioSource audioSource;
    AudioClip missileSFX;
    public float startTime;
    public float endTime;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        missileSFX = Resources.Load<AudioClip>("Sound/导弹发射音效");
        PlaySFX();
    }
    private void Update()
    {
        if (audioSource.time >= endTime) audioSource.Stop();
    }
    void PlaySFX()
    {
        if (missileSFX) audioSource.clip = missileSFX;
        audioSource.time = startTime;
        audioSource.Play();
    }
}
