namespace StockportContentApi.Model
{
    public class BasicLink
    {
        public string Url { get; }

        public string Text { get; }

        public BasicLink(string url, string text)
        {
            Url = url;
            Text = text;
        }
    }

    public class NullBasicLink : BasicLink
    {
        public NullBasicLink() : base(string.Empty, string.Empty) { }
    }
}
