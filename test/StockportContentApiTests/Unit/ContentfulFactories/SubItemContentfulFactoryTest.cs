using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class SubItemContentfulFactoryTest
    {
        private readonly SubItemContentfulFactory _subItemContentfulFactory;

        public SubItemContentfulFactoryTest()
        {
            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _subItemContentfulFactory = new SubItemContentfulFactory(timeProvider.Object, HttpContextFake.GetHttpContextFake());
        }

        [Fact]
        public void ShouldCreateASubItemFromAContentfulReference()
        {
            var ContentfulReference =
                new ContentfulReference
                {
                    Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
                };

            var subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

            subItem.Slug.Should().Be(ContentfulReference.Slug);
            subItem.Title.Should().Be(ContentfulReference.Title);
            subItem.Icon.Should().Be(ContentfulReference.Icon);
            subItem.Teaser.Should().Be(ContentfulReference.Teaser);
            subItem.SunriseDate.Should().Be(ContentfulReference.SunriseDate);
            subItem.SunsetDate.Should().Be(ContentfulReference.SunsetDate);
            subItem.Type.Should().Be(ContentfulReference.Sys.ContentType.SystemProperties.Id);
        }

        // TODO: remove start page inconsistency
        [Fact]
        public void ShouldSetStartPageToADifferentIdThanProvided()
        {
            var ContentfulReference =
                new ContentfulReference
                {
                    Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
                };

            var subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

            subItem.Type.Should().Be("start-page");
        }

        [Fact]
        public void ShouldCreateSubItemWithNameForTitleWhenNoTitleProvided()
        {
            var ContentfulReference =
                new ContentfulReference
                {
                    Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
                };

            var subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

            subItem.Title.Should().Be(ContentfulReference.Name);

        }

        [Fact]
        public void ShouldCreateSubItemWithDefaultIconIfNotSet()
        {
            var ContentfulReference =
                new ContentfulReference
                {
                    Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
                };

            var subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

            subItem.Icon.Should().Be("si-default");
        }


        [Fact]
        public void ShouldCreateSubItemWithIcon()
        {
            var ContentfulReference =
                new ContentfulReference
                {
                    Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } },
                    Icon = "fa-unique"
                };

            var subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

            subItem.Icon.Should().Be("fa-unique");
        }

        [Fact]
        public void ShouldCreateSubItemWithoutSubItems()
        {
            var ContentfulReference = new ContentfulReferenceBuilder()
                .Name("custom name")
                .Title(string.Empty)
                .SubItems(null)
                .TertiaryItems(null)
                .SecondaryItems(null)
                .SystemContentTypeId("topic")
                .Build();

            var subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

            subItem.Should().NotBeNull();
            subItem.Should().BeOfType<SubItem>();
            subItem.Title.Should().Be("custom name");
            subItem.SubItems.Should().BeEmpty();
        }

        [Fact]
        public void ShouldCreateSubItemWithSubItems()
        {
            var ContentfulReference = new ContentfulReferenceBuilder()
                .SubItems(new List<ContentfulReference>()
                {
                    new ContentfulReference()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    },
                    new ContentfulReference()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    }
                })
                .TertiaryItems(new List<ContentfulReference>()
                {
                    new ContentfulReference()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    },
                    new ContentfulReference()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    }
                })
                .SecondaryItems(new List<ContentfulReference>()
                {
                    new ContentfulReference()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    },
                    new ContentfulReference()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    }
                })
                .Build();

            var subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

            subItem.SubItems.Should().HaveCount(6);
        }

        [Fact]
        public void ToModel_ShouldHandleGroupsHomepageSlugCorrectly()
        {
            // Arrange
            var contentfulReference = new ContentfulReferenceBuilder()
                .Slug("test-group-homepage")
                .Name("custom name")
                .Title(string.Empty)
                .SubItems(null)
                .TertiaryItems(null)
                .SecondaryItems(null)
                .SystemContentTypeId("groupHomepage")
                .Build();

            // Act
            var subItem = _subItemContentfulFactory.ToModel(contentfulReference);

            // Assert
            subItem.Slug.Should().Be("groups");
        }

        [Fact]
        public void ToModel_ShouldSetPaymentsGroupIconCorrectly_WhenNonSet()
        {
            // Arrange
            var contentfulReference = new ContentfulReferenceBuilder()
                .Slug("test-payment-group")
                .Name("custom name")
                .Title("title")
                .SubItems(null)
                .TertiaryItems(null)
                .SecondaryItems(null)
                .Icon(null)
                .SystemContentTypeId("payment")
                .Build();

            // Act
            var subItem = _subItemContentfulFactory.ToModel(contentfulReference);

            // Assert
            subItem.Icon.Should().Be("fa fa-gbp");
        }
    }
}
