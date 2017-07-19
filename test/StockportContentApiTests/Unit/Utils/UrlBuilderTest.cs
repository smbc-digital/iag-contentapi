using StockportContentApi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace StockportContentApiTests.Unit.Utils
{
    

    public class UrlBuilderTest
    {
        private const string ENTRIES_BASE_URL = "https://test-host.com/spaces/XX/entries?access_token=XX";

        private readonly UrlBuilder _urlBuilder;

        public UrlBuilderTest()
        {
           _urlBuilder = new UrlBuilder(ENTRIES_BASE_URL);
        }

        [Fact]
        public void ShouldGetUrlForAtoZRepository()
        {
            _urlBuilder.UrlFor(type: "AtoZ", displayOnAtoZ:true).Should().Be(ENTRIES_BASE_URL + "&content_type=AtoZ&fields.displayOnAZ=true");
        }

        [Fact]
        public void ShouldGetUrlForHomeRepository()
        {
            _urlBuilder.UrlFor(type:"home", referenceLevel:2, slug:"slug").Should().Be(ENTRIES_BASE_URL + "&content_type=home&include=2&fields.slug=slug");
        }

        [Fact]
        public void ShouldGetUrlForStartPageRepository()
        {
            _urlBuilder.UrlFor(type: "startPage", referenceLevel: 2, slug: "slug").Should().Be(ENTRIES_BASE_URL + "&content_type=startPage&include=2&fields.slug=slug");
        }

        [Fact]
        public void ShouldGetUrlForNewsRepository()
        {           
            _urlBuilder.UrlFor(type: "news", referenceLevel: 2).Should().Be(ENTRIES_BASE_URL + "&content_type=news&include=2");
            _urlBuilder.UrlFor(type: "news", referenceLevel: 2, slug: "slug").Should().Be(ENTRIES_BASE_URL + "&content_type=news&include=2&fields.slug=slug");
            _urlBuilder.UrlFor(type: "news", referenceLevel: 2, slug: "slug", tag: "tag").Should().Be(ENTRIES_BASE_URL + "&content_type=news&include=2&fields.slug=slug&fields.tags[in]=tag");
            _urlBuilder.UrlFor(type: "news", referenceLevel: 2, slug: "slug", tag: "tag", limit: 100).Should().Be(ENTRIES_BASE_URL + "&content_type=news&include=2&fields.slug=slug&fields.tags[in]=tag&limit=100");
        }

        [Fact]
        public void ShouldGetUrlForRedirectRepository()
        {
            _urlBuilder.UrlFor(type: "redirect").Should().Be(ENTRIES_BASE_URL + "&content_type=redirect");
        }
    }
}
