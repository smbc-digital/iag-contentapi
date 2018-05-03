using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.ContentfulFactories
{
    public class PrivacyNoticeContentfulFactory : IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>
    {
        public PrivacyNotice ToModel(ContentfulPrivacyNotice entry)
        {
            var privacyNotice = new PrivacyNotice(entry.Slug,entry.Title, entry.Directorate, entry.ActivitiesAsset, entry.TransactionsActivity, entry.Purpose, entry.TypeOfData, entry.Legislation, entry.Obtained, entry.ExternallyShared, entry.RetentionPeriod, entry.Conditions, entry.ConditionsSpecial, entry.UrlOne, entry.UrlTwo, entry.UrlThree);

            return privacyNotice;
        }
    }
}
