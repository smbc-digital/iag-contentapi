using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class SectionListFactoryTest
    {
        private readonly SectionListFactory _sectionListFactory;
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly Mock<IBuildContentTypesFromReferences<Document>> _mockDocumentListFactory;
        private readonly List<Document> _documents = new List<Document>() { new Document("Title", 1212, DateTime.Now, "/thisisaurl", "filename1.pdf"),
                                                                            new Document("Title 2", 3412, DateTime.Now.AddHours(2), "/anotherurl", "filename2.pdf") };

        public SectionListFactoryTest()
        {
            _mockDocumentListFactory = new Mock<IBuildContentTypesFromReferences<Document>>();
            _mockDocumentListFactory.Setup(
                o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(_documents);

            _mockTimeProvider = new Mock<ITimeProvider>();
            _sectionListFactory = new SectionListFactory(new ProfileListFactory(), _mockDocumentListFactory.Object,_mockTimeProvider.Object);
        }

        [Fact]
        public void BuildsAListOfSectionsFromReferences()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 12, 01));

            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithSectionsAndSectionProfiles.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();

            List<Section> sections = _sectionListFactory.BuildFromReferences(article.fields.sections, contentfulResponse);

            sections.Count().Should().Be(2);
            var section = sections.FirstOrDefault();

            section.Title.Should().Be("Overview");
            section.Slug.Should().Be("slug");
            section.Body.Should().Be("body {{PROFILE:test-profile}}");
            section.Documents.Should().BeEquivalentTo(_documents);
        }

        [Fact]
        public void BuildsSectionThatHasAProfile()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 12, 01));

            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithSectionsAndSectionProfiles.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();

            List<Section> sections = _sectionListFactory.BuildFromReferences(article.fields.sections, contentfulResponse);

            sections.Count().Should().Be(2);
            var section = sections.FirstOrDefault();

            section.Profiles.Should().NotBeNullOrEmpty();
            var profile = section.Profiles.FirstOrDefault();

            profile.Type.Should().Be("Success Story");
            profile.Title.Should().Be("A profile");
            profile.Subtitle.Should().Be("This is a test profile");
            profile.Slug.Should().Be("test-profile");
            profile.Image.Should().Be("image.jpg");
        }

        [Fact]
        public void ReturnsValidSunsetAndSunriseDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 12, 01));

            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithSectionsAndSectionProfiles.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();

            List<Section> sections = _sectionListFactory.BuildFromReferences(article.fields.sections, contentfulResponse);

            sections.Count().Should().Be(2);
            var section = sections.FirstOrDefault();
            section.SunriseDate.Year.Should().Be(2015);
            section.SunsetDate.Year.Should().Be(2017);
        }

        [Fact]
        public void ReturnsNoSectionsForSunsetOutOfRange()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2014, 11, 01));

            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithSectionsAndSectionProfiles.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();

            List<Section> sections = _sectionListFactory.BuildFromReferences(article.fields.sections, contentfulResponse);

            sections.Count().Should().Be(0);
        }

        [Fact]
        public void ReturnsNoSectionsForSunriseOutOfRange()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2018, 01, 01));

            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithSectionsAndSectionProfiles.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();

            List<Section> sections = _sectionListFactory.BuildFromReferences(article.fields.sections, contentfulResponse);

            sections.Count().Should().Be(0);
        }

        [Fact]
        public void ReturnsSectionsForSunriseAndSunsetDateWhenBlank()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 12, 01));

            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithSectionsAndSectionProfilesWithBlankDate.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();

            List<Section> sections = _sectionListFactory.BuildFromReferences(article.fields.sections, contentfulResponse);

            sections.Count().Should().Be(2);
            var section = sections.FirstOrDefault();
            section.SunriseDate.Should().Be(DateTime.MinValue);
            section.SunsetDate.Should().Be(DateTime.MinValue);
        }
}
}
