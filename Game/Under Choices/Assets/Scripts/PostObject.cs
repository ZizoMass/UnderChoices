using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PostObject : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    const string GovernmentTemplatePath = "post_template_government";
    const string HealthTemplatePath = "post_template_health";
    const string RadicalismTemplatePath = "post_template_radicalism";
    const string ViolenceTemplatePath = "post_template_violence";

    const string AngryReactionPath = "emoji_reaction_angry";
    const string HappyReactionPath = "emoji_reaction_happy";
    const string SadReactionPath = "emoji_reaction_sad";

    [HideInInspector] public MediaPost mediaPost;
    public GameObject template, publisher, headline, image, reaction, engagement, boostCost;
    public List<GameObject> hashtags;
    [HideInInspector] public bool isBoosted;

    RectTransform rectTransform;
    bool isScrolling = true;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if(isScrolling)
            transform.position = new Vector2(transform.position.x, transform.position.y + .5f * Time.deltaTime);
    }

    public void SetPost(MediaPost _mediaPost)
    {
        mediaPost = _mediaPost;

        if (mediaPost.subject == MediaPost.Subject.Government)
            template.GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/" + GovernmentTemplatePath);
        else if (mediaPost.subject == MediaPost.Subject.Health)
            template.GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/" + HealthTemplatePath);
        else if (mediaPost.subject == MediaPost.Subject.Radicalism)
            template.GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/" + RadicalismTemplatePath);
        else
            template.GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/" + ViolenceTemplatePath);

        publisher.GetComponent<Text>().text = mediaPost.publisher;
        headline.GetComponent<Text>().text = mediaPost.headline;
        image.GetComponent<Image>().sprite = Resources.Load<Sprite>("Post Images/" + mediaPost.imageFilePath);
        engagement.GetComponent<Text>().text = mediaPost.baseEngagement.ToString();
        boostCost.GetComponent<Text>().text = "R$" + mediaPost.boostCost;

        if (mediaPost.reaction == MediaPost.Reaction.Angry)
            reaction.GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/" + AngryReactionPath);
        else if (mediaPost.reaction == MediaPost.Reaction.Happy)
            reaction.GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/" + HappyReactionPath);
        else
            reaction.GetComponent<Image>().sprite = Resources.Load<Sprite>("Media Post Assets/" + SadReactionPath);

        for(int i = 0; i < hashtags.Count && i < mediaPost.hashtags.Count; i++)
            hashtags[i].GetComponent<Text>().text = mediaPost.hashtags[i];
    }

    public void Boost()
    {
        isBoosted = true;
        engagement.GetComponent<Text>().text = mediaPost.boostedEngagement.ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
        isScrolling = false;
    }
}
