namespace StockportContentApi.Utils;

public static class Ensure
{
    public static void ArgumentNotNullOrEmpty(string variable, string name)
    {
        if (string.IsNullOrWhiteSpace(variable))
            throw new ArgumentException($"'{name}' cannot be null or empty.");
    }

    public static void ArgumentNotNull(object variable, string name)
    {
        if (variable == null)
            throw new ArgumentNullException($"'{name}' cannot be null or empty.");
    }
}