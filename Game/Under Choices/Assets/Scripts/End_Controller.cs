using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class End_Controller : MonoBehaviour
{
    public VideoClip myClip;
    VideoPlayer videoPlayer;

    //Reference to the Fmod files
    FMOD.Studio.EventInstance instance;
    FMOD.Studio.Bus MasterBus;

    //Scene to be loaded after the video finish
    public string SceneName;

    bool waitForFirstFrame;
    bool canSetSkipOnDrop;

    // Start is called before the first frame update
    void Start()
    {
        Application.runInBackground = true;
        videoPlayer = gameObject.AddComponent<VideoPlayer>();

        //Check if the video is finished
        videoPlayer.loopPointReached += LoadScene;

        //Define the reference with the Master Bus from Fmod
        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");

        //Stop all Fmod audio events before start the scene
        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        waitForFirstFrame = true;
        canSetSkipOnDrop = false;

        instance = FMODUnity.RuntimeManager.CreateInstance("event:/Video Sound/Violence End");

        LoadAndPlay();

    }

    public void LoadAndPlay()
    {
        //Start the video clip
        videoPlayer.clip = myClip;
        videoPlayer.Play();
        //play video sound;
        instance.start();
    }

    //If the video is finished, load a new scene
    void LoadScene(VideoPlayer vp)
    {
        //Stop all Fmod audio events before start the scene
        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        //load next scene
        SceneManager.LoadScene(SceneName);
    }

}
