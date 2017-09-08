using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class GroupHomepage
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string BackgroundImage { get; set; }

        public GroupHomepage(string title, string slug, string backgroundImage)
        {
            Title = title;
            Slug = slug;
            BackgroundImage = backgroundImage;
        }
    }
}
