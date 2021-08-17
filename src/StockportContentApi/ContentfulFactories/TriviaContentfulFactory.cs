using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class TriviaContentfulFactory : IContentfulFactory<ContentfulTrivia, Trivia>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TriviaContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Trivia ToModel(ContentfulTrivia entry)
        {
            return new Trivia(entry.Name, entry.Icon, entry.Text, entry.Link).StripData(_httpContextAccessor);
        }
    }
}
