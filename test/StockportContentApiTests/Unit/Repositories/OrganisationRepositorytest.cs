using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;
using StockportContentApiTests.Unit.Builders;
using Microsoft.AspNetCore.Http;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.Repositories
{
    public class OrganisationRepositoryTest
    {
        private readonly OrganisationRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;

        public OrganisationRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            var contentfulFactory = new OrganisationContentfulFactory(HttpContextFake.GetHttpContextFake());

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            var _groupRepository = new Mock<IGroupRepository>();
            var groups = new List<Group>();
            var organisation = new Organisation() { Slug = "slug", Title = "Title" };
            groups.Add(new GroupBuilder().Organisation(organisation).Build());
            groups.Add(new GroupBuilder().Organisation(organisation).Build());
            groups.Add(new GroupBuilder().Organisation(organisation).Build());
            _groupRepository.Setup(o => o.GetLinkedGroupsByOrganisation(It.IsAny<string>())).ReturnsAsync(groups);

            _repository = new OrganisationRepository
            (
                config,
                contentfulFactory,
                contentfulClientManager.Object,
                _groupRepository.Object
            );
        }

        [Fact]
        public void ShouldGetOrganisation()
        {
            // Arrange          
            var contentfulOrganisation = new ContentfulOrganisation()
            {
                AboutUs = "about us",
                Email = "Email",
                Phone = "Phone",
                Slug = "slug",
                Title = "title",
                Volunteering = true,
                VolunteeringText = "test"
            };

            var builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", "slug");
            var contentfulCollection = new ContentfulCollection<ContentfulOrganisation>()
            {
                Items = new List<ContentfulOrganisation> { contentfulOrganisation }
            };

            _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulOrganisation>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(contentfulCollection);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetOrganisation("slug"));
            var organisation = response.Get<Organisation>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            contentfulOrganisation.Should().BeEquivalentTo(organisation, o => o.ExcludingNestedObjects());
        }

        [Fact]
        public void ShouldReturn404ForNonExistentSlug()
        {
            // Arrange
            const string slug = "invalid-url";

            var collection = new ContentfulCollection<ContentfulOrganisation>();
            collection.Items = new List<ContentfulOrganisation>();

            var builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", slug);
            _contentfulClient.Setup(o => o.GetEntries(
                It.IsAny<QueryBuilder<ContentfulOrganisation>>(),
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetOrganisation(slug));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}

