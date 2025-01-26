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
    [HideInInspector] private bool wasDragged = false;
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

        cardVisual.Initalize(this);
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

    private void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        Vector3 clampedPosition = transform.position;

        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);

        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);

        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
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
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

}
