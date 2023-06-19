using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragButton : MonoBehaviour
{

    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void playDragSound()
    {
        // plays the drag sound only if its not playing
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
