using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using System.IO;



public class VideoComponent : MonoBehaviour
{
    public UnityEngine.Video.VideoPlayer videoPlayer;
    private string status;
    public string videoName;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.url = Path.Combine(Application.streamingAssetsPath, videoName);
    }

    // Update is called once per frame
    void Update()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "myFile.mp4");
    }
}
