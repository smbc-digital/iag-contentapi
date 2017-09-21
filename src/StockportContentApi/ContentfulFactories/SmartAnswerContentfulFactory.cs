using Newtonsoft.Json;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class SmartAnswerContentfulFactory : IContentfulFactory<ContentfulSmartAnswers, SmartAnswer>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SmartAnswerContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public SmartAnswer ToModel(ContentfulSmartAnswers entry)
        {
            var stringQuestionJson = JsonConvert.SerializeObject(entry.QuestionJson);
            return new SmartAnswer(entry.Title, entry.Slug, stringQuestionJson).StripData(_httpContextAccessor);
        }
    }
}
