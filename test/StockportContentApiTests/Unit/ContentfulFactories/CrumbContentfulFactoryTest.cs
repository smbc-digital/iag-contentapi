﻿namespace StockportContentApiTests.Unit.ContentfulFactories;

public class CrumbContentfulFactoryTest
{
    [Fact]
    public void ShouldCreateACrumbFromAContentfulReference()
    {
        var ContentfulReference =
            new ContentfulReferenceBuilder().Build();

        var crumb = new CrumbContentfulFactory().ToModel(ContentfulReference);

        crumb.Slug.Should().Be(ContentfulReference.Slug);
        crumb.Title.Should().Be(ContentfulReference.Title);
        crumb.Type.Should().Be(ContentfulReference.Sys.ContentType.SystemProperties.Id);
    }

    [Fact]
    public void ShouldCreateACrumbWithNameIfSet()
    {
        var ContentfulReference =
            new ContentfulReferenceBuilder().Name("name").Title(string.Empty).Build();

        var crumb = new CrumbContentfulFactory().ToModel(ContentfulReference);

        crumb.Title.Should().Be(ContentfulReference.Name);
    }
}
