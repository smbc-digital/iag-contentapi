﻿namespace StockportContentApiTests.Unit.Model;

public class ContentfulGroupTest
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        var actual = new ContentfulGroup();
        var expected = new ContentfulGroup
        {
            Name = string.Empty,
            Slug = string.Empty,
            PhoneNumber = string.Empty,
            Email = string.Empty,
            Website = string.Empty,
            Twitter = string.Empty,
            Facebook = string.Empty,
            Address = string.Empty,
            Description = string.Empty,
            Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
            MapPosition = new MapPosition(),
            Volunteering = false
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
