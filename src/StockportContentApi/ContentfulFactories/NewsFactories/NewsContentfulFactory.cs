using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApi.ContentfulFactories.NewsFactories
{
    public class NewsContentfulFactory : IContentfulFactory<ContentfulNews, News>
    {
        private readonly IVideoRepository _videoRepository;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DateComparer _dateComparer;

        public NewsContentfulFactory(IVideoRepository videoRepository, IContentfulFactory<Asset, Document> documentFactory, IHttpContextAccessor httpContextAccessor, IContentfulFactory<ContentfulAlert, Alert> alertFactory, ITimeProvider timeProvider)
        {
            _videoRepository = videoRepository;
            _documentFactory = documentFactory;
            _httpContextAccessor = httpContextAccessor;
            _alertFactory = alertFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public News ToModel(ContentfulNews entry)
        {
            var documents = entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                           .Select(document => _documentFactory.ToModel(document)).ToList();
            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;

            var alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                      && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                      .Select(alert => _alertFactory.ToModel(alert));

            return new News(entry.Title, entry.Slug, entry.Teaser, imageUrl, ImageConverter.ConvertToThumbnail(imageUrl), 
                _videoRepository.Process(entry.Body), entry.SunriseDate, entry.SunsetDate, new List<Crumb> { new Crumb("News", string.Empty, "news") },
                alerts.ToList(), entry.Tags, documents, entry.Categories).StripData(_httpContextAccessor);
        }
    }
}