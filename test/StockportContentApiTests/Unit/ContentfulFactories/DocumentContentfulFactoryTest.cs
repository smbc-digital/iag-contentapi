﻿using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class DocumentContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateADocumentFromAContentfulDocument()
        {
            var contentfulDocument = new ContentfulDocumentBuilder().Build();

            var document = new DocumentContentfulFactory(HttpContextFake.GetHttpContextFake()).ToModel(contentfulDocument);

            document.FileName.Should().Be(contentfulDocument.File.FileName);
            document.Title.Should().Be(contentfulDocument.Description);
            document.LastUpdated.Should().Be(contentfulDocument.SystemProperties.UpdatedAt.Value);
            document.Size.Should().Be((int)contentfulDocument.File.Details.Size);
            document.Url.Should().Be(contentfulDocument.File.Url);
        }
    }
}
