namespace StockportContentApiTests.Unit.ContentfulFactories;

public class AtoZContentfulFactoryTest
{

    [Fact]
    public void ShouldCreateAAtoZFromAContentfulReference()
    {
        var ContentfulReference =
            new ContentfulAToZBuilder().Build();

        var atoZ = new AtoZContentfulFactory().ToModel(ContentfulReference);

        atoZ.Slug.Should().Be(ContentfulReference.Slug);
        atoZ.Title.Should().Be(ContentfulReference.Title);
        atoZ.Type.Should().Be(ContentfulReference.Sys.ContentType.SystemProperties.Id);
    }
}
