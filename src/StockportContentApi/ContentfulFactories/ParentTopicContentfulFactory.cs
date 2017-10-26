using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class ParentTopicContentfulFactory : IContentfulFactory<ContentfulArticle, Topic>
    {
        private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory;
        private readonly DateComparer _dateComparer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParentTopicContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subItemFactory,
            ITimeProvider timeProvider, IHttpContextAccessor httpContextAccessor)
        {
            _subItemFactory = subItemFactory;
            _dateComparer = new DateComparer(timeProvider);
            _httpContextAccessor = httpContextAccessor;
        }

        private ContentfulArticle _entry;

        public Topic ToModel(ContentfulArticle entry)
        {
            _entry = entry;

            var topicInBreadcrumb = entry.Breadcrumbs.LastOrDefault(o => o.Sys.ContentType.SystemProperties.Id == "topic");

            if (topicInBreadcrumb == null) return new NullTopic();

            var subItems = topicInBreadcrumb.SubItems
                .Where(subItem => subItem != null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

            var secondaryItems = topicInBreadcrumb.SecondaryItems
                .Where(subItem => subItem != null  && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

            var tertiaryItems = topicInBreadcrumb.TertiaryItems
                .Where(subItem => subItem != null && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                .Select(subItem => _subItemFactory.ToModel(subItem)).ToList();

            if (!topicInBreadcrumb.SubItems.Any(x => x.Sys.Id == _entry.Sys.Id) && (!topicInBreadcrumb.SecondaryItems.Any(x => x.Sys.Id == _entry.Sys.Id)) && (!topicInBreadcrumb.TertiaryItems.Any(x => x.Sys.Id == _entry.Sys.Id)))
            {
                subItems.Insert(0, _subItemFactory.ToModel(entry));
            }
            
            return new Topic(topicInBreadcrumb.Name, topicInBreadcrumb.Slug, subItems, secondaryItems, tertiaryItems).StripData(_httpContextAccessor);
        }
    }
}