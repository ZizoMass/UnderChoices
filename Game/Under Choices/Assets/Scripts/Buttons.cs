using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        FindObjectOfType<GameController>().ScreenTransition("Game Screen");
    }

    public void LoadGame()
    {
        FindObjectOfType<GameController>().ScreenTransition("Load Screen");
    }

    public void HowToPlay()
    {
        FindObjectOfType<GameController>().ScreenTransition("How to Play Screen");
    }

    public void BackToTitle()
    {
        FindObjectOfType<GameController>().ScreenTransition("Title Screen");
    }

    public void Boost()
    {
        FindObjectOfType<GameController>().BoostPost(transform.parent.GetComponent<PostObject>());
    }
}
