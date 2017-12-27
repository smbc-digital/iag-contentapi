using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories.SmartAnswersFactories
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
