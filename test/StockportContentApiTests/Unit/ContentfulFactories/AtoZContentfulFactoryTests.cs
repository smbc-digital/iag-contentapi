namespace StockportContentApiTests.Unit.ContentfulFactories;

public class AtoZContentfulFactoryTests
{

    [Fact]
    public void ShouldCreateAtoZFromAContentfulReference()
    {
        ContentfulAtoZ ContentfulReference =
            new ContentfulAToZBuilder().Build();

        AtoZ atoZ = new AtoZContentfulFactory().ToModel(ContentfulReference);

        atoZ.Slug.Should().Be(ContentfulReference.Slug);
        atoZ.Title.Should().Be(ContentfulReference.Title);
        atoZ.Type.Should().Be(ContentfulReference.Sys.ContentType.SystemProperties.Id);
    }
}