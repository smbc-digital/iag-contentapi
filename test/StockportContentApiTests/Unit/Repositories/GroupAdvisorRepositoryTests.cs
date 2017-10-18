using Xunit;
using FluentAssertions;
using StockportContentApi.Repositories;
using StockportContentApi.Config;
using StockportContentApi.Client;
using Moq;
using System.Net;
using StockportContentApi.Model;
using System.Collections.Generic;
using Contentful.Core;
using System.Threading;
using Contentful.Core.Search;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Unit.Repositories
{
    public class GroupAdvisorRepositoryTests
    {
        Mock<IContentfulClientManager> _contentfulClientManager;
        Mock<IContentfulClient> _client = new Mock<IContentfulClient>();
        GroupAdvisorRepository _reporisory;

        public GroupAdvisorRepositoryTests()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

            _reporisory = new GroupAdvisorRepository(config, _contentfulClientManager.Object);
        }

        [Fact]
        public void ShouldGetAdvisorsForAGroup()
        {
            // Arrange
            var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").Include(1).Build();
            var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
            collection.Items = new List<ContentfulGroupAdvisor>()
            {
                new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("test-group").Build() }).Build(),
                new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("test-group").Build() }).Build(),
                new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("not-a-group").Build() }).Build()
            };

            // Mock
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(collection);

            // Act
            var result = AsyncTestHelper.Resolve(_reporisory.GetAdvisorsByGroup("test-group"));
            var model = result.Get<IEnumerable<GroupAdvisor>>();

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            model.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldGetAdvisorsGroupByEmail()
        {
            // Arrange
            var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
            var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
            collection.Items = new List<ContentfulGroupAdvisor>()
            {
                new ContentfulGroupAdvisorBuilder().Email("not-email").Build(),
                new ContentfulGroupAdvisorBuilder().Build()
            };

            // Mock
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(collection);

            // Act
            var result = AsyncTestHelper.Resolve(_reporisory.Get("email"));
            var model = result.Get<IEnumerable<GroupAdvisor>>();

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            model.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldReturnTrueForIfGroupIsInListForEmail()
        {
            // Arrange
            var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
            var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
            collection.Items = new List<ContentfulGroupAdvisor>()
            {
                new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("test-group").Build() }).Build()
            };

            // Mock
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(collection);

            // Act
            var result = AsyncTestHelper.Resolve(_reporisory.CheckIfUserHasAccessToGroupBySlug("test-group", "email"));
            var model = result.Get<bool>();

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            model.Should().BeTrue();
        }

        [Fact]
        public void ShouldReturnFalseIfGroupIsntInListForEmail()
        {
            // Arrange
            var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
            var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
            collection.Items = new List<ContentfulGroupAdvisor>()
            {
                new ContentfulGroupAdvisorBuilder().ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("group").Build() }).Build()
            };

            // Mock
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(collection);

            // Act
            var result = AsyncTestHelper.Resolve(_reporisory.CheckIfUserHasAccessToGroupBySlug("test-group", "email"));
            var model = result.Get<bool>();

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            model.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrueIfUserHasGlobalAccessAndDoesntHaveGroupInList()
        {
            // Arrange
            var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
            var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
            collection.Items = new List<ContentfulGroupAdvisor>()
            {
                new ContentfulGroupAdvisorBuilder().GlobalAccess(true).ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("group").Build() }).Build()
            };

            // Mock
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(collection);

            // Act
            var result = AsyncTestHelper.Resolve(_reporisory.CheckIfUserHasAccessToGroupBySlug("test-group", "email"));
            var model = result.Get<bool>();

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            model.Should().BeFalse();
        }

        [Fact]
        public void ShouldReturnTrueIfUserHasGlobalAccessAndHasGroupInList()
        {
            // Arrange
            var query = new QueryBuilder<ContentfulGroupAdvisor>().ContentTypeIs("groupAdvisors").FieldEquals("fields.email", "email").Include(1).Build();
            var collection = new ContentfulCollection<ContentfulGroupAdvisor>();
            collection.Items = new List<ContentfulGroupAdvisor>()
            {
                new ContentfulGroupAdvisorBuilder().GlobalAccess(true).ContentfulReferences(new List<ContentfulReference> { new ContentfulReferenceBuilder().Slug("group").Build() }).Build()
            };

            // Mock
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulGroupAdvisor>>(q => q.Build() == query), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(collection);

            // Act
            var result = AsyncTestHelper.Resolve(_reporisory.CheckIfUserHasAccessToGroupBySlug("group", "email"));
            var model = result.Get<bool>();

            // Assert
            result.StatusCode.Should().Be((int)HttpStatusCode.OK);
            model.Should().BeFalse();
        }
    }
}
