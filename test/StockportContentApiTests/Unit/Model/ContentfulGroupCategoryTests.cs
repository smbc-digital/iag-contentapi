﻿namespace StockportContentApiTests.Unit.Model;

public class ContentfulGroupCategoryTests
{
    [Fact]
    public void ShouldSetDefaultsOnModel()
    {
        ContentfulGroupCategory actual = new();
        ContentfulGroupCategory expected = new()
        {
            Name = string.Empty,
            Slug = string.Empty,
            Icon = string.Empty,
            Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } }
        };
        actual.Should().BeEquivalentTo(expected);
    }
}
