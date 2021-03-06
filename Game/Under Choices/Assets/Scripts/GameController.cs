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

    // Game data
    [SerializeField] int startDay, startGovernmentPoints = 1, startViolencePoints, startHealthPoints, startRadicalismPoints;
    [SerializeField] float startingFund, timerLength;
    float playerFunds;
    int currentNight, currentPage, totalPages;
    int pointsGovernment, pointsViolence, pointsHealth, pointsRadicalism;
    bool isNewGame = true, isPlayingMessage;
    MediaPost.Subject dominantSubject;
    NarrativeEvent currentEvent;
    [HideInInspector] public bool dayComplete;

    // Game objects
    GameObject playerFundsDisplay, currentNightDisplay, timerDisplay, postSpawnPoint, postTemplate, screen, primaryOrders, secondaryOrders, strikesDisplay, messageSpawn,
        messageTemplate, radicalismEnding;
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
        // Check for singleton
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

    void LoadInitialValues()
    {
        // Initialize values for a new game
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
        // Begin a new game
        LoadInitialValues();
    }

    void LoadGameScreen()
    {

        // If a new game just started, initialize parameters
        if (isNewGame)
            LoadInitialValues();

        // Set start of day values
        playerFunds = startingFund;
        dayComplete = false;
        isPlayingMessage = false;

        // Play the narrative event/text messages, unless it's day 0
        if(currentNight != 0)
            PlayNarrativeEvent();

        // Load game objects
        playerFundsDisplay = GameObject.FindGameObjectWithTag("Player Funds Display");
        currentNightDisplay = GameObject.FindGameObjectWithTag("Current Night Display");
        timerDisplay = GameObject.FindGameObjectWithTag("Timer Display");
        strikesDisplay = GameObject.FindGameObjectWithTag("Strikes Display");
        postSpawnPointSet = new List<GameObject>(GameObject.FindGameObjectsWithTag("Post Spawn Point"));
        messageSpawn = GameObject.FindGameObjectWithTag("Message Spawn");
        screen = GameObject.FindGameObjectWithTag("Screen");
        primaryOrders = GameObject.FindGameObjectWithTag("Primary Orders Display");
        secondaryOrders = GameObject.FindGameObjectWithTag("Secondary Orders Display");
        radicalismEnding = GameObject.FindGameObjectWithTag("Radicalism Ending");

        // If it's not the radicalism ending event, hide the radicalism ending
        if (currentNight != 8 || dominantSubject != MediaPost.Subject.Radicalism)
            radicalismEnding.SetActive(false);

        // If it's not day 0, hide the tutorial email
        if (currentNight != 0)
            GameObject.FindGameObjectWithTag("Tutorial Email").SetActive(false);

        // Load current day media posts
        foreach (MediaPost post in mediaPosts)
        {
            if (currentNight == 8 && post.subject != dominantSubject)
                continue;

            if (post.day == currentNight)
                currentDayPosts.Add(post);
        }

        // Count the pages
        currentPage = 0;
        totalPages = (int) Mathf.Ceil(currentDayPosts.Count / 3);

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
        UpdateOrders();
        RefreshSet();
    }

    void UpdatePlayerFunds()
    {
        // Update player funds display label
        playerFundsDisplay.GetComponent<Text>().text = "R$" + playerFunds;
    }

    void UpdateOrders()
    {
        // Clear the sticky notes
        primaryOrders.GetComponent<TextMeshProUGUI>().text = "";
        secondaryOrders.GetComponent<TextMeshProUGUI>().text = "";

        // Go through each order in the current orders
        foreach (BossOrder order in currentOrders)
        {
            // Check the progress of the order
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
            }

            // Check if it's a primary or secondary order
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
        // Create fields for checking narrative subjects
        int[] pointsArray = new int[4];
        pointsArray[0] = pointsGovernment;
        pointsArray[1] = pointsViolence;
        pointsArray[2] = pointsHealth;
        pointsArray[3] = pointsRadicalism;

        // Find narrative subject with most points
        int highestIndex = 0;
        for (int i = 1; i < pointsArray.Length; i++)
        {
            if (pointsArray[i] > pointsArray[highestIndex])
                highestIndex = i;
        }

        // If the highest is 0 points, don't do anything
        if (pointsArray[highestIndex] == 0)
            return;

        // Decide the dominant subject based on results
        if (highestIndex == 0)
            dominantSubject = MediaPost.Subject.Government;
        else if (highestIndex == 1)
            dominantSubject = MediaPost.Subject.Violence;
        else if (highestIndex == 2)
            dominantSubject = MediaPost.Subject.Health;
        else if (highestIndex == 3)
            dominantSubject = MediaPost.Subject.Radicalism;
    }

    public void PlayNarrativeEvent()
    {
        // Check which narrative subject is dominant
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
        // Create new field for messages
        List<TextMessage> messages = new List<TextMessage>();

        // Check if each text message matches the current event, if so then load the message
        foreach(TextMessage message in textMessages)
        {
            if (message.correspondingEvent == narrativeEvent.eventNumber)
                messages.Add(message);
        }

        // If there are any messages, load and play them
        if(messages.Count > 0)
            StartCoroutine(LoadMessage(messages, 0));
    }

    public void BoostPost(PostObject post)
    {
        // If a post can be boosted, boost and update values
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
        // Check if the post can be boosted and if the player has enough money
        if (!post.isBoosted && playerFunds >= post.mediaPost.boostCost)
            return true;
        else
            return false;
    }

    public void RefreshSet()
    {
        // Clear current set
        for (int j = currentPostSet.Count - 1; j >= 0; j--)
            StartCoroutine(RemovePost(currentPostSet[j]));

        currentPostSet.Clear();

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
        if(messagesTiedToRefresh.Count > 0 && !isPlayingMessage)
        {
            isPlayingMessage = true;
            StartCoroutine(LoadMessage(messagesTiedToRefresh, 0));
        }

        // Update page counter
        currentPage++;
        GameObject.FindGameObjectWithTag("Day Complete Label").GetComponent<Text>().text = "PAGE " + currentPage + "/" + totalPages;
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
        // Check each order in the current orders
        foreach(BossOrder order in currentOrders)
        {
            // Check for "don't boost" orders
            if (order.dontBoost && order.progress == 0)
                completedOrders.Add(order);

            // If there's an incomplete primary order, do nothing
            if (order.type == BossOrder.Type.Primary && !completedOrders.Contains(order))
            {
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

        // If the game is over, play the corresponding ending
        if(currentNight > 8)
        {
            //Stop all Fmod audio events before return to the title scene
            MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

            SelectEnding();
            return;
        }

        // Double all subject points
        pointsGovernment *= 2;
        pointsViolence *= 2;
        pointsHealth *= 2;
        pointsRadicalism *= 2;

        ScreenTransition("Game Screen");
    }

    void SelectEnding()
    {
        // Play ending based on dominant subject
        if(dominantSubject == MediaPost.Subject.Government)
            ScreenTransition("Ending_Government");
        else if (dominantSubject == MediaPost.Subject.Violence)
            ScreenTransition("Ending_Violence");
        else if (dominantSubject == MediaPost.Subject.Health)
            ScreenTransition("Ending_Health");
        else if (dominantSubject == MediaPost.Subject.Radicalism)
            ScreenTransition("Ending_Radicalism");
    }

    void CheckScene()
    {
        // Check which scene the player is on
        currentScene = SceneManager.GetActiveScene();

        // If it's the game screen, then load the game screen
        if (currentScene.name == "Game Screen")
            LoadGameScreen();
    }

    public void ScreenTransition(string screenName)
    {
        StartCoroutine(Transition(screenName));
    }

    IEnumerator Transition(string screenName)
    {
        // Transition to a new screen with the black fade transition
        GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>().SetTrigger("Fade In");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(screenName);
        StartCoroutine(LoadDelay(screenName));
        GameObject.FindGameObjectWithTag("Transition").GetComponent<Animator>().SetTrigger("Fade Out");
    }

    public void PlayEvent()
    {
        // Check which event should be played
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

        // Play the corresponding event
        GameObject.FindObjectOfType<event_animation_manager>().eventType = (event_animation_manager.Event) eventNum;
    }

    IEnumerator RemovePost(GameObject post)
    {
        // Play slide out animation
        post.GetComponent<PostObject>().animator.SetTrigger("Slide Out");

        yield return new WaitForSeconds(1);

        // Remove post
        Destroy(post);
    }

    IEnumerator LoadDelay(string screenName)
    {
        // Wait a moment before checking the scene
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

        // Wait for next message
        yield return new WaitForSeconds(3);

        // If there are more texts in the current message, continue
        if (index < _messages[0].messages.Count - 1)
            StartCoroutine(LoadMessage(_messages, index + 1));

        // If there are no more texts in the current message, move on to the next message
        else if(_messages.Count > 0)
        {
            _messages.RemoveAt(0);
            messagesTiedToRefresh = _messages;
            isPlayingMessage = false;
        }
    }
}
