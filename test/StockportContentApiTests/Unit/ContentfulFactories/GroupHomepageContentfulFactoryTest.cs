using FluentAssertions;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Unit.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupHomepageContentfulFactoryTest
    {
        private readonly ContentfulGroupHomepage _contentfulGroupHomepage;
        private readonly GroupHomepageContentfulFactory _groupHomepageContentfulFactory;

        public GroupHomepageContentfulFactoryTest()
        {
            _contentfulGroupHomepage = new ContentfulGroupHomepageBuilder().Build();
            _groupHomepageContentfulFactory = new GroupHomepageContentfulFactory();
        }

        [Fact]
        public void ShouldReturnGroupHomepage()
        {
            var groupHomepage = _groupHomepageContentfulFactory.ToModel(_contentfulGroupHomepage);
            groupHomepage.ShouldBeEquivalentTo(_contentfulGroupHomepage, o=>o.Excluding(e => e.BackgroundImage));
            groupHomepage.BackgroundImage.Should().Be(_contentfulGroupHomepage.BackgroundImage.File.Url);
        }
        
    }
}
