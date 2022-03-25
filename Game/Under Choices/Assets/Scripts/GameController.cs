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
    [SerializeField] int startDay, startGovernmentPoints = 1, startViolencePoints, startHealthPoints, startRadicalismPoints;
    [SerializeField] float startingFund, timerLength;
    float playerFunds, timer;
    int currentNight, playerStrikes;
    int pointsGovernment, pointsViolence, pointsHealth, pointsRadicalism;
    bool isNewGame = true;
    MediaPost.Subject dominantSubject;
    NarrativeEvent currentEvent;
    Screen currentScreen;
    [HideInInspector] public bool dayComplete;

    // Game objects
    GameObject playerFundsDisplay, currentNightDisplay, timerDisplay, postSpawnPoint, postTemplate, screen, primaryOrders, secondaryOrders, strikesDisplay, messageSpawn,
        messageTemplate;
    Scene currentScene;
    List<MediaPost> mediaPosts, currentDayPosts, boostedPosts;
    List<BossOrder> bossOrders, currentOrders, completedOrders;
    List<NarrativeEvent> narrativeEvents;
    List<TextMessage> textMessages, messagesTiedToRefresh;
    List<GameObject> postSpawnPointSet, currentPostSet, currentMessages;

    //Reference to the Fmod master bus
    FMOD.Studio.Bus MasterBus;

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
        textMessages = Resources.LoadAll<TextMessage>("Text Messages").ToList();
        postTemplate = Resources.Load("Prefabs/Media Post 2") as GameObject;
        messageTemplate = Resources.Load("Prefabs/Message") as GameObject;
        currentDayPosts = new List<MediaPost>();
        boostedPosts = new List<MediaPost>();
        currentOrders = new List<BossOrder>();
        completedOrders = new List<BossOrder>();
        currentPostSet = new List<GameObject>();
        currentMessages = new List<GameObject>();
        messagesTiedToRefresh = new List<TextMessage>();
    }

    // Start is called before the first frame update
    void Start()
    {
        // Check which scene to load
        CheckScene();

        //Define the reference with the Master Bus from Fmod
        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
    }

    // Update is called once per frame
    void Update()
    {
        // Update timer when on the game screen
        /*if(currentScreen == Screen.Game)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;

            UpdateTimer();
        }*/
    }

    void LoadInitialValues()
    {
        currentNight = startDay;
        pointsGovernment = startGovernmentPoints;
        pointsViolence = startViolencePoints;
        pointsHealth = startHealthPoints;
        pointsRadicalism = startRadicalismPoints;
        FindDominantSubject();
        isNewGame = false;
    }

    public void NewGame()
    {
        LoadInitialValues();
        playerStrikes = 0;
    }

    void LoadTitleScreen()
    {
        currentScreen = Screen.Title;
    }

    void LoadGameScreen()
    {

        // If a new game just started, initialize parameters
        if (isNewGame)
            LoadInitialValues();

        playerFunds = startingFund;
        currentScreen = Screen.Game;
        dayComplete = false;

        PlayNarrativeEvent();

        // Load game objects
        playerFundsDisplay = GameObject.FindGameObjectWithTag("Player Funds Display");
        currentNightDisplay = GameObject.FindGameObjectWithTag("Current Night Display");
        timerDisplay = GameObject.FindGameObjectWithTag("Timer Display");
        strikesDisplay = GameObject.FindGameObjectWithTag("Strikes Display");
        //postSpawnPoint = GameObject.FindGameObjectWithTag("Post Spawn Point");
        postSpawnPointSet = new List<GameObject>(GameObject.FindGameObjectsWithTag("Post Spawn Point"));
        messageSpawn = GameObject.FindGameObjectWithTag("Message Spawn");
        screen = GameObject.FindGameObjectWithTag("Screen");
        primaryOrders = GameObject.FindGameObjectWithTag("Primary Orders Display");
        secondaryOrders = GameObject.FindGameObjectWithTag("Secondary Orders Display");

        if (currentNight != 0)
            GameObject.FindGameObjectWithTag("Tutorial Email").SetActive(false);

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

        // Randomize posts
        int lastPos = currentDayPosts.Count - 1;
        for (int i = 0; i < lastPos; i++)
        {
            int random = UnityEngine.Random.Range(i, lastPos);
            MediaPost tempPost = currentDayPosts[i];
            currentDayPosts[i] = currentDayPosts[random];
            currentDayPosts[random] = tempPost;
        }

        // Load current day orders
        foreach (BossOrder order in bossOrders)
        {
            if (order.day != currentNight)
                continue;

            if (order.type == BossOrder.Type.Secondary && order.narrativeSubject != dominantSubject)
                continue;
               
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

    void FindDominantSubject()
    {
        // Find narrative subject with most points
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

        if (highestIndex == 0)
            dominantSubject = MediaPost.Subject.Government;
        else if (highestIndex == 1)
            dominantSubject = MediaPost.Subject.Violence;
        else if (highestIndex == 2)
            dominantSubject = MediaPost.Subject.Health;
        else if (highestIndex == 3)
            dominantSubject = MediaPost.Subject.Radicalism;
    }

    void PlayNarrativeEvent()
    {
        FindDominantSubject();

        // Play correspoing narrative event
        foreach (NarrativeEvent narrativeEvent in narrativeEvents)
        {
            if (narrativeEvent.subject == dominantSubject && narrativeEvent.day == currentNight)
            {
                currentEvent = narrativeEvent;
                UpdateMessages(narrativeEvent);
                PlayEvent();
                break;
            }
        }
    }

    void UpdateMessages(NarrativeEvent narrativeEvent)
    {
        List<TextMessage> messages = new List<TextMessage>();

        foreach(TextMessage message in textMessages)
        {
            if (message.correspondingEvent == narrativeEvent.eventNumber)
                messages.Add(message);
        }

        if(messages.Count > 0)
            StartCoroutine(LoadMessage(messages, 0));
    }

    public void BoostPost(PostObject post)
    {
        if(CanBeBoosted(post))
        {
            post.Boost();
            playerFunds -= post.mediaPost.boostCost;
            boostedPosts.Add(post.mediaPost);
            UpdatePlayerFunds();
            UpdateOrders();
        }
    }

    public Boolean CanBeBoosted(PostObject post)
    {
        if (!post.isBoosted && playerFunds >= post.mediaPost.boostCost)
            return true;
        else
            return false;
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

        // Receive phone message
        if(messagesTiedToRefresh.Count > 0)
            StartCoroutine(LoadMessage(messagesTiedToRefresh, 0));
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

            // If there's an incomplete primary order, give a strike
            if (order.type == BossOrder.Type.Primary && !completedOrders.Contains(order))
            {
                //playerStrikes++;
                break;
            }
        }
    }

    public void EndDay()
    {
        // Clear data
        boostedPosts.Clear();
        currentOrders.Clear();
        currentMessages.Clear();
        messagesTiedToRefresh.Clear();

        // Update night number
        currentNight++;

        StopAllCoroutines();

        if(currentNight > 8)
        {
            //Stop all Fmod audio events before return to the title scene
            MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            ScreenTransition("Title Screen");
            return;
        }

        if (playerStrikes < 3)
            ScreenTransition("Game Screen");
        else
            ScreenTransition("Game Over Screen");
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
        StartCoroutine(Transition(screenName));
    }

    IEnumerator Transition(string screenName)
    {
        GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>().SetTrigger("Fade In");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(screenName);
        StartCoroutine(LoadDelay(screenName));
        GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>().SetTrigger("Fade Out");
    }

    public void PlayEvent()
    {
        int eventNum;

        if (currentEvent.effect == "Speech")
            eventNum = 0;
        else if(currentEvent.effect == "Hijack")
            eventNum = 1;
        else if (currentEvent.effect == "Fired")
            eventNum = 2;
        else if (currentEvent.effect == "Siren")
            eventNum = 3;
        else if (currentEvent.effect == "Riot")
            eventNum = 4;
        else
            eventNum = 5;

        GameObject.FindObjectOfType<event_animation_manager>().eventType = (event_animation_manager.Event) eventNum;
    }

    IEnumerator LoadDelay(string screenName)
    {
        yield return new WaitForSeconds(0.0001f);
        CheckScene();
    }

    IEnumerator LoadMessage(List<TextMessage> _messages, int index)
    {
        yield return new WaitForSeconds(1f);

        // Move current messages down
        foreach (GameObject messageObject in currentMessages)
            messageObject.transform.position = new Vector2(messageObject.transform.position.x, messageObject.transform.position.y - 1.2f);

        // Load newest message
        GameObject newMessage = Instantiate(messageTemplate, messageSpawn.transform.position, Quaternion.identity);
        newMessage.GetComponent<MessageObject>().SetMessage(_messages[0].messages[index], _messages[0].character);
        currentMessages.Add(newMessage);
        newMessage.transform.SetParent(messageSpawn.transform);
        newMessage.transform.localPosition = new Vector2(0, 0);
        newMessage.transform.localScale = new Vector2(1, 1);

        // Remove offscreen messages
        /*if (currentMessages.Count > 3)
            currentMessages.RemoveAt(0);*/

        // Wait for next message
        yield return new WaitForSeconds(3);

        // If there are more texts in the current message, continue
        if (index < _messages[0].messages.Count - 1)
            StartCoroutine(LoadMessage(_messages, index + 1));

        // If there are no more texts in the current message, move on to the next message
        else if(_messages.Count > 1)
        {
            _messages.RemoveAt(0);
            messagesTiedToRefresh = _messages;
            //StartCoroutine(LoadMessage(_messages, 0));
        }
    }
}
