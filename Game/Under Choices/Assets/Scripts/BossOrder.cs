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
}
