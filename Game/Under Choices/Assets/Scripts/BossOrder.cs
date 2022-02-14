using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Order", menuName = "Assets/New Boss Order")]
public class BossOrder : ScriptableObject
{
    public enum Type { Primary, Secondary }

    public int orderNumber, day, numberToBoost;
    public bool anySubject, anyReaction, dontBoost;
    public string publisher;
    public MediaPost.Subject subject, narrativeSubject;
    public MediaPost.Reaction reaction;
    public Type type;

    [HideInInspector] public int progress;
    [HideInInspector] public bool isComplete;

    override public string ToString()
    {
        string tempString = "- ";

        if (!dontBoost)
            tempString += "Boost " + numberToBoost;
        else
            tempString += "Don't boost any";

        if (!anyReaction)
            tempString += " " + reaction.ToString();

        if (!anySubject)
            tempString += " " + subject.ToString();

        tempString += " post";

        if (numberToBoost > 1 || dontBoost)
            tempString += "s";

        if (publisher != "" && publisher != "None")
            tempString += " by " + publisher;

        /*if (anySubject && anyReaction)
            tempString += " of your choice";*/

        if (!dontBoost)
            tempString += " (" + progress + "/" + numberToBoost + ")";
        else if (dontBoost && progress > 0)
            tempString += " (failed)";


        return tempString;
    }
}
