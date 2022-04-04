using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PostObject : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    const string GovernmentTemplatePath = "post_template_VERSION_03_0002_Government";
    const string HealthTemplatePath = "post_template_VERSION_03_0000_Health";
    const string RadicalismTemplatePath = "post_template_VERSION_03_0001_Radicalism";
    const string ViolenceTemplatePath = "post_template_VERSION_03_0003_Violence";

    const string AngryReactionPath = "emoji_reaction_angry_gradient";
    const string HappyReactionPath = "emoji_reaction_happy_gradient";
    const string SadReactionPath = "emoji_reaction_sad_gradient";

    [HideInInspector] public MediaPost mediaPost;
    public GameObject template, publisher, headline, image, reaction, engagement, boostCost;
    public List<GameObject> hashtags;
    public Animator animator;
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
        /*if(isScrolling)
            transform.position = new Vector2(transform.position.x, transform.position.y + .5f * Time.deltaTime);*/
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

        /*for(int i = 0; i < hashtags.Count && i < mediaPost.hashtags.Count; i++)
            hashtags[i].GetComponent<Text>().text = "#" + mediaPost.hashtags[i];*/

        hashtags[0].GetComponent<Text>().text = "";
        foreach (string tag in mediaPost.hashtags)
            hashtags[0].GetComponent<Text>().text += "#" + tag + " ";
    }

    public void Boost()
    {
        isBoosted = true;
        StartCoroutine(Increment());
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
        /*rectTransform.anchoredPosition += eventData.delta;
        isScrolling = false;*/
    }

    IEnumerator Increment()
    {
        int endValue = mediaPost.boostedEngagement;

        // 1/5 increment
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("Boost");
        engagement.GetComponent<Text>().text = ( (int) (endValue / 5) ).ToString();

        // 2/5 increment
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("Boost");
        engagement.GetComponent<Text>().text = ( (int) ( 2 * endValue / 5) ).ToString();

        // 3/5 increment
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("Boost");
        engagement.GetComponent<Text>().text = ( (int) ( 3 * endValue / 5) ).ToString();

        // 4/5 increment
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("Boost");
        engagement.GetComponent<Text>().text = ( (int) ( 4 * endValue / 5) ).ToString();

        // Display final value
        yield return new WaitForSeconds(.5f);
        animator.SetTrigger("Boost");
        engagement.GetComponent<Text>().text = endValue.ToString();
    }
}
