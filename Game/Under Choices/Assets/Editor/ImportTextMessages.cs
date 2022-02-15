using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportTextMessages
{
    static string CSVPath = "/Editor/Text Messages.csv";
    static int NumberOfFields = 4;

    [MenuItem("Utilities/Import Text Messages")]
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
            TextMessage textMessage = ScriptableObject.CreateInstance<TextMessage>();
            textMessage.messageNumber = int.Parse(splitData[0]);
            textMessage.correspondingEvent = int.Parse(splitData[1]);

            // Character
            string tempString_1 = splitData[2];
            if (tempString_1 == "Alexandre Reis")
                textMessage.character = TextMessage.Character.Alexandre_Reis;
            else if (tempString_1 == "Braga Filha")
                textMessage.character = TextMessage.Character.Braga_Filha;
            else if (tempString_1 == "Augusto Carreira")
                textMessage.character = TextMessage.Character.Augusto_Carreira;
            else if (tempString_1 == "Patricia Jequitinda")
                textMessage.character = TextMessage.Character.Patricia_Jequitinda;
            else
                textMessage.character = TextMessage.Character.Mc_Leqkin;

            // Message
            string tempString_2 = splitData[3].Replace(";",",");
            string[] splitData_2 = tempString_2.Split('|');
            textMessage.messages = new List<string>();
            for (int i = 0; i < splitData_2.Length; i++)
                textMessage.messages.Add(splitData_2[i]);

            // Create media post object
            AssetDatabase.CreateAsset(textMessage, $"Assets/Resources/Text Messages/{"Text Message #" + textMessage.messageNumber}.asset");
        }
    }
}
