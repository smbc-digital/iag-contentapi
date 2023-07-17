namespace StockportContentApiTests.Unit.ContentfulFactories;

public class CallToActionContentfulFactoryTest
{
    private readonly CallToActionContentfulFactory _factory= new CallToActionContentfulFactory();
    
    [Fact]
    public void ToModel_ShouldReturnNull_IfContentfulCallToActionModelIsNull()
    {
        // Act
        var model = _factory.ToModel(null);

        // Assert
        Assert.Null(model);
    }

    [Fact]
    public void ToModel_ShouldReturnCallToActionModel()
    {
        // Arrange
        var imageurl = new Asset(){
            File = new File(){
                Url = "imageUrl"
            }
        };

        var model = new ContentfulCallToAction("title", "text", new Link("url", "text", false), imageurl);

        // Act
        var result = _factory.ToModel(model);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("title", result.Title);
        Assert.Equal("text", result.Text);
        Assert.Equal("url", result.Link.Url);
        Assert.Equal("imageUrl", result.ImageUrl);
    }
}
