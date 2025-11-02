using UnityEngine;
using UnityEngine.EventSystems;

// Add to object to change cursor when hovering over it

public class CursorHoverClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.SetClickCursor();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CursorManager.Instance != null)
            CursorManager.Instance.SetDefaultCursor();
    }
}