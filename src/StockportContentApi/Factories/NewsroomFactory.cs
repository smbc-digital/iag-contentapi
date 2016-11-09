using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class NewsroomFactory : IFactory<Newsroom>
    {
        private readonly IBuildContentTypesFromReferences<Alert> _alertListFactory;

        public NewsroomFactory(IBuildContentTypesFromReferences<Alert> alertListFactory)
        {
            _alertListFactory = alertListFactory;
        }

        public Newsroom Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            var alerts = _alertListFactory.BuildFromReferences(entry.fields.alerts, contentfulResponse);
            var emailAlerts = false;
            if (entry.fields.emailAlerts != null) bool.TryParse((string)entry.fields.emailAlerts, out emailAlerts);
            var emailAlertsTopicId = (string)entry.fields.emailAlertsTopicId ?? string.Empty;

            return new Newsroom(alerts, emailAlerts, emailAlertsTopicId);
        }
    }
}