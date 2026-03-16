using System.Globalization;

namespace ASP_Fund_Project.Helpers;

public static class CurrencyFormatter
{
    public static string Euro(decimal amount)
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"EUR {amount:N2}");
    }

    public static string Percent(decimal value)
    {
        return value.ToString("0.##", CultureInfo.InvariantCulture) + "%";
    }
}
