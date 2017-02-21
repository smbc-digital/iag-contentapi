     using System.Collections.Generic;
using System.Linq;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class HomepageFactory : IFactory<Homepage>
    {
        private readonly IBuildContentTypesFromReferences<Alert> _alertListFactory;
        private readonly IBuildContentTypesFromReferences<Topic> _topicListFactory;
        private readonly IBuildContentTypesFromReferences<CarouselContent> _carouselContentListFactory;
        private readonly IBuildContentTypesFromReferences<SubItem> _subitemFactory;

        public HomepageFactory(IBuildContentTypesFromReferences<Topic> topicListFactory, IBuildContentTypesFromReferences<Alert> alertListFactory, IBuildContentTypesFromReferences<CarouselContent> carouselContentListFactory, IBuildContentTypesFromReferences<SubItem> subitemFactory)
        {
            _alertListFactory = alertListFactory;
            _topicListFactory = topicListFactory;
            _carouselContentListFactory = carouselContentListFactory;
            _subitemFactory = subitemFactory;
        }

        public Homepage Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            var fields = entry.fields;

            if (fields == null)
                return new NullHomepage();

            var popularSearchTerms = fields.popularSearchTerms != null
                ? ConvertToListOfStrings(fields.popularSearchTerms.ToObject<List<dynamic>>())
                : Enumerable.Empty<string>();
            var featuredTasksHeading = fields.featuredTasksHeading != null ? (string)fields.featuredTasksHeading : string.Empty;
            var featuredTasksSummary = fields.featuredTasksSummary != null ? (string)fields.featuredTasksSummary : string.Empty;
            var featuredTasks = _subitemFactory.BuildFromReferences(fields.featuredTasks, contentfulResponse);
            var featuredTopics = _topicListFactory.BuildFromReferences(fields.featuredTopics, contentfulResponse);
            var alerts = _alertListFactory.BuildFromReferences(fields.alerts, contentfulResponse);
            var carouselContents = _carouselContentListFactory.BuildFromReferences(fields.carouselContents, contentfulResponse);
            var freeText = fields.freeText != null ? (string)fields.freeText : string.Empty;
            var backgroundImage = contentfulResponse.GetImageUrl(fields.backgroundImage);

            return new Homepage(popularSearchTerms, featuredTasksHeading, featuredTasksSummary, featuredTasks, featuredTopics, alerts, carouselContents, backgroundImage, freeText);
        }

        private static IEnumerable<string> ConvertToListOfStrings(IEnumerable<dynamic> term)
        {
            return term.Cast<string>().ToList();
        }
    }
}
