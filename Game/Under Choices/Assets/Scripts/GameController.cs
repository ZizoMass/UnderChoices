using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    GameObject playerFundsDisplay, currentNightDisplay, timerDisplay, postSpawnPoint, postTemplate, screen;
    Scene currentScene;
    List<MediaPost> mediaPosts, boostedPosts;
    List<GameObject> currentDayPosts;

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

        // Load media posts and post template
        mediaPosts = Resources.LoadAll<MediaPost>("Media Posts").ToList();
        postTemplate = Resources.Load("Prefabs/Media Post") as GameObject;
        boostedPosts = new List<MediaPost>();
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
        postSpawnPoint = GameObject.FindGameObjectWithTag("Post Spawn Point");
        screen = GameObject.FindGameObjectWithTag("Screen");

        int postCount = -1;

        // Load current day media posts
        foreach(MediaPost post in mediaPosts)
        {
            if (post.day == currentNight)
            {
                postCount++;
                GameObject newPost = Instantiate(postTemplate, new Vector2(postSpawnPoint.transform.position.x, postSpawnPoint.transform.position.y 
                    - postCount * 2.1f), Quaternion.identity);
                newPost.GetComponent<PostObject>().SetPost(post);
                newPost.transform.SetParent(screen.transform);

            }
        }

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

    public void BoostPost(PostObject post)
    {
        if(!post.isBoosted && playerFunds >= post.mediaPost.boostCost)
        {
            post.Boost();
            playerFunds -= post.mediaPost.boostCost;
            boostedPosts.Add(post.mediaPost);
            UpdatePlayerFunds();
        }
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
