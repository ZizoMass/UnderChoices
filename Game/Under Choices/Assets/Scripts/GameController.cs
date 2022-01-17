using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }
    enum Screen { Title, Game };

    // Game data
    [SerializeField] float startingFund, timerLength;
    float playerFunds, timer;
    int currentNight;
    Screen currentScreen;

    // Game objects
    GameObject playerFundsDisplay, currentNightDisplay, timerDisplay;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadGameScreen();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentScreen == Screen.Game)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;

            UpdateTimer();
        }
    }

    void LoadGameScreen()
    {
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
        //timerDisplay.GetComponent<Text>().text = timer.ToString();
        timerDisplay.GetComponent<Text>().text = TimeSpan.FromSeconds(timer).ToString(@"mm\:ss");
    }
}
