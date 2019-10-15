namespace StockportContentApi.Model
{
    public class SmartAnswer
    {
        public string Title { get; }
        public string Slug { get; } 
        public string QuestionJson { get; }
        public string TypeformUrl { get; }

        public SmartAnswer(string title, string slug, string questionJson, string typeformUrl)
        {
            Title = title;
            Slug = slug;
            QuestionJson = questionJson;
            TypeformUrl = typeformUrl;
        }
    }
}
