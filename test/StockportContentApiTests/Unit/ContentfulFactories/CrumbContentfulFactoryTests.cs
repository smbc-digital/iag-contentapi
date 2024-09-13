namespace StockportContentApiTests.Unit.ContentfulFactories;

public class CrumbContentfulFactoryTests
{
    [Fact]
    public void ShouldCreateACrumbFromAContentfulReference()
    {
        ContentfulReference ContentfulReference = new ContentfulReferenceBuilder().Build();

        Crumb crumb = new CrumbContentfulFactory().ToModel(ContentfulReference);

        crumb.Slug.Should().Be(ContentfulReference.Slug);
        crumb.Title.Should().Be(ContentfulReference.Title);
        crumb.Type.Should().Be(ContentfulReference.Sys.ContentType.SystemProperties.Id);
    }

    [Fact]
    public void ShouldCreateACrumbWithNameIfSet()
    {
        ContentfulReference ContentfulReference = new ContentfulReferenceBuilder().Name("name").Title(string.Empty).Build();

        Crumb crumb = new CrumbContentfulFactory().ToModel(ContentfulReference);

        crumb.Title.Should().Be(ContentfulReference.Name);
    }
}