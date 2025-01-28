using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CardVisual : MonoBehaviour
{
    private bool initalize = false;

    [Header("Card")]
    public Card parentCard;
    private Transform cardTransform;
    private Canvas canvas;
    private int savedIndex;
    private Vector3 movementDelta;
    private Vector3 rotationDelta;

    [Header("References")]
    [SerializeField] private Transform visualShadow;
    [SerializeField] private Transform shakeParent;
    [SerializeField] private Transform tiltParent;
    [SerializeField] private Image cardImage;
    [SerializeField] private float shadowOffset = 20;
    private Canvas shadowCanvas;
    private Vector2 shadowDistance;

    [Header("Follow Parameters")]
    [SerializeField] private float followSpeed = 30;

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationAmount = 20;
    [SerializeField] private float rotationSpeed = 20;
    [SerializeField] private float autoTiltAmount = 30;
    [SerializeField] private float manualTiltAmount = 20;
    [SerializeField] private float tiltSpeed = 20;

    [Header("Scale Parameters")]
    [SerializeField] private bool scaleAnimations = true;
    [SerializeField] private float scaleOnHover = 1.15f;
    [SerializeField] private float scaleOnSelect = 1.25f;
    [SerializeField] private float scaleTransition = .15f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;

    [Header("Select Parameters")]
    [SerializeField] private float selectPunchAmount = 20;

    [Header("Hoer Parameters")]
    [SerializeField] private float hoverPunchAngle = 5;
    [SerializeField] private float hoverTransition = .15f;

    [Header("Swap Parameters")]
    [SerializeField] private bool swapAnimations = true;
    [SerializeField] private float swapRotationAngle = 30;
    [SerializeField] private float swapTransition = .15f;
    [SerializeField] private int swapVibrato = 5;

    [Header("Curve")]
    [SerializeField] private CurveParameters curve;
    private float curveRotationOffset;
    private float curveYOffset;

    private void Start()
    {
        shadowDistance = visualShadow.localPosition;
    }

    public void Initialize(Card card, int index = 0)
    {
        parentCard = card;
        cardTransform = card.transform;
        cardImage.sprite = card.cardInfo?.sprite;
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

    private void Update()
    {
        if (!initalize || parentCard == null) return;

        HandPositioning();

        SmoothFollow();

        FollowRotation();

        CardTilt();
    }

    private void HandPositioning()
    {
        curveYOffset = curve.positioning.Evaluate(parentCard.NormalizedPosition()) * curve.positioningInfluence * parentCard.SiblingAmount();

        curveYOffset = parentCard.SiblingAmount() < 5 ? 0 : curveYOffset;

        curveRotationOffset = curve.rotation.Evaluate(parentCard.NormalizedPosition());
    }

    private void SmoothFollow()
    {
        Vector3 verticalOffset = Vector3.up * (parentCard.isDragging ? 0 : curveYOffset);

        transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, followSpeed * Time.deltaTime);
    }

    private void FollowRotation()
    {
        Vector3 movement = transform.position - cardTransform.position;

        movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);

        Vector3 movementRotation = (parentCard.isDragging ? movementDelta : movement) * rotationAmount;

        rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.deltaTime);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
    }
    private void CardTilt()
    {
        savedIndex = parentCard.isDragging ? savedIndex : parentCard.ParentIndex();
        float sine = Mathf.Sin(Time.time + savedIndex) * (parentCard.isHovering ? .2f : 1);
        float cosine = Mathf.Cos(Time.time + savedIndex) * (parentCard.isHovering ? .2f : 1);

        Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float tiltX = parentCard.isHovering ? ((offset.y * -1) * manualTiltAmount) : 0;
        float tiltY = parentCard.isHovering ? ((offset.x) * manualTiltAmount) : 0;
        float tiltZ = parentCard.isDragging ? tiltParent.eulerAngles.z : (curveRotationOffset * (curve.rotationInfluence * parentCard.SiblingAmount()));

        float lerpX = Mathf.LerpAngle(tiltParent.eulerAngles.x, tiltX + (sine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpY = Mathf.LerpAngle(tiltParent.eulerAngles.y, tiltY + (cosine * autoTiltAmount), tiltSpeed * Time.deltaTime);
        float lerpZ = Mathf.LerpAngle(tiltParent.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.deltaTime);

        tiltParent.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
    }

    public void Swap(float dir = 1)
    {
        if (!swapAnimations)
            return;

        DOTween.Kill(2, true);

        shakeParent.DOPunchRotation(Vector3.forward * swapRotationAngle * dir, swapTransition, swapVibrato, 1).SetId(3);
    }

    public void UpdateIndex()
    {
        transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
    }

    private void OnBeginDrag(Card card)
    {
        // on begin drag
        if (scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

        canvas.overrideSorting = true; // enable put the card to the top of canvas
    }

    private void OnEndDrag(Card card)
    {
        // on end drag
        canvas.overrideSorting = false; // disable overide sorting => put the card to original soring   

        transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

    private void OnPointerEnter(Card card)
    {
        // on pointer enter
        if (scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);

        DOTween.Kill(2, true);

        shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2);
    }

    private void OnPointerExit(Card card)
    {
        // on pointer exit
        if (!parentCard.wasDragged)
            transform.DOScale(1, scaleTransition).SetEase(scaleEase);
    }

    private void OnPointerUp(Card card, bool longPress)
    {
        // on pointer up
        if (scaleAnimations)
            transform.DOScale(longPress ? scaleOnHover : scaleOnSelect, scaleTransition).SetEase(scaleEase);

        canvas.overrideSorting = false;

        visualShadow.localPosition = shadowDistance; // move the shadow 

        shadowCanvas.overrideSorting = true;
    }

    private void OnPointerDown(Card card)
    {
        // on pointer down
        if (scaleAnimations)
            transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase);

        visualShadow.localPosition += -Vector3.up * shadowOffset;// reset the pos of shadow

        shadowCanvas.overrideSorting = false;
    }

    private void OnSelect(Card card, bool state)
    {
        DOTween.Kill(2, true);

        float dir = state ? 1 : 0;

        shakeParent.DOPunchPosition(shakeParent.up * selectPunchAmount * dir, scaleTransition, 10, 1);

        shakeParent.DOPunchRotation(Vector3.forward * (hoverPunchAngle / 2), hoverTransition, 20, 1).SetId(2);

        if (scaleAnimations)
            transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase);
    }


}
