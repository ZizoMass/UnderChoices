using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    enum Screen { Title, Game, Load };

    // Game data
    [SerializeField] float startingFund, timerLength;
    float playerFunds, timer;
    int currentNight;
    Screen currentScreen;

    // Game objects
    GameObject playerFundsDisplay, currentNightDisplay, timerDisplay;
    Scene currentScene;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Check which scene to load
        CheckScene();
    }

    // Update is called once per frame
    void Update()
    {
        // Update timer when on the game screen
        if(currentScreen == Screen.Game)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;

            UpdateTimer();
        }
    }

    void LoadTitleScreen()
    {
        // Currently unused
    }

    void LoadGameScreen()
    {
        // If a new game just started, initialize parameters
        if (currentNight == 0)
        {
            currentNight = 1;
            playerFunds = startingFund;
        }

        timer = timerLength;
        currentScreen = Screen.Game;

        // Load game objects
        playerFundsDisplay = GameObject.FindGameObjectWithTag("Player Funds Display");
        currentNightDisplay = GameObject.FindGameObjectWithTag("Current Night Display");
        timerDisplay = GameObject.FindGameObjectWithTag("Timer Display");

        UpdatePlayerFunds();
        UpdateCurrentNight();
        UpdateTimer();
    }

    void LoadFileScreen()
    {
        // Currently unused
    }

    void UpdatePlayerFunds()
    {
        playerFundsDisplay.GetComponent<Text>().text = "R$" + playerFunds;
    }

    void UpdateCurrentNight()
    {
        currentNightDisplay.GetComponent<Text>().text = "NIGHT " + currentNight;
    }

    void UpdateTimer()
    {
        timerDisplay.GetComponent<Text>().text = TimeSpan.FromSeconds(timer).ToString(@"mm\:ss");
    }

    void CheckScene()
    {
        currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "Title Screen")
            LoadTitleScreen();
        else if (currentScene.name == "Game Screen")
            LoadGameScreen();
        else if (currentScene.name == "Load Screen")
            LoadFileScreen();
    }

    public void ScreenTransition(string screenName)
    {
        SceneManager.LoadScene(screenName);
        //currentScene = SceneManager.GetActiveScene();
        StartCoroutine(LoadDelay(screenName));
    }

    IEnumerator LoadDelay(string screenName)
    {
        yield return new WaitForSeconds(0.0001f);
        CheckScene();
    }
}
