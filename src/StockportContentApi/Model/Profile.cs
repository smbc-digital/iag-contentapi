using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Profile
    {
        public string Title { get; }
        public string Slug { get; }
        public string Subtitle { get; }
        public string Quote { get; }
        public string Image { get; }
        public string Body { get; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public List<Alert> Alerts { get; }
        public string TriviaSubheading { get; }
        public List<InformationList> TriviaSection { get; }
        public FieldOrder FieldOrder { get; }
        public string Author { get; }
        public string Subject { get; }

        public Profile()
        {

        }

        public Profile(string title,
            string slug,
            string subtitle,
            string quote,
            string image,
            string body,
            IEnumerable<Crumb> breadcrumbs,
            List<Alert> alerts,
            string triviaSubheading,
            List<InformationList> triviaSection,
            FieldOrder fieldOrder,
            string author,
            string subject)
        {
            Title = title;
            Slug = slug;
            Subtitle = subtitle;
            Quote = quote;
            Image = image;
            Body = body;
            Breadcrumbs = breadcrumbs;
            Alerts = alerts;
            TriviaSubheading = triviaSubheading;
            TriviaSection = triviaSection;
            FieldOrder = fieldOrder;
            Author = author;
            Subject = subject;
        }

    }
}
