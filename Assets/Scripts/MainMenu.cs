using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class MainMenu : MonoBehaviour
{

    public VideoPlayer introVideo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGameScene()
    {
        introVideo.frame = 0;
        introVideo.Play();
    }

    public void exitGame()
    {
        Application.Quit();
    }
}
