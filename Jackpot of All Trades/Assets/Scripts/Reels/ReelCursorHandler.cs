using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ReelCursorHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler, IPointerClickHandler
{
    [Header("Cursor Textures")]
    public Texture2D defaultCursor;
    public Texture2D upCursor;
    public Texture2D downCursor;
    public Texture2D lockCursor;
    public Texture2D unlockCursor;

    [Header("Threshold Settings")]
    [Range(0.01f, 0.49f)]
    public float edgeThresholdPercent = 0.33f;

    [Header("References and Data")]
    public Reel reel;
    private RectTransform rectTransform;
    private bool isPointerOver = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;
        UpdateCursor(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);


        //Hide spell tooltip
        if (TooltipUI.Instance != null)
            TooltipUI.Instance.Hide();
    }


    public void OnPointerMove(PointerEventData eventData)
    {
        if (isPointerOver)
            UpdateCursor(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPointerOver)
        {
            UpdateCursor(eventData);
        }
    }

    //also sets spell tooltip
    private void UpdateCursor(PointerEventData eventData)
    {
        if (rectTransform == null) return;

        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.enterEventCamera,
            out localPoint))
            return;

        float normalizedY = Mathf.InverseLerp(
            rectTransform.rect.yMin,
            rectTransform.rect.yMax,
            localPoint.y
        );

        SpellSO spell = null;

        //up arrow cursor at top of reel

        if (normalizedY >= (1f - edgeThresholdPercent))
        {
            Cursor.SetCursor(upCursor, Vector2.zero, CursorMode.Auto);
            spell = reel.GetTopSpell();
        }

        //down arrow cursor at bottom of reel
        else if (normalizedY <= edgeThresholdPercent)
        {
            Cursor.SetCursor(downCursor, Vector2.zero, CursorMode.Auto);
            spell = reel.GetBottomSpell();
        }

        //lock or unlock cursor at center of reel depending on lock state
        else
        {
            Cursor.SetCursor(reel.IsLocked ? unlockCursor : lockCursor, Vector2.zero, CursorMode.Auto);
            spell = reel.GetCenterSpell();
        }

        // Show tooltip only if spell is valid and tooltip instance exists
        if (spell != null && TooltipUI.Instance != null)
        {
            TooltipUI.Instance.Show(spell);
            TooltipUI.Instance.SetPosition(eventData.position, eventData.enterEventCamera);
        }
    }
}