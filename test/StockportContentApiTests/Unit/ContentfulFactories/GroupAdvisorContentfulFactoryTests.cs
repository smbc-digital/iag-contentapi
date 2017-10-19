using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Fakes;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupAdvisorContentfulFactoryTests
    {
        [Fact]
        public void ToModel_ShouldCreateAGroupAdvisor()
        {
            // Arrange
            var builder = new ContentfulGroupAdvisorBuilder();
            var contentfulGroupAdvisor = builder.Build();
            var factory = new GroupAdvisorContentfulFactory(HttpContextFake.GetHttpContextFake());

            // Act
            var result = factory.ToModel(contentfulGroupAdvisor);

            // Assert
            result.Name.Should().Be(contentfulGroupAdvisor.Name);
        }
    }
}
