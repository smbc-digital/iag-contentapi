using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class SmartAnswerContentfulFactory : IContentfulFactory<ContentfulSmartAnswers, SmartAnswer>
    {
        public SmartAnswer ToModel(ContentfulSmartAnswers entry)
        {
            var stringQuestionJson = JsonConvert.SerializeObject(entry.QuestionJson);
            return new SmartAnswer(entry.Title, entry.Slug, stringQuestionJson);
        }
    }
}
