using System;
using System.Collections.Generic;
using StockportContentApi.Model;
using System.Linq;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class TopicListFactory :  IBuildContentTypesFromReferences<Topic>
    {
        private readonly IFactory<Topic> _topicFactory;
        private readonly ITimeProvider _timeProvider;
        private readonly SunriseSunsetDates _sunriseSunsetDates;

        public TopicListFactory(IFactory<Topic> topicFactory, ITimeProvider timeProvider)
        {
            _topicFactory = topicFactory;
            _timeProvider = timeProvider;
            _sunriseSunsetDates = new SunriseSunsetDates(_timeProvider);
        }

        public IEnumerable<Topic> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<Topic>();
            var topicEntries = contentfulResponse.GetEntriesFor(references);

            if (topicEntries == null) return new List<Topic>();

            return topicEntries
                .Select(item => _topicFactory.Build(item, contentfulResponse))
                .Cast<Topic>()
                .Where(topic => _sunriseSunsetDates.CheckIsWithinSunriseAndSunsetDates(topic.SunriseDate,topic.SunsetDate))
                .ToList();
        }
    }
}