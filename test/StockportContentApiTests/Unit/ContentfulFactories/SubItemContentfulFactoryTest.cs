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

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class SubItemContentfulFactoryTest
    {
        private readonly SubItemContentfulFactory _subItemContentfulFactory;

        public SubItemContentfulFactoryTest()
        {
            var timeProvider = new Mock<ITimeProvider>();
            timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

            _subItemContentfulFactory = new SubItemContentfulFactory(timeProvider.Object);
        }

        [Fact]
        public void ShouldCreateASubItemFromAContentfulSubItem()
        {
            var contentfulSubItem =
                new ContentfulSubItem
                {
                    Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
                };

            var subItem = _subItemContentfulFactory.ToModel(contentfulSubItem);

            subItem.Slug.Should().Be(contentfulSubItem.Slug);
            subItem.Title.Should().Be(contentfulSubItem.Title);
            subItem.Icon.Should().Be(contentfulSubItem.Icon);
            subItem.Teaser.Should().Be(contentfulSubItem.Teaser);
            subItem.SunriseDate.Should().Be(contentfulSubItem.SunriseDate);
            subItem.SunsetDate.Should().Be(contentfulSubItem.SunsetDate);
            subItem.Type.Should().Be(contentfulSubItem.Sys.ContentType.SystemProperties.Id);
        }

        // TODO: remove start page inconsistency
        [Fact]
        public void ShouldSetStartPageToADifferentIdThanProvided()
        {
            var contentfulSubItem =
                new ContentfulSubItem
                {
                    Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
                };

            var subItem = _subItemContentfulFactory.ToModel(contentfulSubItem);

            subItem.Type.Should().Be("start-page");
        }

        [Fact]
        public void ShouldCreateSubItemWithNameForTitleWhenNoTitleProvided()
        {
            var contentfulSubItem =
                new ContentfulSubItem
                {
                    Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
                };

            var subItem = _subItemContentfulFactory.ToModel(contentfulSubItem);

            subItem.Title.Should().Be(contentfulSubItem.Name);

        }

        [Fact]
        public void ShouldCreateSubItemWithoutSubItems()
        {
            var contentfulSubItem = new ContentfulSubItemBuilder()
                .Name("custom name")
                .Title(string.Empty)
                .SubItems(null)
                .TertiaryItems(null)
                .SecondaryItems(null)
                .SystemContentTypeId("topic")
                .Build();

            var subItem = _subItemContentfulFactory.ToModel(contentfulSubItem);

            subItem.Should().NotBeNull();
            subItem.Should().BeOfType<SubItem>();
            subItem.Title.Should().Be("custom name");
            subItem.SubItems.Should().BeEmpty();
        }

        [Fact]
        public void ShouldCreateSubItemWithSubItems()
        {
            var contentfulSubItem = new ContentfulSubItemBuilder()
                .SubItems(new List<IContentfulSubItem>()
                {
                    new ContentfulSubItem()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    },
                    new ContentfulSubItem()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    }
                })
                .TertiaryItems(new List<IContentfulSubItem>()
                {
                    new ContentfulSubItem()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    },
                    new ContentfulSubItem()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    }
                })
                .SecondaryItems(new List<IContentfulSubItem>()
                {
                    new ContentfulSubItem()
                    {
                        Sys =
                            new SystemProperties()
                            {
                                Type = "Entry",
                                ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}
                            }
                    },
                    new ContentfulSubItem()
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

            var subItem = _subItemContentfulFactory.ToModel(contentfulSubItem);

            subItem.SubItems.Should().HaveCount(6);
        }
    }
}
