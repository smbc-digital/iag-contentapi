using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulHomepageBuilder
    {
        private readonly List<string> _popularSearchTerms = new List<string> { "popular search term" };
        private readonly string _featuredTasksHeading = "Featured tasks heading";
        private readonly string _featuredTasksSummary = "Featured tasks summary";
        private readonly Asset _backgroundImage = new Asset { File = new File { Url = "image.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
        private readonly string _freeText = "homepage text";
        private readonly string _metaDescription = "meta description";
        private IEnumerable<ContentfulGroup> _featuredGroups = new List<ContentfulGroup>
        {
            new ContentfulGroupBuilder().Build()
        };

        private readonly List<ContentfulCarouselContent> _carouselContents = new List<ContentfulCarouselContent>
        {
            new ContentfulCarouselContentBuilder().Build()
        };

        private readonly List<ContentfulAlert> _alerts = new List<ContentfulAlert>
        {
            new ContentfulAlertBuilder().Build()
        };

        private readonly List<ContentfulReference> _featuredTasks = new List<ContentfulReference>();

        private readonly List<ContentfulReference> _featuredTopics = new List<ContentfulReference>
        {
            new ContentfulTopicBuilder().Build()
        };

        private readonly SystemProperties _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
        };

        private readonly ContentfulCarouselContent _campaignBanner = new ContentfulCarouselContentBuilder().Build();

        public ContentfulHomepage Build()
        {
            return new ContentfulHomepage
            {
                PopularSearchTerms = _popularSearchTerms,
                FeaturedTasksHeading = _featuredTasksHeading,
                FeaturedTasksSummary = _featuredTasksSummary,
                FeaturedTasks = _featuredTasks,
                FeaturedTopics = _featuredTopics,
                Alerts = _alerts,
                CarouselContents = _carouselContents,
                BackgroundImage = _backgroundImage,
                FreeText = _freeText,
                FeaturedGroups = _featuredGroups,
                Sys = _sys,
                MetaDescription = _metaDescription,
                CampaignBanner = _campaignBanner
            };
        }

        public ContentfulHomepageBuilder FeaturedGroups(List<ContentfulGroup> featuredGroups)
        {
            _featuredGroups = featuredGroups;
            return this;
        }
    }
}
