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
    int currentNight, playerStrikes;
    int pointsGovernment, pointsViolence, pointsHealth, pointsRadicalism;
    Screen currentScreen;
    [HideInInspector] public bool dayComplete;

    // Game objects
    GameObject playerFundsDisplay, currentNightDisplay, timerDisplay, postSpawnPoint, postTemplate, screen, primaryOrders, secondaryOrders, strikesDisplay;
    Scene currentScene;
    List<MediaPost> mediaPosts, currentDayPosts, boostedPosts;
    List<BossOrder> bossOrders, currentOrders, completedOrders;
    List<NarrativeEvent> narrativeEvents;
    List<GameObject> postSpawnPointSet, currentPostSet;

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
        narrativeEvents = Resources.LoadAll<NarrativeEvent>("Narrative Events").ToList();
        postTemplate = Resources.Load("Prefabs/Media Post") as GameObject;
        currentDayPosts = new List<MediaPost>();
        boostedPosts = new List<MediaPost>();
        currentOrders = new List<BossOrder>();
        completedOrders = new List<BossOrder>();
        currentPostSet = new List<GameObject>();
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
        PlayNarrativeEvent();

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
        dayComplete = false;

        // Load game objects
        playerFundsDisplay = GameObject.FindGameObjectWithTag("Player Funds Display");
        currentNightDisplay = GameObject.FindGameObjectWithTag("Current Night Display");
        timerDisplay = GameObject.FindGameObjectWithTag("Timer Display");
        strikesDisplay = GameObject.FindGameObjectWithTag("Strikes Display");
        //postSpawnPoint = GameObject.FindGameObjectWithTag("Post Spawn Point");
        postSpawnPointSet = new List<GameObject>(GameObject.FindGameObjectsWithTag("Post Spawn Point"));
        screen = GameObject.FindGameObjectWithTag("Screen");
        primaryOrders = GameObject.FindGameObjectWithTag("Primary Orders Display");
        secondaryOrders = GameObject.FindGameObjectWithTag("Secondary Orders Display");

        /*int postCount = -1;

        // Load current day media posts
        foreach(MediaPost post in mediaPosts)
        {
            if (post.day == currentNight)
            {
                postCount++;
                GameObject newPost = Instantiate(postTemplate, new Vector2(postSpawnPoint.transform.position.x, postSpawnPoint.transform.position.y 
                    - postCount * 2.5f), Quaternion.identity);
                newPost.GetComponent<PostObject>().SetPost(post);
                newPost.transform.SetParent(screen.transform);

            }
        }*/

        // Load current day media posts
        foreach (MediaPost post in mediaPosts)
        {
            if (post.day == currentNight)
                currentDayPosts.Add(post);
        }

        // Load current day orders
        boostedPosts.Clear();
        currentOrders.Clear();
        foreach (BossOrder order in bossOrders)
        {
            if(order.day == currentNight)
                currentOrders.Add(Instantiate(order));
        }

        UpdatePlayerFunds();
        UpdateCurrentNight();
        UpdateTimer();
        UpdateOrders();
        UpdateStrikes();
        RefreshSet();
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

    void UpdateStrikes()
    {
        if (playerStrikes <= 0)
            return;

        strikesDisplay.GetComponent<Text>().text = "Strikes:";
        for (int i = 0; i < playerStrikes; i++)
            strikesDisplay.GetComponent<Text>().text += " X";
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

    void PlayNarrativeEvent()
    {
        // Find narrative subject with most points
        MediaPost.Subject dominantSubject;
        int[] pointsArray = new int[4];
        pointsArray[0] = pointsGovernment;
        pointsArray[1] = pointsViolence;
        pointsArray[2] = pointsHealth;
        pointsArray[3] = pointsRadicalism;

        int highestIndex = 0;
        for (int i = 1; i < pointsArray.Length; i++)
        {
            if (pointsArray[i] > pointsArray[highestIndex])
                highestIndex = i;
        }

        if (pointsArray[highestIndex] == 0)
            return;

        // Play correspoing narrative event
        if (highestIndex == 0)
            dominantSubject = MediaPost.Subject.Government;
        else if (highestIndex == 1)
            dominantSubject = MediaPost.Subject.Violence;
        else if (highestIndex == 2)
            dominantSubject = MediaPost.Subject.Health;
        else
            dominantSubject = MediaPost.Subject.Radicalism;

        foreach (NarrativeEvent narrativeEvent in narrativeEvents)
        {
            if (narrativeEvent.subject == dominantSubject && narrativeEvent.day == currentNight)
            {
                print(narrativeEvent.ToString());
                break;
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

    public void RefreshSet()
    {
        // Clear current set
        for(int j = currentPostSet.Count - 1; j >= 0; j--)
            Destroy(currentPostSet[j]);

        // Load new set
        for (int i = 0; i < postSpawnPointSet.Count; i++)
        {
            if (currentDayPosts.Count <= 0)
            {
                dayComplete = true;
                break;
            }

            GameObject newPost = Instantiate(postTemplate, postSpawnPointSet[i].transform.position, Quaternion.identity);
            newPost.GetComponent<PostObject>().SetPost(currentDayPosts[0]);
            currentDayPosts.RemoveAt(0);
            currentPostSet.Add(newPost);
            newPost.transform.SetParent(screen.transform);
        }
    }

    public void CheckPosts()
    {
        // Tally points in each narrative subject
        foreach(MediaPost post in boostedPosts)
        {
            if (post.subject == MediaPost.Subject.Government)
                pointsGovernment++;
            else if (post.subject == MediaPost.Subject.Violence)
                pointsViolence++;
            else if (post.subject == MediaPost.Subject.Health)
                pointsHealth++;
            else
                pointsRadicalism++;
        }
    }

    public void CheckOrders()
    {
        foreach(BossOrder order in currentOrders)
        {
            // Check for "don't boost" orders
            if (order.dontBoost && order.progress == 0)
                completedOrders.Add(order);

            // If there's an uncompleted order, give a strike
            if (!completedOrders.Contains(order))
            {
                playerStrikes++;
                break;
            }
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
