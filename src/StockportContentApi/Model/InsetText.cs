﻿namespace StockportContentApi.Model
{
    public class InsetText
    {
        public string Title { get; }
        public string SubHeading { get; }
        public string Body { get; }
        public string Colour { get; }
        public string Slug { get; set; }

        public InsetText(string title, string subHeading, string body, string colour, string slug)
        {
            Title = title;
            SubHeading = subHeading;
            Body = body;
            Colour = colour;
            Slug = slug;
        }
    }

    public class NullInsetText : InsetText
    {
        public NullInsetText() : base(string.Empty,string.Empty,string.Empty,string.Empty,string.Empty) { }
    }

}
