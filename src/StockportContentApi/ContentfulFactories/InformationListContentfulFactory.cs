using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class InformationListContentfulFactory : IContentfulFactory<ContentfulInformationList, InformationList>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InformationListContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public InformationList ToModel(ContentfulInformationList entry)
        {
            return new InformationList(entry.Name, entry.Icon, entry.Text, entry.Link).StripData(_httpContextAccessor);
        }
    }
}
