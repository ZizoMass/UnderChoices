using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Narrative Event", menuName = "Assets/New Narrative Event")]
public class NarrativeEvent : ScriptableObject
{
    public int eventNumber, day;
    public string eventName, effect;
    public MediaPost.Subject subject;

    override public string ToString()
    {
        return "Play " + subject.ToString() + " Event: " + eventName + "\nEffect: " + effect;
    }
}
