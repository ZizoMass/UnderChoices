using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    GameObject playerFundsDisplay, currentNightDisplay, timerDisplay, postSpawnPoint, postTemplate, screen, primaryOrders, secondaryOrders;
    Scene currentScene;
    List<MediaPost> mediaPosts, boostedPosts;
    List<BossOrder> bossOrders, currentOrders, completedOrders;
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
        bossOrders = Resources.LoadAll<BossOrder>("Boss Orders").ToList();
        postTemplate = Resources.Load("Prefabs/Media Post") as GameObject;
        boostedPosts = new List<MediaPost>();
        currentOrders = new List<BossOrder>();
        completedOrders = new List<BossOrder>();
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
        currentScreen = Screen.Title;
    }

    void LoadGameScreen()
    {
        // If a new game just started, initialize parameters
        /*if (currentNight == 0)
        {
            currentNight = 1;
            playerFunds = startingFund;
        }*/

        currentNight = 1;
        playerFunds = startingFund;
        timer = timerLength;
        currentScreen = Screen.Game;

        // Load game objects
        playerFundsDisplay = GameObject.FindGameObjectWithTag("Player Funds Display");
        currentNightDisplay = GameObject.FindGameObjectWithTag("Current Night Display");
        timerDisplay = GameObject.FindGameObjectWithTag("Timer Display");
        postSpawnPoint = GameObject.FindGameObjectWithTag("Post Spawn Point");
        screen = GameObject.FindGameObjectWithTag("Screen");
        primaryOrders = GameObject.FindGameObjectWithTag("Primary Orders Display");
        secondaryOrders = GameObject.FindGameObjectWithTag("Secondary Orders Display");

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

        // Load current day orders
        foreach(BossOrder order in bossOrders)
        {
            if(order.day == currentNight)
                currentOrders.Add(Instantiate(order));
        }

        UpdatePlayerFunds();
        UpdateCurrentNight();
        UpdateTimer();
        UpdateOrders();
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
        /*if (currentScreen != Screen.Game)
            return;

        if (timer <= 0)
            ScreenTransition("Title Screen");
        else
            timerDisplay.GetComponent<Text>().text = TimeSpan.FromSeconds(timer).ToString(@"mm\:ss");*/
    }

    void UpdateOrders()
    {
        primaryOrders.GetComponent<TextMeshProUGUI>().text = "";
        secondaryOrders.GetComponent<TextMeshProUGUI>().text = "";

        foreach (BossOrder order in currentOrders)
        {
            order.progress = 0;
            foreach (MediaPost post in boostedPosts)
            {
                bool doesMatch = true;

                if (!order.anySubject && order.subject != post.subject)
                    doesMatch = false;

                if (!order.anyReaction && order.reaction != post.reaction)
                    doesMatch = false;

                if (order.publisher != "None" && order.publisher != post.publisher)
                    doesMatch = false;

                if (doesMatch)
                    order.progress++;

                /*// If subject required, check if the subject matches
                if (order.anyReaction && post.subject == order.subject)
                    order.progress++;

                // If reaction required, check if the reaction matches
                else if (order.anySubject && post.reaction == order.reaction)
                    order.progress++;

                // If both required, check if both matches
                else if (post.subject == order.subject && post.reaction == order.reaction)
                    order.progress++;
                
                // If neither required, just increase
                else if (order.anySubject && order.anyReaction)
                    order.progress++;*/
            }

            if (order.type == BossOrder.Type.Primary)
                primaryOrders.GetComponent<TextMeshProUGUI>().text += order.ToString() + "\n";
            else if (order.type == BossOrder.Type.Secondary)
                secondaryOrders.GetComponent<TextMeshProUGUI>().text += order.ToString() + "\n";

            // Check if order is complete
            if (!order.dontBoost && !order.isComplete && order.progress >= order.numberToBoost)
            {
                order.isComplete = true;
                completedOrders.Add(order);
            }
        }
    }

    public void BoostPost(PostObject post)
    {
        if(!post.isBoosted && playerFunds >= post.mediaPost.boostCost)
        {
            post.Boost();
            playerFunds -= post.mediaPost.boostCost;
            boostedPosts.Add(post.mediaPost);
            UpdatePlayerFunds();
            UpdateOrders();
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
        StartCoroutine(LoadDelay(screenName));
    }

    IEnumerator LoadDelay(string screenName)
    {
        yield return new WaitForSeconds(0.0001f);
        CheckScene();
    }
}
