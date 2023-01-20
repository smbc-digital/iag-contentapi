using Contentful.Core.Models;

namespace StockportContentApi.Utils
{
    public class ContentfulHelpers
    {
        public static bool EntryIsNotALink(SystemProperties sys)
        {
            return sys.LinkType is null;
        }

        public static IEnumerable<string> ConvertToListOfStrings(IEnumerable<dynamic> term)
        {
            return term.Cast<string>().ToList();
        }
    }
}
