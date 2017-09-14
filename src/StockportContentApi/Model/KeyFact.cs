using System;
namespace StockportContentApi.Model
{
    public class KeyFact
    {
        public string Icon { get; }
        public string Text { get; }
        public string Link { get; }

        public KeyFact(string icon, string text, string link)
        {
            Icon = icon;
            Text = text;
            Link = link;
        }
    }
}
