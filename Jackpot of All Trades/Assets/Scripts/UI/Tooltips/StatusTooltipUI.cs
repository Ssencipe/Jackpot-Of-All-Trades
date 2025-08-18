using UnityEngine;
using UnityEngine.EventSystems;

public class StatusTooltipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private IStatusEffect effect;
    private bool isHovering;

    public void Initialize(IStatusEffect attachedEffect)
    {
        effect = attachedEffect;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        StatusTooltipPanel.Instance?.Show(effect);
        StatusTooltipPanel.Instance?.SetPosition(Input.mousePosition, Camera.main);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        StatusTooltipPanel.Instance?.Hide();
    }

    private void Update()
    {
        if (isHovering && StatusTooltipPanel.Instance != null)
        {
            StatusTooltipPanel.Instance?.SetPosition(Input.mousePosition, Camera.main);
        }
    }
}