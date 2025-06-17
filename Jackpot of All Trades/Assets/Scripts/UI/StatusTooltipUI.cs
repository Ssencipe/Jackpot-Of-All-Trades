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
        StatusTooltipHandler.ShowStatusTooltip(effect);
        TooltipUI.Instance.SetPosition(Input.mousePosition, Camera.main);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        TooltipUI.Instance.Hide();
    }

    private void Update()
    {
        if (isHovering && TooltipUI.Instance != null)
        {
            TooltipUI.Instance.SetPosition(Input.mousePosition, Camera.main);
        }
    }
}