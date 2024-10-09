namespace StockportContentApi.Utils;

[ExcludeFromCodeCoverage]
public class ContentfulHelpers
{
    public static bool EntryIsNotALink(SystemProperties sys) =>
        sys.LinkType is null;

    public static IEnumerable<string> ConvertToListOfStrings(IEnumerable<dynamic> term) =>
        term.Cast<string>().ToList();
}
