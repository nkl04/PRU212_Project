using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class HorizontalCardHolder : MonoBehaviour
{
    public List<Card> CardList => cardList;
    [SerializeField] private Card selectedCard;
    [SerializeReference] private Card hoveredCard;
    [SerializeField] private GameObject slotCard;
    [SerializeField] private List<Card> cardList;
    private RectTransform rectTransform;

    [Header("Spawn Data")]
    [SerializeField] private int cardsToSpawn;

    [SerializeField] private bool tweenCardReturn;

    private void Start()
    {
        for (int i = 0; i < cardsToSpawn; i++)
        {
            Instantiate(slotCard, transform);
        }

        rectTransform = GetComponent<RectTransform>();

        cardList = GetComponentsInChildren<Card>().ToList();

        int cardCount = 0;

        foreach (Card card in cardList)
        {
            card.BeginDragEvent.AddListener(CardBeginDrag);

            card.EndDragEvent.AddListener(CardEndDrag);

            card.PointerEnterEvent.AddListener(CardPointerEnter);

            card.PointerExitEvent.AddListener(CarPointerExit);

            card.name = cardCount.ToString();

            cardCount++;
        }

        StartCoroutine(Frame());

        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);

            for (int i = 0; i < cardList.Count; i++)
            {
                if (cardList[i].cardVisual != null)
                {
                    cardList[i].cardVisual.UpdateIndex();
                }
            }
        }

    }

    private void CarPointerExit(Card card)
    {
        hoveredCard = null;
    }

    private void CardPointerEnter(Card card)
    {
        hoveredCard = card;
    }

    private void CardEndDrag(Card card)
    {
        if (selectedCard == null) return;

        selectedCard.transform.DOLocalMove(selectedCard.isSelected ? new Vector3(0f, selectedCard.selectionOffset, 0f) : Vector3.zero,
                                            tweenCardReturn ? .15f : 0f)
                                            .SetEase(Ease.OutBack);

        rectTransform.sizeDelta += Vector2.right;

        rectTransform.sizeDelta -= Vector2.right;

        selectedCard = null;
    }

    private void CardBeginDrag(Card card)
    {
        selectedCard = card;
    }
}
