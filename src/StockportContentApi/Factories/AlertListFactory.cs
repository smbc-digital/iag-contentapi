using StockportContentApi.Model;
using System.Collections.Generic;
using StockportContentApi.Utils;
using System.Linq;

namespace StockportContentApi.Factories
{
    public class AlertListFactory : IBuildContentTypesFromReferences<Alert>
    {
        private readonly IFactory<Alert> _alertFactory;
        private readonly DateComparer _dateComparer;

        public AlertListFactory(ITimeProvider timeProvider, IFactory<Alert> alertFactory)
        {
            _alertFactory = alertFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public IEnumerable<Alert> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<Alert>();
            var alertEntries = contentfulResponse.GetEntriesFor(references);

            if (alertEntries == null) return new List<Alert>();
            return alertEntries
               .Select(item => _alertFactory.Build(item, contentfulResponse))
               .Cast<Alert>()
               .Where(alert => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
               .ToList();
        }
    }
}
