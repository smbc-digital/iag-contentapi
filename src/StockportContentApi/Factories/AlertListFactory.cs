using StockportContentApi.Model;
using System.Collections.Generic;
using StockportContentApi.Utils;
using System.Linq;

namespace StockportContentApi.Factories
{
    public class AlertListFactory : IBuildContentTypesFromReferences<Alert>
    {
        private readonly ITimeProvider _timeProvider;
        private readonly IFactory<Alert> _alertFactory;
        private readonly SunriseSunsetDates _sunriseSunsetDates;

        public AlertListFactory(ITimeProvider timeProvider, IFactory<Alert> alertFactory)
        {
            _timeProvider = timeProvider;
            _alertFactory = alertFactory;
            _sunriseSunsetDates = new SunriseSunsetDates(_timeProvider);
        }

        public IEnumerable<Alert> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<Alert>();
            var alertEntries = contentfulResponse.GetEntriesFor(references);

            if (alertEntries == null) return new List<Alert>();
            return alertEntries
               .Select(item => _alertFactory.Build(item, contentfulResponse))
               .Cast<Alert>()
               .Where(alert => _sunriseSunsetDates.CheckIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
               .ToList();
        }
    }
}
