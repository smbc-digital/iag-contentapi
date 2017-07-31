using System.Collections.Generic;
using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class EventHomepageContentfulFactory : IContentfulFactory<ContentfulEventHomepage, EventHomepage>
    {
        private readonly DateComparer _dateComparer;

        public EventHomepageContentfulFactory(ITimeProvider timeProvider)
        {
            _dateComparer = new DateComparer(timeProvider);
        }

        public EventHomepage ToModel(ContentfulEventHomepage entry)
        {
            var tags = new List<string>();

            tags.Add(entry.Tag1);
            tags.Add(entry.Tag2);
            tags.Add(entry.Tag3);
            tags.Add(entry.Tag4);
            tags.Add(entry.Tag5);
            tags.Add(entry.Tag6);
            tags.Add(entry.Tag7);
            tags.Add(entry.Tag8);
            tags.Add(entry.Tag9);
            tags.Add(entry.Tag10);

            var rows = new List<EventHomepageRow>();
            rows.Add(new EventHomepageRow
            {
                IsLatest = true,
                Tag = string.Empty,
                Events = null
            });

            foreach (var tag in tags)
            {
                rows.Add(new EventHomepageRow
                {
                    IsLatest = false,
                    Tag = tag,
                    Events = null
                });
            }

            return new EventHomepage(rows);
        }
    }
}
