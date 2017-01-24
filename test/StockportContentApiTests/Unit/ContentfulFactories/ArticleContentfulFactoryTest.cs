﻿using System;
using System.Collections.Generic;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using Xunit;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.Repositories;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ArticleContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAnArticleFromAContentfulArticle()
        {
            var contentfulArticle = new ContentfulArticleBuilder().Build();
            var videoRepository = new Mock<IVideoRepository>();
            const string processedBody = "this is processed body";
            videoRepository.Setup(o => o.Process(contentfulArticle.Body)).Returns(processedBody);
            var section = new Section("title", "slug", "body", new List<Profile>(), new List<Document>(), DateTime.MinValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime());
            var sectionFactory = new Mock<IContentfulFactory<ContentfulSection, Section>>();
            sectionFactory.Setup(o => o.ToModel(contentfulArticle.Sections.First())).Returns(section);
            var crumb = new Crumb("title", "slug", "type");
            var crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            crumbFactory.Setup(o => o.ToModel(contentfulArticle.Breadcrumbs.First())).Returns(crumb);
            var profile = new Profile("type", "title", "slug", "subtitle", "body", "icon", "image", new List<Crumb> { crumb });
            var profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            profileFactory.Setup(o => o.ToModel(contentfulArticle.Profiles.First())).Returns(profile);
            var subItems = new List<SubItem> { new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue) };
            var topic = new Topic("slug", "name", "teaser", "summary", "icon", "image", subItems, subItems, subItems, 
                new List<Crumb> {crumb}, 
                new List<Alert> { new Alert("title", "subHeading", "body", "severity", DateTime.MinValue, DateTime.MaxValue) }, 
                DateTime.MinValue, DateTime.MaxValue, false, "id");
            var topicFactory = new Mock<IContentfulFactory<ContentfulTopic, Topic>>();
            topicFactory.Setup(o => o.ToModel(contentfulArticle.ParentTopic)).Returns(topic);
            var documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            var document = new Document("title", 1000, DateTime.MinValue.ToUniversalTime(), "url", "fileName");
            documentFactory.Setup(o => o.ToModel(contentfulArticle.Documents.First())).Returns(document);

            var articleFactory = new ArticleContentfulFactory(sectionFactory.Object, crumbFactory.Object, profileFactory.Object, 
                topicFactory.Object, documentFactory.Object, videoRepository.Object);
            var article = articleFactory.ToModel(contentfulArticle);

            article.ShouldBeEquivalentTo(contentfulArticle, o => o.Excluding(e => e.BackgroundImage)
                                                                  .Excluding(e => e.Documents)
                                                                  .Excluding(e => e.Sections)
                                                                  .Excluding(e => e.Profiles)
                                                                  .Excluding(e => e.ParentTopic)
                                                                  .Excluding(e => e.Breadcrumbs)
                                                                  .Excluding(e => e.Body));

            videoRepository.Verify(o => o.Process(contentfulArticle.Body), Times.Once());
            article.Body.Should().Be(processedBody);
            article.BackgroundImage.Should().Be(contentfulArticle.BackgroundImage.File.Url);

            sectionFactory.Verify(o => o.ToModel(contentfulArticle.Sections.First()), Times.Once);
            article.Sections.First().ShouldBeEquivalentTo(section);

            crumbFactory.Verify(o => o.ToModel(contentfulArticle.Breadcrumbs.First()), Times.Once);
            article.Breadcrumbs.First().ShouldBeEquivalentTo(crumb);

            profileFactory.Verify(o => o.ToModel(contentfulArticle.Profiles.First()), Times.Once);
            article.Profiles.First().ShouldBeEquivalentTo(profile);

            topicFactory.Verify(o => o.ToModel(contentfulArticle.ParentTopic), Times.Once);
            article.ParentTopic.ShouldBeEquivalentTo(topic);

            documentFactory.Verify(o => o.ToModel(contentfulArticle.Documents.First()), Times.Once);
            article.Documents.Count.Should().Be(1);
            article.Documents.First().Should().Be(document);
        }
    }
}