using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class SmartAnswerContentfulFactory : IContentfulFactory<ContentfulSmartAnswers, SmartAnswer>
    {
        public SmartAnswer ToModel(ContentfulSmartAnswers entry)
        {
            return new SmartAnswer(entry.Slug, entry.QuestionJson);
        }
    }
}
