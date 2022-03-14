using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class event_animation_manager : MonoBehaviour
{
    public enum Event {speech, hijacked, fired, sirens, riot,none};
    //Event: riot refers to the particle effect systems which will be activated
    // The events are in order of the heirarchy of the event_animation_manager in the scene
    public Event eventType;
    public Transform[] childEventsArray;
   

    // Start is called before the first frame update
    void Start()
    {
        eventType = Event.none;
        childEventsArray = new Transform[5];
        EventsSetup();
 
    }

    // Update is called once per frame
    void Update()
    {
        deleteLaterTestingEventTrigger();
    }

  /*  Event eventSelection (Event eventName)
    {
        if(eventName == Event.none)

        return eventName;
    }*/
    public void EventsSetup()
    {
        EventsAddChildToArray();
        for (int i = 0; i<childEventsArray.Length;i++)
        {
            childEventsArray[i].gameObject.SetActive(false);
        }
       
    }

    public void EventsAddChildToArray()
    {
       for( int i = 0; i<childEventsArray.Length; i++)
        {
            childEventsArray[i] = transform.GetChild(i);
        }
        Debug.Log(childEventsArray.Length);
       
    }
    public void EventActivate(Event desiredEvent)
    {
        for (int i = 0; i < childEventsArray.Length; i++)
        {
            int desiredEventIndex = (int)desiredEvent;
            //Since an ENUM is just an integer with a name, and that the enum order follows
            //*continued* the order of child objects parented to the event_animations_manager, 
            //*continued* they will refer to the associated one without needing to search by string
            
            if(desiredEventIndex > childEventsArray.Length-1)// Returns if enum is "none" so there isn't an out of bounds since there isn't a 6th child event
            {
                return;
            }
            else if(childEventsArray[i] == childEventsArray[desiredEventIndex])
            {
                childEventsArray[i].gameObject.SetActive(true);
            }
            else
            {
                childEventsArray[i].gameObject.SetActive(false);
            }
        }
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
