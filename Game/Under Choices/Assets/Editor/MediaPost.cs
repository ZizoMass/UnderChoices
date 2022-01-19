using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Media Post", menuName = "Assets/New Media Post")]
public class MediaPost : ScriptableObject
{
    public enum Subject { Government, Violence, Health, Radicalism }
    public enum Reaction { Happy, Sad, Angry }

    public int postNumber, day, baseEngagement, boostedEngagement;
    public string publisher, headline, imageFilePath;
    public Subject subject;
    public Reaction reaction;
    public List<string> hashtags;
    public float boostCost;
}
