using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Deck Info", menuName = "New Deck")]
public class DeckInfo : ScriptableObject
{
    public List<CardInfo> cardInfos;
}

