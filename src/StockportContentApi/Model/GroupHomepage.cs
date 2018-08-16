using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class GroupHomepage
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string BackgroundImage { get; set; }
        public string FeaturedGroupsHeading { get; set; }
        public List<Group> FeaturedGroups { get; set; }
        public GroupCategory FeaturedGroupsCategory { get; set; }
        public GroupSubCategory FeaturedGroupsSubCategory { get; set; }
        public IEnumerable<Alert> Alerts { get; set; }
        public string Body { get; set; }
        public string SecondaryBody { get; set; }

        public GroupHomepage(string title, string slug, string backgroundImage, string featuredGroupsHeading, List<Group> featuredGroups, 
            GroupCategory featuredGroupsCategory, GroupSubCategory featuredGroupsSubCategory, IEnumerable<Alert> alerts, string body, string secondaryBody)
        {
            Title = title;
            Slug = slug;
            BackgroundImage = backgroundImage;
            FeaturedGroups = featuredGroups;
            FeaturedGroupsCategory = featuredGroupsCategory;
            FeaturedGroupsHeading = featuredGroupsHeading;
            FeaturedGroupsSubCategory = featuredGroupsSubCategory;
            Alerts = alerts;
            Body = body;
            SecondaryBody = secondaryBody;
        }
    }
}
