namespace StockportContentApiTests.Config;

public class ContentfulConfigTest
{
    [Fact]
    public void BuildsContentfulUrlForBusinessIdBasedOnEnvironmentVariables()
    {
        var key = "KEY";
        var space = "SPACE";

        var config = new ContentfulConfig("testid")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TESTID_ACCESS_KEY", key)
            .Add("TESTID_MANAGEMENT_KEY", "KEY")
            .Add("TESTID_SPACE", space)
            .Build();
        var expectedResult = "https://fake.url/spaces/SPACE/entries?access_token=KEY";

        config.AccessKey.Should().Be(key);
        config.SpaceKey.Should().Be(space);

        Assert.Equal(config.ContentfulUrl.ToString(), expectedResult);
    }

    [Fact]
    public void FailsIfDeliveryUrlIsMissing()
    {
        Exception exception = Assert.Throws<ArgumentException>(() => new ContentfulConfig("testid")
            .Add("TESTID_ACCESS_KEY", "KEY")
            .Add("TESTID_MANAGEMENT_KEY", "KEY")
            .Add("TESTID_SPACE", "SPACE")
            .Build());

        Assert.Equal("No value found for 'DELIVERY_URL' in the contentful config.", exception.Message);
    }

    [Fact]
    public void FailsIfSpaceIsMissing()
    {
        Exception exception = Assert.Throws<ArgumentException>(() => new ContentfulConfig("testid")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TESTID_ACCESS_KEY", "KEY")
            .Build());

        Assert.Equal("No value found for 'TESTID_SPACE' in the contentful config.", exception.Message);
    }

    [Fact]
    public void FailsIfAccessKeyIsMissing()
    {
        Exception exception = Assert.Throws<ArgumentException>(() => new ContentfulConfig("testid")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TESTID_SPACE", "SPACE")
            .Build());

        Assert.Equal("No value found for 'TESTID_ACCESS_KEY' in the contentful config.", exception.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FailsIfBusinessIdlIsMissing(string value)
    {
        try
        {
            new ContentfulConfig(value).Build();
            Assert.True(false, "");
        }
        catch (ArgumentException e)
        {
            Assert.Equal("'BUSINESS_ID' cannot be null or empty.", e.Message);
        }
    }
}