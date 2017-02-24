using System;
using StockportContentApi.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class ArticleFactory : IFactory<Article>
    {
        private readonly IBuildContentTypesFromReferences<Alert> _alertListFactory;
        private readonly IFactory<Topic> _topicFactory;
        private readonly IBuildContentTypesFromReferences<Crumb> _breadcrumbFactory;
        private readonly IBuildContentTypesFromReferences<Section> _sectionListFactory;
        private readonly IBuildContentTypesFromReferences<Profile> _profileListFactory;
        private readonly IBuildContentTypesFromReferences<Document> _documentListFactory;
        private readonly IBuildContentTypeFromReference<LiveChat> _liveChatListFactory;
      
        public ArticleFactory(IFactory<Topic> topicFactory,
                              IBuildContentTypesFromReferences<Alert> alertListFactory, 
                              IBuildContentTypesFromReferences<Crumb> breadcrumbFactory, 
                              IBuildContentTypesFromReferences<Section> sectionListFactory, 
                              IBuildContentTypesFromReferences<Profile> profileListFactory, 
                              IBuildContentTypesFromReferences<Document> documentListFactory,
                              IBuildContentTypeFromReference<LiveChat>  liveChatListFactory)
        {
            _alertListFactory = alertListFactory;
            _topicFactory = topicFactory;
            _breadcrumbFactory = breadcrumbFactory;
            _sectionListFactory = sectionListFactory;
            _profileListFactory = profileListFactory;
            _documentListFactory = documentListFactory;
            _liveChatListFactory = liveChatListFactory;
        }

        public Article Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry == null || entry.fields == null)
                return null;

            var fields = entry.fields;

            var slug = MustNotBeNull(fields.slug);
            var title = MustNotBeNull(fields.title);
            var teaser = MustNotBeNull(fields.teaser);
            var body = MakeEmptyStringIfNull(fields.body);
            var icon = MakeEmptyStringIfNull(fields.icon);
            var backgroundImage = contentfulResponse.GetImageUrl(fields.backgroundImage);
            var image = contentfulResponse.GetImageUrl(fields.image);
            var sections = _sectionListFactory.BuildFromReferences(fields.sections, contentfulResponse);
            var breadcrumbs = _breadcrumbFactory.BuildFromReferences(fields.breadcrumbs, contentfulResponse);
            var alerts = _alertListFactory.BuildFromReferences(fields.alerts, contentfulResponse);
            var profiles = _profileListFactory.BuildFromReferences(fields.profiles, contentfulResponse);
            var documents = _documentListFactory.BuildFromReferences(fields.documents, contentfulResponse);

            var liveChatVisible = MakeFalseIfBooleanIsNull(fields.liveChatVisible);
            var liveChat = _liveChatListFactory.BuildFromReference(fields.liveChatText, contentfulResponse);

            DateTime sunriseDate = DateComparer.DateFieldToDate(fields.sunriseDate);
            DateTime sunsetDate = DateComparer.DateFieldToDate(fields.sunsetDate);
           
            // find the parent topic from the breadcrumbs (the last topic in the list)
            var parentTopicFromTheBreadcrumb = BuildParentTopic(fields.breadcrumbs, contentfulResponse);

            return new Article(body, slug, title, teaser, icon, backgroundImage, image, sections, breadcrumbs, alerts, 
                profiles, parentTopicFromTheBreadcrumb, documents,sunriseDate, sunsetDate, liveChatVisible, liveChat);
        }

        private Topic BuildParentTopic(dynamic references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new NullTopic();
            var breadcrumbEntries = contentfulResponse.GetEntriesFor(references);
            var parentTopicReference = GetLastTopicInBreadcrumbs(breadcrumbEntries);

            if (parentTopicReference == null) return new NullTopic();
            var parentTopicEntry = contentfulResponse.GetEntryFor(parentTopicReference);
            return _topicFactory.Build(parentTopicEntry, contentfulResponse);
        }

        private object GetLastTopicInBreadcrumbs(IEnumerable<dynamic> entries)
        {
            // convert the IEnumerable to a list to use linq
            var entriesList = entries.ToList();
            return entriesList.LastOrDefault(o => o.sys.contentType.sys.id.Value == "topic");
        }

        private string MustNotBeNull(dynamic field)
        {
            var value = (string) field;
            if (value == null)
            {
                throw new InvalidDataException("slug, title or teaser cannot be null");
            }
            return value;
        }

        private static string MakeEmptyStringIfNull(dynamic superBody)
        {
            return ((string)superBody) ?? string.Empty;
        }


        private static bool MakeFalseIfBooleanIsNull(dynamic field)
        {

            if (field == null)
                return false;
           
            return (bool) field;
           
        }
    }
};