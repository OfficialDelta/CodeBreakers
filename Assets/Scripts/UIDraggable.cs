using UnityEngine;
using UnityEngine.EventSystems;

public class UIDraggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private DropZone currentDropZone; // Reference to the current drop zone

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        canvas.sortingOrder = 100; // 100 is an arbitrary number that should be higher than the sorting order of other canvases

        // Clear the reference to the drop zone when picked up
        if (currentDropZone != null)
        {
            currentDropZone.RemoveItem(this);
            currentDropZone = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor; // Move the element
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f; // Reset the transparency
        canvasGroup.blocksRaycasts = true; // Re-enable raycasts

        if (transform.parent.GetComponent<DropZone>())
        {
            currentDropZone = transform.parent.GetComponent<DropZone>();
        }
    }
}
