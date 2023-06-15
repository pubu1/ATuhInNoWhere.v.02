using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SoundManager : MonoBehaviour
{
    private AudioSource audioSource;
    private Slider volumeSlider;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        volumeSlider = GetComponentInChildren<Slider>();
    }

    public void ToggleSound()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.Play();
        }
    }

    public void SetVolume(float value)
    {
        audioSource.volume = value;
    }
}
