using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportBossOrders
{
    static string CSVPath = "/Editor/Boss Orders.csv";
    static int NumberOfFields = 6;

    [MenuItem("Utilities/Import Boss Orders")]
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
            BossOrder bossOrder = ScriptableObject.CreateInstance<BossOrder>();
            bossOrder.orderNumber = int.Parse(splitData[0]);
            bossOrder.day = int.Parse(splitData[1]);
            bossOrder.numberToBoost = int.Parse(splitData[2]);

            string tempString_1 = splitData[3];
            if (tempString_1 == "Government")
                bossOrder.subject = MediaPost.Subject.Government;
            else if (tempString_1 == "Violence")
                bossOrder.subject = MediaPost.Subject.Violence;
            else if (tempString_1 == "Health")
                bossOrder.subject = MediaPost.Subject.Health;
            else if (tempString_1 == "Radicalism")
                bossOrder.subject = MediaPost.Subject.Radicalism;
            else
                bossOrder.anySubject = true;

            string tempString_2 = splitData[4];
            if (tempString_2 == "Happy")
                bossOrder.reaction = MediaPost.Reaction.Happy;
            else if (tempString_2 == "Sad")
                bossOrder.reaction = MediaPost.Reaction.Sad;
            else if (tempString_2 == "Angry")
                bossOrder.reaction = MediaPost.Reaction.Angry;
            else
                bossOrder.anyReaction = true;

            string tempString_3 = splitData[5];
            if (tempString_3 == "Primary")
                bossOrder.type = BossOrder.Type.Primary;
            else
                bossOrder.type = BossOrder.Type.Secondary;

            // Create media post object
            AssetDatabase.CreateAsset(bossOrder, $"Assets/Resources/Boss Orders/{"Boss Order #" + bossOrder.orderNumber}.asset");
        }
    }
}
