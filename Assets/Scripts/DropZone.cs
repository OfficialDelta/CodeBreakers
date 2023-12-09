using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    private UIDraggable currentDraggable; // Variable to keep track of the current draggable item
    private Transform originalParent;
    private bool isCorrect = false;

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            UIDraggable draggable = eventData.pointerDrag.GetComponent<UIDraggable>();

            if (draggable != null)
            {
                // Check if the drop zone already has an item
                if (currentDraggable == null)
                {
                    // Snap the draggable object to the center of the drop zone
                    originalParent = draggable.transform.parent;
                    draggable.transform.SetParent(transform, false);
                    draggable.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                    // Update currentDraggable to the new item
                    currentDraggable = draggable;
                }
            }
        }
    }

    // Optional: Method to handle item removal from the drop zone
    public void RemoveItem(UIDraggable draggable)
    {
        if (currentDraggable == draggable && !isCorrect)
        {
            currentDraggable = null;
            draggable.transform.SetParent(originalParent);
            originalParent = null;
        }
    }
}
