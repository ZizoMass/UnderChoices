using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Order", menuName = "Assets/New Boss Order")]
public class BossOrder : ScriptableObject
{
    public enum Type { Primary, Secondary }

    public int orderNumber, day, numberToBoost;
    public bool anySubject, anyReaction;
    public MediaPost.Subject subject;
    public MediaPost.Reaction reaction;
    public Type type;

    [HideInInspector] public int progress;
    [HideInInspector] public bool isComplete;

    override public string ToString()
    {
        string tempString = "- Boost " + numberToBoost;

        if (!anyReaction)
            tempString += " " + reaction.ToString();

        if (!anySubject)
            tempString += " " + subject.ToString();

        tempString += " post";

        if (numberToBoost > 1)
            tempString += "s";

        /*if (anySubject && anyReaction)
            tempString += " of your choice";*/

        tempString += " (" + progress + "/" + numberToBoost + ")";

        return tempString;
    }
}
