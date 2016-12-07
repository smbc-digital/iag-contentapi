using System.Collections.Generic;
using StockportContentApi.Model;
using System.Linq;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class TopicListFactory :  IBuildContentTypesFromReferences<Topic>
    {
        private readonly IFactory<Topic> _topicFactory;
        private readonly DateComparer _dateComparer;

        public TopicListFactory(IFactory<Topic> topicFactory, ITimeProvider timeProvider)
        {
            _topicFactory = topicFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public IEnumerable<Topic> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<Topic>();
            var topicEntries = contentfulResponse.GetEntriesFor(references);

            if (topicEntries == null) return new List<Topic>();

            return topicEntries
                .Select(item => _topicFactory.Build(item, contentfulResponse))
                .Cast<Topic>()
                .Where(topic => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(topic.SunriseDate,topic.SunsetDate))
                .ToList();
        }
    }
}