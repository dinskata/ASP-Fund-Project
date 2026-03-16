using System.Globalization;

namespace ASP_Fund_Project.Helpers;

public static class DateFormatter
{
    public static string EnglishShort(DateTime value) =>
        value.ToString("dd MMM yyyy", CultureInfo.GetCultureInfo("en-GB"));
}
