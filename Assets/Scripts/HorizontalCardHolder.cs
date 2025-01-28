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
    private bool isCrossing = false;
    private Deck deck;

    [Header("Spawn Data")]
    [SerializeField] private int cardListToSpawn;
    [SerializeField] private bool tweenCardReturn;

    private void Start()
    {
        deck = FindFirstObjectByType<Deck>();

        for (int i = 0; i < cardListToSpawn; i++)
        {
            Instantiate(slotCard, transform);
        }

        rectTransform = GetComponent<RectTransform>();

        cardList = GetComponentsInChildren<Card>().ToList();

        int cardCount = 0;

        // register card event 
        foreach (Card card in cardList)
        {
            card.BeginDragEvent.AddListener(CardBeginDrag);

            card.EndDragEvent.AddListener(CardEndDrag);

            card.PointerEnterEvent.AddListener(CardPointerEnter);

            card.PointerExitEvent.AddListener(CardPointerExit);

            card.name = cardCount.ToString();

            card.cardInfo = GetRandomCardInfoFromDeck(deck);

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (hoveredCard != null)
            {
                Destroy(hoveredCard.transform.parent.gameObject);
                cardList.Remove(hoveredCard);

            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in cardList)
            {
                card.Deselect();
            }
        }

        if (selectedCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < cardList.Count; i++)
        {

            if (selectedCard.transform.position.x > cardList[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() < cardList[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (selectedCard.transform.position.x < cardList[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() > cardList[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    private CardInfo GetRandomCardInfoFromDeck(Deck deck)
    {
        //get random card from deck then remove it from deck
        CardInfo cardInfo = deck.CardInfos[UnityEngine.Random.Range(0, deck.CardInfos.Count)];
        deck.RemoveCard(cardInfo);
        return cardInfo;
    }
    private void Swap(int index)
    {
        isCrossing = true;

        Transform focusedParent = selectedCard.transform.parent;

        Transform crossedParent = cardList[index].transform.parent;

        cardList[index].transform.SetParent(focusedParent);

        cardList[index].transform.localPosition = cardList[index].isSelected ? new Vector3(0, cardList[index].selectionOffset, 0) : Vector3.zero;

        selectedCard.transform.SetParent(crossedParent);

        isCrossing = false;

        if (cardList[index].cardVisual == null)
            return;

        bool swapIsRight = cardList[index].ParentIndex() > selectedCard.ParentIndex();

        cardList[index].cardVisual.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (Card card in cardList)
        {
            card.cardVisual.UpdateIndex();
        }
    }

    #region Registered card event
    private void CardPointerExit(Card card)
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
    #endregion
}
