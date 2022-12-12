using Contentful.Core.Models;

namespace StockportContentApi.Utils
{
    public class ContentfulHelpers
    {
        public static bool EntryIsNotALink(SystemProperties sys)
        {
            return sys.Type != "Link";
        }

        public static IEnumerable<string> ConvertToListOfStrings(IEnumerable<dynamic> term)
        {
            return term.Cast<string>().ToList();
        }
    }
}
