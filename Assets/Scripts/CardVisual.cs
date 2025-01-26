using System;
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
    public Transform visualShadow;
    [SerializeField] private Transform shakeParent;
    [SerializeField] private Transform tiltParent;
    [SerializeField] private Image cardImage;
    private Canvas shadowCanvas;

    [Header("Follow Parameters")]
    [SerializeField] private float followSpeed = 30;

    [Header("Rotation Parameters")]
    [SerializeField] private float rotationAmount = 20;
    [SerializeField] private float rotationSpeed = 20;
    [SerializeField] private float autoTiltAmount = 30;
    [SerializeField] private float manualTiltAmount = 20;
    [SerializeField] private float tiltSpeed = 20;

    [Header("Curve")]
    [SerializeField] private CurveParameters curve;
    private float curveRotationOffset;
    private float curveYOffset;

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
        Vector3 verticalOffset = (Vector3.up * (parentCard.isDragging ? 0 : curveYOffset));
        transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, followSpeed * Time.deltaTime);
    }

    private void FollowRotation()
    {
        Vector3 movement = (transform.position - cardTransform.position);
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
