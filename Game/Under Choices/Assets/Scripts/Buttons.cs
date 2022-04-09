using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public bool isBoostButton;
    public bool isEmailButton;

    //Reference to the Fmod master bus
    FMOD.Studio.Bus MasterBus;

    // Start is called before the first frame update
    void Start()
    {
        // If the player doesn't have enough money, grey out the boost button
        if (isBoostButton && !FindObjectOfType<GameController>().CanBeBoosted(transform.parent.GetComponent<PostObject>()))
            GetComponent<Button>().interactable = false;

        if (isEmailButton)
            StartCoroutine(ShowEmail());

        //Define the reference with the Master Bus from Fmod
        MasterBus = FMODUnity.RuntimeManager.GetBus("Bus:/");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewGame()
    {
        FindObjectOfType<GameController>().NewGame();
        FindObjectOfType<GameController>().ScreenTransition("Game Screen");
    }

    public void GoToIntro()
    {
        //Stop all Fmod audio events before change to the intro scene
        MasterBus.stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        SceneManager.LoadScene("IntroScene");
    }

    public void LoadGame()
    {
        FindObjectOfType<GameController>().ScreenTransition("Load Screen");
    }

    public void Credits()
    {
        FindObjectOfType<GameController>().ScreenTransition("Credits Screen");
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
        GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/media_post_button_Default_Button");
        GetComponent<Button>().interactable = false;
    }

    public void Refresh()
    {
        FindObjectOfType<GameController>().RefreshSet();

        if (FindObjectOfType<GameController>().GetComponent<GameController>().dayComplete)
        {
            GameObject.FindGameObjectWithTag("Day Complete Label").GetComponent<Text>().text = "No more posts";
            StartCoroutine(Disable());
        }
    }

    public void CompleteDay()
    {
        FindObjectOfType<GameController>().CheckOrders();
        FindObjectOfType<GameController>().CheckPosts();
        FindObjectOfType<GameController>().EndDay();
        StartCoroutine(Disable());
    }

    public void CloseEmail()
    {
        FindObjectOfType<GameController>().PlayNarrativeEvent();
        transform.parent.gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public IEnumerator Disable()
    {
        yield return new WaitForSeconds(0.0001f);
        gameObject.SetActive(false);
    }

    public IEnumerator ShowEmail()
    {
        yield return new WaitForSeconds(0.0001f);
        transform.parent.SetAsLastSibling();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isBoostButton && GetComponent<Button>().interactable == true)
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/media_post_button_OnHover");
            //Play the mouse over button sound
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/ButtonOver");
        }  
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isBoostButton && GetComponent<Button>().interactable == true)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/media_post_button_Default_Button");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isBoostButton && GetComponent<Button>().interactable == true)
        {
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/media_post_button_OnClick");
            //Play the click button sound
            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/UI/ButtonClick");
        }
    }
}
