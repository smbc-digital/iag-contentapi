using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using System.Linq;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class PrivacyNoticeContentfulFactory : IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>
    {
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly IContentfulFactory<ContentfulPrivacyNotice, Topic> _parentTopicFactory;

        public PrivacyNoticeContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory, IContentfulFactory<ContentfulPrivacyNotice, Topic> parentTopicFactory)
        {
            _crumbFactory = crumbFactory;
            _parentTopicFactory = parentTopicFactory;
        }

        public PrivacyNotice ToModel(ContentfulPrivacyNotice entry)
        {
            var breadcrumbs = entry.Breadcrumbs
                .Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var topic = _parentTopicFactory.ToModel(entry) ?? new NullTopic();

            var privacyNotice = new PrivacyNotice(entry.Slug, entry.Title, entry.Category, entry.OutsideEu, entry.AutomatedDecision, entry.Purpose, entry.TypeOfData, entry.Legislation, entry.Obtained, entry.ExternallyShared, entry.RetentionPeriod, entry.UrlOne, entry.UrlTwo, entry.UrlThree, breadcrumbs, topic);

            return privacyNotice;
        }
    }
}
