using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class IntroVideoManager : MonoBehaviour
{
    public VideoPlayer video;

    bool video_finished = false;

    void Start() { video.loopPointReached += CheckOver; }

    void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        print("Video Is Over");
        video_finished = true;
    }

    private void FixedUpdate()
    {
        if (video_finished && video != null)
        {
            if (video.targetCameraAlpha > 0.0)
            {
                // Slowly fade out the video
                video.targetCameraAlpha -= Time.deltaTime;
            }
            else
            {
                // Destroy the video object when video has faded out
                Destroy(video);
                video = null;
            }
        }
    }

}
