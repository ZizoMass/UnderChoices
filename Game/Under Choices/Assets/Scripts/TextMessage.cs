using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMessage : ScriptableObject
{
    public enum Character { Alexandre_Reis, Braga_Filha, Augusto_Carreira, Patricia_Jequitinda, Mc_Leqkin }

    public int messageNumber, correspondingEvent;
    public List<string> messages;
    public Character character;
}
