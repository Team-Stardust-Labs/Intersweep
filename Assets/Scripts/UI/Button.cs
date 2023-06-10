using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public AudioSource audioSource;

    public void playClickSound()
    {
        audioSource.Play();
    }

    public void playSliderSound()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

}
