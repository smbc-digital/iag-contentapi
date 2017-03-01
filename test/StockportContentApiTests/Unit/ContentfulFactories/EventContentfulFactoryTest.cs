﻿using System;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using System.Collections.Generic;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class EventContentfulFactoryTest
    {
        private readonly ContentfulEvent _contentfulEvent;
        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
        private readonly Mock<IContentfulFactory<ContentfulGroup, Group>> _groupFactory;
        private readonly EventContentfulFactory _eventContentfulFactory;
        private readonly List<Alert> _alerts = new List<Alert>() {
                new Alert("title", "subHeading", "body", "severity", new DateTime(0001, 1, 1), new DateTime(9999, 9, 9)) };

        public EventContentfulFactoryTest()
        {
            _contentfulEvent = new ContentfulEventBuilder().Build();

            _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            _groupFactory = new Mock<IContentfulFactory<ContentfulGroup, Group>>();
            _eventContentfulFactory = new EventContentfulFactory(_documentFactory.Object, _groupFactory.Object);
            
        }

        [Fact]
        public void ShouldCreateAnEventFromAContentfulEvent()
        {
            var document = new Document("title", 1000, DateTime.MinValue.ToUniversalTime(), "url", "fileName");
            _documentFactory.Setup(o => o.ToModel(_contentfulEvent.Documents.First())).Returns(document);

            var anEvent = _eventContentfulFactory.ToModel(_contentfulEvent);

            var mapPosition = new MapPosition() {Lat = 53.5, Lon = -2.5};

            anEvent.ShouldBeEquivalentTo(_contentfulEvent, o => o.Excluding(e => e.ImageUrl).Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Documents).Excluding(e => e.UpdatedAt).Excluding(e => e.Group));
            anEvent.ImageUrl.Should().Be(_contentfulEvent.Image.File.Url);
            anEvent.ThumbnailImageUrl.Should().Be(_contentfulEvent.Image.File.Url + "?h=250");
            anEvent.Documents.Count.Should().Be(1);
            anEvent.Documents.First().Should().Be(document);
            anEvent.MapPosition.Lat.Should().Be(mapPosition.Lat);
            anEvent.MapPosition.Lon.Should().Be(mapPosition.Lon);
            anEvent.Featured.Should().BeFalse();
            anEvent.Alerts.Count.Should().Be(1);
            anEvent.Alerts[0].Title.Should().Be(_alerts[0].Title);
            anEvent.Alerts[0].Body.Should().Be(_alerts[0].Body);
            anEvent.Alerts[0].Severity.Should().Be(_alerts[0].Severity);
            anEvent.Alerts[0].SubHeading.Should().Be(_alerts[0].SubHeading);
            anEvent.Alerts[0].SunsetDate.Should().Be(_alerts[0].SunsetDate);
            anEvent.Alerts[0].SunriseDate.Should().Be(_alerts[0].SunriseDate);
            _documentFactory.Verify(o => o.ToModel(_contentfulEvent.Documents.First()), Times.Once);
        }

        [Fact]
        public void ShouldNotAddDocumentsOrImageIfTheyAreLinks() 
        {
            _contentfulEvent.Documents.First().SystemProperties.Type = "Link";
            _contentfulEvent.Image.SystemProperties.Type = "Link";

            var anEvent = _eventContentfulFactory.ToModel(_contentfulEvent);

            anEvent.Documents.Count.Should().Be(0);
            anEvent.ImageUrl.Should().Be(string.Empty);
            anEvent.ThumbnailImageUrl.Should().Be(string.Empty);
        }

        [Fact]
        public void ShouldReturnGroupLinkedToEvent()
        {
            _groupFactory.Setup(o => o.ToModel(It.IsAny<ContentfulGroup>()))
                .Returns(new Group("Test Group", "test-group", null, null, null, null, null, null, null, null, null));

            var anEvent = _eventContentfulFactory.ToModel(_contentfulEvent);

            anEvent.Group.Name.Should().Be("Test Group");
        }
    }
}
