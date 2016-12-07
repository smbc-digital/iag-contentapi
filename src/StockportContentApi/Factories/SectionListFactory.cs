using System;
using System.Collections.Generic;
using StockportContentApi.Model;
using System.Linq;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class SectionListFactory : IBuildContentTypesFromReferences<Section>
    {
        private readonly IBuildContentTypesFromReferences<Profile> _profileListFactory;
        private readonly IBuildContentTypesFromReferences<Document> _documentListFactory;
        private readonly ITimeProvider _timeProvider;

        public SectionListFactory(IBuildContentTypesFromReferences<Profile> profileListFactory, IBuildContentTypesFromReferences<Document> documentListFactory, ITimeProvider timeProvider)
        {
            _profileListFactory = profileListFactory;
            _documentListFactory = documentListFactory;
            _timeProvider = timeProvider;
        }

        public IEnumerable<Section> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            var sunrisesunsetDates =  new DateComparer(_timeProvider);

            if (references == null) return new List<Section>();
            var sectionEntries = contentfulResponse.GetEntriesFor(references);
            var sections = sectionEntries
                .Select(entry => BuildSection(entry, contentfulResponse))
                .Where(item => item != null)
                .Cast<Section>()
                .Where(section => sunrisesunsetDates.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate,section.SunsetDate))
                .ToList();

            return sections;
        }

        private Section BuildSection(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry.fields == null) return null;

            var title = entry.fields.title.ToString();
            var slug = entry.fields.slug.ToString();
            var body = entry.fields.body.ToString();
            var profiles = _profileListFactory.BuildFromReferences(entry.fields.profiles, contentfulResponse);
            var documents = _documentListFactory.BuildFromReferences(entry.fields.documents, contentfulResponse);

            DateTime sunriseDate = DateComparer.DateFieldToDate(entry.fields.sunriseDate);
            DateTime sunsetDate = DateComparer.DateFieldToDate(entry.fields.sunsetDate);

            return new Section(title, slug, body, profiles, documents,sunriseDate,sunsetDate);
        }

    }
}
