public static class TooltipFormatter
{
    public static string FormatEnumName(string raw)
    {
        return System.Text.RegularExpressions.Regex.Replace(raw, "(\\B[A-Z])", " $1");
    }

    public static string FormatTickTiming(TickTiming timing)
    {
        return FormatEnumName(timing.ToString());
    }

    public static string FormatEffectType(OverTimeType type)
    {
        return FormatEnumName(type.ToString());
    }
}