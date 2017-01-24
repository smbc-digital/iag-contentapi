using Contentful.Core.Models;

namespace StockportContentApi.Utils
{
    public class ContentfulHelpers
    {
        public static bool EntryIsNotALink(SystemProperties sys) {
            return sys.Type != "Link";
        }
    }
}
