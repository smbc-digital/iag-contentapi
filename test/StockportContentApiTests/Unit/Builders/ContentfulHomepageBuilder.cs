using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulHomepageBuilder
    {
        private readonly List<string> _popularSearchTerms = new List<string> { "popular search term" };
        private readonly string _featuredTasksHeading = "Featured tasks heading";
        private readonly string _featuredTasksSummary = "Featured tasks summary";
        private readonly Asset _backgroundImage = new Asset { File = new File { Url = "image.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
        private readonly string _freeText = "homepage text";
        private IEnumerable<ContentfulGroup> _featuredGroups = new List<ContentfulGroup>
        {
            new ContentfulGroupBuilder().Build()
        };

        private readonly List<ContentfulCarouselContent> _carouselContents = new List<ContentfulCarouselContent>
        {
            new ContentfulCarouselContent
            {
                Title = "Red Rock is opening this Autumn",
                Slug = "red-rock-opening",
                Teaser = "The long awaited cinema complex is due to open late Oct 2016. Come and take a look.",
                Image = new Asset { File = new File { Url = "image.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } },
                SunriseDate = new DateTime(2016, 9, 1, 0, 0, 0, DateTimeKind.Utc),
                SunsetDate = new DateTime(2016, 9, 30, 0, 0, 0, DateTimeKind.Utc),
                Url = "http://fake.url",
                Sys = new SystemProperties {Type = "Entry"}
            }
        };

        private readonly List<ContentfulAlert> _alerts = new List<ContentfulAlert>
        {
            new ContentfulAlertBuilder().Build()
        };

        private readonly List<ContentfulReference> _featuredTasks = new List<ContentfulReference>();

        private readonly List<ContentfulReference> _featuredTopics = new List<ContentfulReference>
        {
            new ContentfulTopicBuilder().Slug("council-tax").Name("Council Tax")
                                        .Teaser("How to pay, discounts")
                                        .Summary(string.Empty)
                                        .Icon("si-house")
                                        .BackgroundImage(new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } })
                                        .Image(new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } })
                                        .SubItems(new List<ContentfulReference>())
                                        .SecondaryItems(new List<ContentfulReference>())
                                        .TertiaryItems(new List<ContentfulReference>())
                                        .Breadcrumbs(new List<ContentfulReference>())
                                        .Alerts(new List<ContentfulAlert>())
                                        .SunriseDate(DateTime.MinValue)
                                        .SunsetDate(DateTime.MinValue)
                                        .EmailAlerts(false)
                                        .EmailAlertsTopicId(string.Empty)
                                        .EventBanner(new ContentfulEventBannerBuilder().Build())
                                        .ExpandingLinkTitle(string.Empty)
                                        .ExpandingLinkBoxes(new List<ContentfulExpandingLinkBox>())
                                        .Build()
        };

        private readonly SystemProperties _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
        };

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
                Sys = _sys
            };
        }

        public ContentfulHomepageBuilder FeaturedGroups(List<ContentfulGroup> featuredGroups)
        {
            _featuredGroups = featuredGroups;
            return this;
        }
    }
}
