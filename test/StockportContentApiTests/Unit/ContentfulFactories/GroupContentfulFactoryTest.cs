using System;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulFactories.GroupFactories;
using StockportContentApi.Fakes;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupContentfulFactoryTest
    {
        private readonly ContentfulGroup _contentfulGroup;
        private readonly GroupContentfulFactory _groupContentfulFactory;

        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
        private readonly Mock<IContentfulFactory<ContentfulOrganisation, Organisation>> _contentfulOrganisationFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _contentfulGroupCategoryFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>> _contentfulGroupSubCategoryFactory;
        private Mock<ITimeProvider> _timeProvider;

        public GroupContentfulFactoryTest()
        {
            _timeProvider = new Mock<ITimeProvider>();

            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));
            _contentfulOrganisationFactory = new Mock<IContentfulFactory<ContentfulOrganisation, Organisation>>();
            _contentfulGroupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
            _contentfulGroupSubCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>();
            _contentfulGroup = new ContentfulGroupBuilder().Build();
         
            _groupContentfulFactory = new GroupContentfulFactory(_contentfulOrganisationFactory.Object, _contentfulGroupCategoryFactory.Object, _contentfulGroupSubCategoryFactory.Object, _timeProvider.Object, HttpContextFake.GetHttpContextFake(), _documentFactory.Object);
        }

        [Fact(Skip = "Fluent Assertions update")]
        public void ShouldCreateAGroupFromAContentfulGroup()
        {
            var group = _groupContentfulFactory.ToModel(_contentfulGroup);
            group.Should().BeEquivalentTo(_contentfulGroup, o => o.ExcludingMissingMembers());
        }
    }
}
