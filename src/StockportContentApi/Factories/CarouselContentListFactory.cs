using System.Collections.Generic;
using StockportContentApi.Utils;
using StockportContentApi.Model;
using System.Linq;

namespace StockportContentApi.Factories
{
    public class CarouselContentListFactory : IBuildContentTypesFromReferences<CarouselContent>
    {
        private readonly IFactory<CarouselContent> _carouselContentFactory;
        private readonly DateComparer _dateComparer;

        public CarouselContentListFactory(ITimeProvider timeProvider, IFactory<CarouselContent> carouselContentFactory)
        {
            _carouselContentFactory = carouselContentFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public IEnumerable<CarouselContent> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<CarouselContent>();
            var carouselContentEntries = contentfulResponse.GetEntriesFor(references);

            if (carouselContentEntries == null) return new List<CarouselContent>();

            return carouselContentEntries
                .Select(item => _carouselContentFactory.Build(item, contentfulResponse))
                .Cast<CarouselContent>()
                .Where(item => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(item.SunriseDate, item.SunsetDate))
                .ToList();
        }  
    }
}