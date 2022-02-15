using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public bool isBoostButton;

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
        BackToTitle();
    }

    public IEnumerator Disable()
    {
        yield return new WaitForSeconds(0.0001f);
        gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isBoostButton)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/media_post_button_OnHover");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isBoostButton)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/media_post_button_Default_Button");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isBoostButton)
            GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/media_post_button_OnClick");
    }
}
