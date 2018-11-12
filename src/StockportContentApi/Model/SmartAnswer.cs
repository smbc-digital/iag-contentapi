namespace StockportContentApi.Model
{
    public class SmartAnswer
    {
        public string Title { get; }
        public string Slug { get; } 
        public string QuestionJson { get; }

        public SmartAnswer(string title, string slug, string questionJson)
        {
            Title = title;
            Slug = slug;
            QuestionJson = questionJson;
        }
    }
}
