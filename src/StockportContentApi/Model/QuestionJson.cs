using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class QuestionJson
    {
        public int pageId { get; set; }
        public string buttonText { get; set; }
        public bool ShouldCache { get; set; }
        public bool IsLastPage { get; set; }
        public List<Question> questions { get; set; }
        public List<Behaviour> behaviours { get; set; }
        public string description { get; set; }
        public bool HideBackButton { get; set; }
    }

    public class Option
    {
        public string label { get; set; }
        public string value { get; set; }
        public string image { get; set; }
        public string tertiaryInformation { get; set; }
    }

    public class ValidatorData
    {
        public string type { get; set; }
        public string message { get; set; }
        public string value { get; set; }
    }


    public class Question
    {
        public string questionId { get; set; }
        public string secondaryInfo { get; set; }
        public string questionType { get; set; }
        public string label { get; set; }
        public List<Option> options { get; set; }
        public List<ValidatorData> validatorData { get; set; }
    }

    public class Condition
    {
        public string questionId { get; set; }
        public string equalTo { get; set; }
        public string between { get; set; }
    }

    public class Behaviour
    {
        public EQuestionType behaviourType { get; set; }
        public List<Condition> conditions { get; set; }
        public object value { get; set; }
    }

    public enum EQuestionType
    {
        Redirect,
        RedirectToAction,
        RedirectToActionController,
        GoToPage,
        GoToSummary,
        HandOffData
    }
}
