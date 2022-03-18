using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class event_animation_manager : MonoBehaviour
{
    public enum Event { speech, hijacked, fired, sirens, riot, none };
    //Event: riot refers to the particle effect systems which will be activated
    // The events are in order of the heirarchy of the event_animation_manager in the scene
    public Event eventType;
    public Transform[] childEventsArray;
    private double videoCurrentTime;
    private double videoTotalTime;
    private bool isVideoPlaying;
    public float riotDuration;


    // Start is called before the first frame update
    void Start()
    {
        eventType = Event.none;
        childEventsArray = new Transform[5];
        riotDuration = 5f;
        EventsSetup();

    }

    // Update is called once per frame
    void Update()
    {
        deleteLaterTestingEventTrigger();
        EventHandleVideoPlaying();
        print("event:" + eventType);
        
    }

    /*  Event eventSelection (Event eventName)
      {
          if(eventName == Event.none)

          return eventName;
      }*/
    public void EventsSetup()
    {
        EventsAddChildToArray();
        for (int i = 0; i < childEventsArray.Length; i++)
        {
            childEventsArray[i].gameObject.SetActive(false);
        }

    }

    public void EventsAddChildToArray()
    {
        for (int i = 0; i < childEventsArray.Length; i++)
        {
            childEventsArray[i] = transform.GetChild(i);
        }

    }
    public void EventActivate(Event desiredEvent)
    {
        for (int i = 0; i < childEventsArray.Length; i++)
        {
            int desiredEventIndex = (int)desiredEvent;
            //Since an ENUM is just an integer with a name, and that the enum order follows
            //*continued* the order of child objects parented to the event_animations_manager, 
            //*continued* they will refer to the associated one without needing to search by string

            if (desiredEventIndex > childEventsArray.Length - 1)// Returns if enum is "none" so there isn't an out of bounds since there isn't a 6th child event
            {
                return;
            }
            else if (childEventsArray[i] == childEventsArray[desiredEventIndex] && (eventType == Event.fired || eventType == Event.hijacked || eventType == Event.speech || eventType == Event.sirens))
            {// Ensures that when the event is a video, that it grabs the length of the clip so it can end the video when finished
                childEventsArray[i].gameObject.SetActive(true);
                isVideoPlaying = true;
                videoTotalTime = childEventsArray[desiredEventIndex].gameObject.GetComponent<VideoPlayer>().clip.length;//checks if there's a vido clip associated with it to get the current frame so I know when the clip ends
            }
            else if (childEventsArray[i] == childEventsArray[desiredEventIndex] && eventType == Event.riot)//If its the Riot - the coroutine will run for a duration before turning itself off (the "riotDuration" float)
            {
               childEventsArray[i].gameObject.SetActive(true);
               StartCoroutine(EventPrimaryRiotParticleOff(riotDuration));
            }
            else if (childEventsArray[i] == childEventsArray[desiredEventIndex] && (eventType != Event.fired || eventType != Event.hijacked || eventType != Event.speech || eventType != Event.sirens))
            {
                childEventsArray[i].gameObject.SetActive(true);
            }
         
            else
            {
                childEventsArray[i].gameObject.SetActive(false);
            }
        }
    }
    void EventHandleVideoPlaying()//Checks per frame for when the video clip is over so it can turn itself off.
    {
        if (isVideoPlaying && eventType != Event.none)
        {
            int videoEventIndex = (int)eventType; // can use this as an index to manipulate the appropriate index
            videoCurrentTime = childEventsArray[videoEventIndex].gameObject.GetComponent<VideoPlayer>().time; //IF THERE IS AN ERROR: it likely happened when the enum eventType swaps from
            //*continued* a animation with a "Video Player" component to one like "Riot" where there isn't one. If you don't change the eventType before the animation finishes playing then there won't be an error.
            if (videoCurrentTime >= videoTotalTime-0.2)
            {
                childEventsArray[videoEventIndex].gameObject.SetActive(false);
                isVideoPlaying = false;
                eventType = Event.none;//This is critical - by it setting itself back to none, the game objects turn off and allows for selection of others
            }
        }
    }
    public IEnumerator EventPrimaryRiotParticleOff(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        int eventRiotIndex = (int)Event.riot;
        ParticleSystem[] childrenParticleSytems = childEventsArray[eventRiotIndex].gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem childParticle in childrenParticleSytems)
        {
            childParticle.Stop();
        }
        StartCoroutine(EventRiotParticleDisable());
    }
    private IEnumerator EventRiotParticleDisable()
    {
        yield return new WaitForSeconds(2.5f);
        int eventRiotIndex = (int)Event.riot;
        eventType = Event.none; // A bug persists where if you set it to Riot, then after Riot ends and game object is disabled, if you try to play another animation event, it bugs and doesn't auto turn off the animation. 
        //*continued* BUT if you wait ~>10 seconds after the Riot event ends, then decide to swap back to i.e. Speech, it will play with no issue. The bug behaviour is that the enum swaps back to None, the frame after selecting another event
        childEventsArray[eventRiotIndex].gameObject.SetActive(false);
    }

    void deleteLaterTestingEventTrigger()
    {
        if(eventType == Event.speech)
        {
            EventActivate(Event.speech);
        }
        else if(eventType == Event.hijacked)
        {
            EventActivate(Event.hijacked);
        }
        else if (eventType == Event.fired)
        {
            EventActivate(Event.fired);
        }
        else if (eventType == Event.sirens)
        {
            EventActivate(Event.sirens);
        }
        else if (eventType == Event.riot)
        {
            EventActivate(Event.riot);
        }
        else if (eventType == Event.none)
        {
            EventActivate(Event.none);
        }
    }
    
}
