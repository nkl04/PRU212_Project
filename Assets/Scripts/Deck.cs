using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Deck : MonoBehaviour
{
    public List<CardInfo> CardInfos => cardInfos;
    [SerializeField] private int cardAmount = 52;
    [SerializeField] private DeckInfo deckInfo;
    private List<CardInfo> cardInfos;
    private void Start()
    {
        InitializeCards();
    }

    private void InitializeCards()
    {
        cardInfos = new List<CardInfo>();
        for (int i = 0; i < cardAmount; i++)
        {
            cardInfos.Add(deckInfo.cardInfos[i]);
        }
    }

    public void RemoveCard(CardInfo cardInfo)
    {
        cardInfos.Remove(cardInfo);
    }
}


