namespace StockportContentApiTests.Unit.Utils;

public class ImageConverterTests
{
    [Theory]
    [InlineData("url", "url?h=250")]
    [InlineData("", "")]
    public void ConvertToThumbnail_ShouldReturnExpectedResult(string imageUrl, string expectedResult)
    {
        // Act
        var result = ImageConverter.ConvertToThumbnail(imageUrl);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("imageUrl", "thumbnailImageUrl", "thumbnailImageUrl")]
    [InlineData("imageUrl", "", "imageUrl")]
    [InlineData("", "", "")]
    public void SetThumbnailWithoutHeight_ShouldReturnExpectedResult(string imageUrl, string thumbnailImageUrl, string expectedResult)
    {
        // Act
        var result = ImageConverter.SetThumbnailWithoutHeight(imageUrl, thumbnailImageUrl);

        // Assert
        Assert.Equal(expectedResult, result);
    }
}
