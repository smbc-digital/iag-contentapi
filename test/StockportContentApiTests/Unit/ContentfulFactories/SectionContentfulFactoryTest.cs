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
using StockportContentApi.Utils;
using StockportContentApi.Fakes;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class SectionContentfulFactoryTest
    {
        private readonly ContentfulSection _contentfulSection;
        private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;
        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
        private readonly Mock<IVideoRepository> _videoRepository;
        private readonly SectionContentfulFactory _sectionFactory;
        private readonly Mock<ITimeProvider> _timeProvider;
        private Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;

        public SectionContentfulFactoryTest()
        {
            _contentfulSection = new ContentfulSectionBuilder().Build();
            _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            _videoRepository = new Mock<IVideoRepository>();
            _timeProvider = new Mock<ITimeProvider>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();

            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _sectionFactory = new SectionContentfulFactory(_profileFactory.Object, _documentFactory.Object,
                _videoRepository.Object, _timeProvider.Object, _alertFactory.Object, HttpContextFake.GetHttpContextFake());
        }

        [Fact]
        public void ShouldCreateASectionFromAContentfulSection()
        {
            // Arrange
            var profile = new Profile("type",
                "title",
                "slug",
                "subtitle",
                "body",
                "icon",
                "image",
                new List<Crumb>
                {
                    new Crumb("title", "slug", "type")
                },
                "",
                "");
            _profileFactory.Setup(o => o.ToModel(_contentfulSection.Profiles.First())).Returns(profile);

            var document = new DocumentBuilder().Build();
            _documentFactory.Setup(o => o.ToModel(_contentfulSection.Documents.First())).Returns(document);

            const string processedBody = "this is processed body";
            _videoRepository.Setup(o => o.Process(_contentfulSection.Body)).Returns(processedBody);

            var alert = new Alert("title", "subHeading", "body", "severity", DateTime.MinValue, DateTime.MinValue, "slug");
            _alertFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulAlert>())).Returns(alert);

            // Act
            var result = _sectionFactory.ToModel(_contentfulSection);

            // Assert
            result.AlertsInline.Count().Should().Be(1);
            result.AlertsInline.First().Should().BeEquivalentTo(alert);
            result.Body.Should().BeEquivalentTo("this is processed body");
            result.Documents.Count().Should().Be(1);
            result.Documents.First().Should().BeEquivalentTo(document);
            result.Profiles.Count().Should().Be(1);
            result.Profiles.First().Should().BeEquivalentTo(profile);
            result.Slug.Should().Be("slug");
            result.SunriseDate.Should().Be(DateTime.MinValue);
            result.SunsetDate.Should().Be(DateTime.MinValue);
            result.Title.Should().Be("title");
        }

        [Fact]
        public void ShouldNotAddDocumentsOrProfilesIfTheyAreLinks()
        {
            _contentfulSection.Documents.First().SystemProperties.Type = "Link";
            _contentfulSection.Profiles.First().Sys.Type = "Link";
            _videoRepository.Setup(o => o.Process(_contentfulSection.Body)).Returns(_contentfulSection.Body);

            var section = _sectionFactory.ToModel(_contentfulSection);

            section.Documents.Count().Should().Be(0);
            _documentFactory.Verify(o => o.ToModel(It.IsAny<Asset>()), Times.Never);
            section.Profiles.Count().Should().Be(0);
            _profileFactory.Verify(o => o.ToModel(It.IsAny<ContentfulProfile>()), Times.Never);
        }
    }
}
