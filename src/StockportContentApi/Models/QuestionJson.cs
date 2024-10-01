namespace StockportContentApi.Models;

public class QuestionJson
{
    public int PageId { get; set; }
    public string ButtonText { get; set; }
    public bool ShouldCache { get; set; }
    public bool IsLastPage { get; set; }
    public List<Question> Questions { get; set; }
    public List<Behaviour> Behaviours { get; set; }
    public string Description { get; set; }
    public bool HideBackButton { get; set; }
    public InlineAlert Alert { get; set; }
}

public class InlineAlert
{
    public string Icon { get; set; }
    public string Content { get; set; }
}

public class Option
{
    public string Label { get; set; }
    public string Value { get; set; }
    public string Image { get; set; }
    public string TertiaryInformation { get; set; }
    public string SubLabel { get; set; }
}

public class ValidatorData
{
    public string Type { get; set; }
    public string Message { get; set; }
    public string Value { get; set; }
}


public class Question
{
    public string questionId { get; set; }
    public string secondaryInfo { get; set; }
    public string textSubLabel { get; set; }
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
    public object redirectValue { get; set; }
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
