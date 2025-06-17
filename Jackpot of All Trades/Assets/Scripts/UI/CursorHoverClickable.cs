using UnityEngine;
using UnityEngine.EventSystems;

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