using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{

    [SerializeField] private bool instantiateVisual = true;
    private CardVisualsHandler cardVisualsHandler;
    private Canvas canvas;
    private Image imageComponent;
    private Vector3 offset;

    [Header("Movement")]
    [SerializeField] private float moveSpeedLimit = 50f;

    [Header("Selection")]
    public bool isSelected = false;
    public float selectionOffset = 50f;
    private float pointerDownTime;
    private float pointerUpTime;

    [Header("States")]
    public bool isDragging = false;
    public bool isHovering = false;
    [HideInInspector] public bool wasDragged = false;

    [Header("Visual")]
    [SerializeField] private GameObject cardVisualPrefab;
    [HideInInspector] public CardVisual cardVisual;

    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();

        imageComponent = GetComponent<Image>();

        if (!instantiateVisual) return;

        cardVisualsHandler = FindFirstObjectByType<CardVisualsHandler>();

        cardVisual = Instantiate(cardVisualPrefab, cardVisualsHandler ? cardVisualsHandler.transform : canvas.transform).GetComponent<CardVisual>();

        cardVisual.Initialize(this);
    }

    private void Update()
    {
        ClampPosition();

        if (isDragging)
        {
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;

            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

            Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.deltaTime);

            transform.Translate(velocity * Time.deltaTime);
        }
    }

    /// <summary>
    ///  Clamp the position of the card in the screen size
    /// </summary>
    private void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        Vector3 clampedPosition = transform.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);

        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);

        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public int SiblingAmount()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
    }

    public int ParentIndex()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }

    public float NormalizedPosition()
    {
        return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
    }

    public void Deselect()
    {
        if (isSelected)
        {
            isSelected = false;

            if (isSelected)
                transform.localPosition += cardVisual.transform.up * 50;
            else
                transform.localPosition = Vector3.zero;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // on drag
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent.Invoke(this);    // Invoke the event

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        offset = mousePosition - (Vector2)transform.position;

        isDragging = true;

        canvas.GetComponent<GraphicRaycaster>().enabled = false; // Disable raycast target to allow raycast to pass through the card

        imageComponent.raycastTarget = false;

        wasDragged = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent.Invoke(this);

        isDragging = false;

        canvas.GetComponent<GraphicRaycaster>().enabled = true;

        imageComponent.raycastTarget = true;

        StartCoroutine(FrameWait());

        IEnumerator FrameWait()
        {
            yield return new WaitForEndOfFrame();

            wasDragged = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent.Invoke(this);

        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent.Invoke(this);

        isHovering = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        pointerUpTime = Time.time;

        PointerUpEvent.Invoke(this, pointerUpTime - pointerDownTime > 0.2f);

        if (pointerUpTime - pointerDownTime > 0.2f) return;

        if (wasDragged) return;

        isSelected = !isSelected;

        SelectEvent.Invoke(this, isSelected);

        if (isSelected)
        {
            transform.localPosition += cardVisual.transform.up * selectionOffset;
        }
        else
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        PointerEnterEvent.Invoke(this);

        pointerDownTime = Time.time;
    }

}
