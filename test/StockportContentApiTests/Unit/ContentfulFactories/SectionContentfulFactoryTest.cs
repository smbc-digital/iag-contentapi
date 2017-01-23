using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class SectionContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateASectionFromAContentfulSection()
        {
            var contentfulSection = new ContentfulSectionBuilder().Build();          
            var profile = new Profile("type", "title", "slug", "subtitle", "body", "icon", "image", new List<Crumb> { new Crumb("title", "slug", "type") });
            var profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            profileFactory.Setup(o => o.ToModel(contentfulSection.Profiles.First())).Returns(profile);
            var documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            var document = new Document("title", 1000, DateTime.MinValue.ToUniversalTime(), "url", "fileName");
            documentFactory.Setup(o => o.ToModel(contentfulSection.Documents.First())).Returns(document);

            var section = new SectionContentfulFactory(profileFactory.Object, documentFactory.Object).ToModel(contentfulSection);

            section.ShouldBeEquivalentTo(contentfulSection, o => o.Excluding(e => e.Profiles)
                                                                  .Excluding(e => e.Documents));
            profileFactory.Verify(o => o.ToModel(contentfulSection.Profiles.First()), Times.Once);
            section.Profiles.First().ShouldBeEquivalentTo(profile);

            documentFactory.Verify(o => o.ToModel(contentfulSection.Documents.First()), Times.Once);
            section.Documents.Count.Should().Be(1);
            section.Documents.First().Should().Be(document);    
        }
    }
}
