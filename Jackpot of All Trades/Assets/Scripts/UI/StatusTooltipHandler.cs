using UnityEngine;

public static class StatusTooltipHandler
{
    public static void ShowStatusTooltip(IStatusEffect effect)
    {
        if (effect == null || TooltipUI.Instance == null) return;

        TooltipUI.Instance.ClearSpellFields(); // Clear before setting anything

        TooltipUI.Instance.SetTitle(effect.ID);
        TooltipUI.Instance.SetDescription(BuildDescription(effect));
        TooltipUI.Instance.SetIcon(effect.Icon);
        TooltipUI.Instance.SetDuration(effect.Duration.ToString());
                                             
        TooltipUI.Instance.SetBackgroundColor(new Color(0.3f, 0f, 0f, 0.95f)); // Use direct RGBA value for status background

        TooltipUI.Instance.ShowUI();
    }

    private static string BuildDescription(IStatusEffect effect)
    {
        if (effect is OverTimeStatusInstance overtime)
        {
            return $"Applies {overtime.Potency} {overtime.Type} each turn\n"
                 + $"Triggers at {overtime.TickTiming}";
        }

        return "Status effect applied to unit.";
    }
}