using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class PrivacyNoticeContentfulFactory : IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>
    {
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;

        public PrivacyNoticeContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory)
        {
            _crumbFactory = crumbFactory;
        }

        public PrivacyNotice ToModel(ContentfulPrivacyNotice entry)
        {
            var breadcrumbs = entry.Breadcrumbs
                .Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var privacyNotice = new PrivacyNotice(entry.Slug, entry.Title, entry.Category, entry.OutsideEu, entry.AutomatedDecision, entry.Purpose, entry.TypeOfData, entry.Legislation, entry.Obtained, entry.ExternallyShared, entry.RetentionPeriod, entry.UrlOne, entry.UrlTwo, entry.UrlThree, breadcrumbs);

            return privacyNotice;
        }
    }
}
