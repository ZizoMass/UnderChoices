using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportNarrativeEvents
{
    static string CSVPath = "/Editor/Narrative Events.csv";
    static int NumberOfFields = 5;

    [MenuItem("Utilities/Import Narrative Events")]
    public static void Import()
    {
        string[] allLines = File.ReadAllLines(Application.dataPath + CSVPath);
        bool firstLine = true;

        foreach (string s in allLines)
        {
            // Skip first line (table headers)
            if (firstLine)
            {
                firstLine = false;
                continue;
            }

            string[] splitData = s.Split(',');

            // Data integrity check
            if (splitData.Length != NumberOfFields)
            {
                Debug.Log(s + " does not have the correct number of values");
                return;
            }

            // Read data
            NarrativeEvent narrativeEvent = ScriptableObject.CreateInstance<NarrativeEvent>();
            narrativeEvent.eventNumber = int.Parse(splitData[0]);
            narrativeEvent.day = int.Parse(splitData[1]);

            // Subject
            string tempString_1 = splitData[2];
            if (tempString_1 == "Government")
                narrativeEvent.subject = MediaPost.Subject.Government;
            else if (tempString_1 == "Violence")
                narrativeEvent.subject = MediaPost.Subject.Violence;
            else if (tempString_1 == "Health")
                narrativeEvent.subject = MediaPost.Subject.Health;
            else
                narrativeEvent.subject = MediaPost.Subject.Radicalism;

            narrativeEvent.eventName = splitData[3];
            narrativeEvent.effect = splitData[4];

            // Create media post object
            AssetDatabase.CreateAsset(narrativeEvent, $"Assets/Resources/Narrative Events/{"Narrative Event #" + narrativeEvent.eventNumber}.asset");
        }
    }
}
