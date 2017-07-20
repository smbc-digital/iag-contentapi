using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulSmartAnswerBuilder
    {
        public string _slug = "smartAnswers";
        public string _questionJson = "questionJson";

        public ContentfulSmartAnswers Build()
        {
            return new ContentfulSmartAnswers()
            {
                Slug = _slug,
                QuestionJson = _questionJson
            };
        }

        public ContentfulSmartAnswerBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }
    }
}
