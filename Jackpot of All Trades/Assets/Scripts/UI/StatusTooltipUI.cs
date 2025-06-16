using UnityEngine;
using UnityEngine.EventSystems;

public class StatusTooltipUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private IStatusEffect effect;

    public void Initialize(IStatusEffect attachedEffect)
    {
        effect = attachedEffect;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipUI.Instance.Show(effect, Input.mousePosition, Camera.main);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.Instance.Hide();
    }
}