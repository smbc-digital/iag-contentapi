using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;
using Moq;
using StockportContentApi.Model;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class FooterContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAFooterFromAContentfulReference()
        {
            var factory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
            factory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));
            var socialMediaFactory = new Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>();

            socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url"));

            var ContentfulReference =
                new ContentfulFooterBuilder().Build();                    
 
            var footer = new FooterContentfulFactory(factory.Object, socialMediaFactory.Object, HttpContextFake.GetHttpContextFake()).ToModel(ContentfulReference);

            footer.Slug.Should().Be(ContentfulReference.Slug);
            footer.Title.Should().Be(ContentfulReference.Title);
        }      
    }
}
