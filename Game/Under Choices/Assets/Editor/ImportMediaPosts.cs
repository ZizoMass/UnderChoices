using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportMediaPosts
{
    static string CSVPath = "/Editor/Media Posts.csv";

    [MenuItem("Utilities/Import Media Posts")]
    public static void Import()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + CSVPath);
        bool firstLine = true;

        foreach(string s in allLines)
        {
            // Skip first line (table headers)
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            string[] splitData = s.Split(',');

            // Data integrity check
            if(splitData.Length != 13)
            {
                Debug.Log(s + " does not have 13 values");
                return;
            }

            // Read data
            MediaPost mediaPost = ScriptableObject.CreateInstance<MediaPost>();
            mediaPost.postNumber = int.Parse(splitData[0]);
            mediaPost.publisher = splitData[1];

            string tempString_1 = splitData[2];
            if (tempString_1 == "Government")
                mediaPost.subject = MediaPost.Subject.Government;
            else if (tempString_1 == "Violence")
                mediaPost.subject = MediaPost.Subject.Violence;
            else if (tempString_1 == "Health")
                mediaPost.subject = MediaPost.Subject.Health;
            else
                mediaPost.subject = MediaPost.Subject.Radicalism;

            mediaPost.day = int.Parse(splitData[3]);

            string tempString_2 = splitData[4];
            if (tempString_2 == "Happy")
                mediaPost.reaction = MediaPost.Reaction.Happy;
            else if (tempString_2 == "Sad")
                mediaPost.reaction = MediaPost.Reaction.Sad;
            else
                mediaPost.reaction = MediaPost.Reaction.Angry;

            mediaPost.hashtags = new List<string>();
            mediaPost.hashtags.Add(splitData[5]);
            mediaPost.hashtags.Add(splitData[6]);
            mediaPost.hashtags.Add(splitData[7]);
            mediaPost.baseEngagement = int.Parse(splitData[8]);
            mediaPost.boostedEngagement = int.Parse(splitData[9]);
            mediaPost.boostCost = float.Parse(splitData[10]);
            mediaPost.headline = splitData[11];
            mediaPost.imageFilePath = splitData[12];

            // Create media post object
            AssetDatabase.CreateAsset(mediaPost, $"Assets/Resources/Media Posts/{"Media Post #" + mediaPost.postNumber}.asset");
        }
    }
}
