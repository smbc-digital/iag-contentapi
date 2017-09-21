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
using Microsoft.AspNetCore.Http;
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
    }
}
