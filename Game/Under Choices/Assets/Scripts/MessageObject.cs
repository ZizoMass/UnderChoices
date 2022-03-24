using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageObject : MonoBehaviour
{
    const string AlexandreReisProfilePath = "character_profile_1024_Alexandre-Reis_USE";
    const string BragaFilhaProfilePath = "character_profile_1024_Braga-Filha_USE";
    const string AugustoCarreiraProfilePath = "character_profile_1024_Augusto-Carreira_USE";
    const string PatriciaJequitindaProfilePath = "character_profile_1024_Patricia-Jequitinda_USE";
    const string McLeqkinProfilePath = "character_profile_1024_Mc-Leqkin_USE";

    public GameObject profile, nameDisplay, textDisplay;

    public void SetMessage(string text, TextMessage.Character character)
    {
        textDisplay.GetComponent<TextMeshProUGUI>().text = text;

        if (character == TextMessage.Character.Alexandre_Reis)
        {
            profile.GetComponent<Image>().sprite = Resources.Load<Sprite>("Character Profiles/" + AlexandreReisProfilePath);
            nameDisplay.GetComponent<TextMeshProUGUI>().text = "Alexandre Reis";
        }
        else if (character == TextMessage.Character.Braga_Filha)
        {
            profile.GetComponent<Image>().sprite = Resources.Load<Sprite>("Character Profiles/" + BragaFilhaProfilePath);
            nameDisplay.GetComponent<TextMeshProUGUI>().text = "Braga Filha";
        }
        else if (character == TextMessage.Character.Augusto_Carreira)
        {
            profile.GetComponent<Image>().sprite = Resources.Load<Sprite>("Character Profiles/" + AugustoCarreiraProfilePath);
            nameDisplay.GetComponent<TextMeshProUGUI>().text = "Augusto Carreira";
        }
        else if (character == TextMessage.Character.Patricia_Jequitinda)
        {
            profile.GetComponent<Image>().sprite = Resources.Load<Sprite>("Character Profiles/" + PatriciaJequitindaProfilePath);
            nameDisplay.GetComponent<TextMeshProUGUI>().text = "Patricia Jequitinda";
        }
        else
        {
            profile.GetComponent<Image>().sprite = Resources.Load<Sprite>("Character Profiles/" + McLeqkinProfilePath);
            nameDisplay.GetComponent<TextMeshProUGUI>().text = "Mc Leqkin";
        }

        //play message sfx everytime a message is recieved 
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/FOL/FOL_RecivingMessage_001");
    }
}
