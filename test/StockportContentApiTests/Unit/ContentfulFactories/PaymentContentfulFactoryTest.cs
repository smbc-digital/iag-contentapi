﻿using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;
using StockportContentApiTests.Unit.Repositories;
using Moq;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class PaymentContentfulFactoryTest
    {
        private Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>> _crumbFactory;

        [Fact]
        public void ShouldCreateAPaymentFromAContentfulPayment()
        {
            var contentfulPayment = new ContentfulPaymentBuilder()
                .Slug("payment-slug")
                .Title("payment title")
                .Teaser("payment teaser")
                .ReferenceLabel("reference label")
                .Build();

            _crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            var contentfulFactory = new PaymentContentfulFactory(_crumbFactory.Object);

            var payment = contentfulFactory.ToModel(contentfulPayment);

            payment.Slug.Should().Be("payment-slug");
            payment.Title.Should().Be("payment title");
            payment.Teaser.Should().Be("payment teaser");
            payment.ReferenceLabel.Should().Be("reference label");
        }
    }
}
