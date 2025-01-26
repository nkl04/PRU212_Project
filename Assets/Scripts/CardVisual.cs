using UnityEngine;

public class CardVisual : MonoBehaviour
{
    private bool initalize = false;

    [Header("Card")]
    public Card parentCard;
    private Transform cardTransform;
    private Canvas canvas;

    [Header("References")]
    public Transform visualShadow;
    private Canvas shadowCanvas;

    private void Start()
    {

    }

    public void Initalize(Card card, int index = 0)
    {
        parentCard = card;
        cardTransform = card.transform;
        canvas = GetComponentInParent<Canvas>();
        shadowCanvas = visualShadow.GetComponent<Canvas>();

        parentCard.BeginDragEvent.AddListener(OnBeginDrag);
        parentCard.EndDragEvent.AddListener(OnEndDrag);
        parentCard.PointerEnterEvent.AddListener(OnPointerEnter);
        parentCard.PointerExitEvent.AddListener(OnPointerExit);
        parentCard.PointerUpEvent.AddListener(OnPointerUp);
        parentCard.PointerDownEvent.AddListener(OnPointerDown);
        parentCard.SelectEvent.AddListener(OnSelect);

        initalize = true;
    }

    public void UpdateIndex()
    {
        transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
    }

    private void OnBeginDrag(Card card)
    {
        // on begin drag
    }

    private void OnEndDrag(Card card)
    {
        // on end drag
    }

    private void OnPointerEnter(Card card)
    {
        // on pointer enter
    }

    private void OnPointerExit(Card card)
    {
        // on pointer exit
    }

    private void OnPointerUp(Card card, bool isOver)
    {
        // on pointer up
    }

    private void OnPointerDown(Card card)
    {
        // on pointer down
    }

    private void OnSelect(Card card, bool isSelected)
    {
        // on select
    }


}
