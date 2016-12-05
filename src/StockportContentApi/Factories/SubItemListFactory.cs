using System;
using StockportContentApi.Model;
using System.Collections.Generic;
using System.Linq;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class SubItemListFactory : IBuildContentTypesFromReferences<SubItem>
    {
        private readonly IFactory<SubItem> _subitemFactory;
        private readonly ITimeProvider _timeProvider;
        private readonly DateComparer _dateComparer;

        public SubItemListFactory(IFactory<SubItem> subitemFactory, ITimeProvider timeProvider)
        {
            _subitemFactory = subitemFactory;
            _timeProvider = timeProvider;
            _dateComparer = new DateComparer(_timeProvider);
        }

        public IEnumerable<SubItem> BuildFromReferences(IEnumerable<dynamic> references,
            IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<SubItem>();
            IEnumerable<dynamic> entries = contentfulResponse.GetEntriesAndItemsFor(references);
           
            if (entries == null) return new List<SubItem>();
            return entries
                .Select(item => _subitemFactory.Build(item, contentfulResponse))
                .Cast<SubItem>()
                .Where(SubItem => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(SubItem.SunriseDate,SubItem.SunsetDate))
                .ToList();
        }
    }
}
