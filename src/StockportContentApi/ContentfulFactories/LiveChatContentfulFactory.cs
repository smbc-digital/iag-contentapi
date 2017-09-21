using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class LiveChatContentfulFactory : IContentfulFactory<ContentfulLiveChat, LiveChat>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LiveChatContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public LiveChat ToModel(ContentfulLiveChat entry)
        {
            return new LiveChat(entry.Title, entry.Text).StripData(_httpContextAccessor);
        }
    }
}