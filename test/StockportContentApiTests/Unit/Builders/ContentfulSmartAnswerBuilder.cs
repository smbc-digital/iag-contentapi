using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulSmartAnswerBuilder
    {
        public string _title = "smartAnswer_title";
        public string _slug = "smartAnswers";
        public List<QuestionJson> _questionJson = new List<QuestionJson>() {
            new QuestionJson() {pageId = 1, buttonText = "Next", ShouldCache = true, description = "Question 1 description",
                behaviours = new List<Behaviour> {new Behaviour {behaviourType = EQuestionType.GoToPage, conditions = new List<Condition> {new Condition {equalTo = "test", questionId = "2"} } } },
                questions = new List<Question> { new Question {questionId = "2", label = "questionLabel", options = new List<Option> {new Option {label = "optionLabel", value = "Label"} } } } }};

        public ContentfulSmartAnswers Build()
        {
            return new ContentfulSmartAnswers()
            {
                Title = _title,
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
