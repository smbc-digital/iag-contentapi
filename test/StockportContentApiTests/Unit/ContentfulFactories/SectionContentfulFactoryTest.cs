using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class SectionContentfulFactoryTest
    {
        private readonly ContentfulSection _contentfulSection;
        private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;
        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
        private readonly Mock<IVideoRepository> _videoRepository;
        private readonly SectionContentfulFactory _sectionFactory;

        public SectionContentfulFactoryTest()
        {
            _contentfulSection = new ContentfulSectionBuilder().Build();
            _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            _videoRepository = new Mock<IVideoRepository>();
            _sectionFactory = new SectionContentfulFactory(_profileFactory.Object, _documentFactory.Object,
                _videoRepository.Object);
        }

        [Fact]
        public void ShouldCreateASectionFromAContentfulSection()
        {
            var profile = new Profile("type", "title", "slug", "subtitle", "body", "icon", "image", new List<Crumb> { new Crumb("title", "slug", "type") });
            _profileFactory.Setup(o => o.ToModel(_contentfulSection.Profiles.First().Fields)).Returns(profile);
            var document = new Document("title", 1000, DateTime.MinValue.ToUniversalTime(), "url", "fileName");
            _documentFactory.Setup(o => o.ToModel(_contentfulSection.Documents.First())).Returns(document);
            const string processedBody = "this is processed body";
            _videoRepository.Setup(o => o.Process(_contentfulSection.Body)).Returns(processedBody);

            var section = _sectionFactory.ToModel(_contentfulSection);

            section.ShouldBeEquivalentTo(_contentfulSection, o => o.Excluding(e => e.Profiles)
                                                                  .Excluding(e => e.Documents)
                                                                  .Excluding(e => e.Body));

            _videoRepository.Verify(o => o.Process(_contentfulSection.Body), Times.Once());
            section.Body.Should().Be(processedBody);
            _profileFactory.Verify(o => o.ToModel(_contentfulSection.Profiles.First().Fields), Times.Once);
            section.Profiles.First().ShouldBeEquivalentTo(profile);
            _documentFactory.Verify(o => o.ToModel(_contentfulSection.Documents.First()), Times.Once);
            section.Documents.Count.Should().Be(1);
            section.Documents.First().Should().Be(document);    
        }

        [Fact]
        public void ShouldNotAddDocumentsOrProfilesIfTheyAreLinks()
        {
            _contentfulSection.Documents.First().SystemProperties.Type = "Link";
            _contentfulSection.Profiles.First().SystemProperties.Type = "Link";
            _videoRepository.Setup(o => o.Process(_contentfulSection.Body)).Returns(_contentfulSection.Body);

            var section = _sectionFactory.ToModel(_contentfulSection);

            section.Documents.Count().Should().Be(0);
            _documentFactory.Verify(o => o.ToModel(It.IsAny<Asset>()), Times.Never);
            section.Profiles.Count().Should().Be(0);
            _profileFactory.Verify(o => o.ToModel(It.IsAny<ContentfulProfile>()), Times.Never);
        }

    }
}
