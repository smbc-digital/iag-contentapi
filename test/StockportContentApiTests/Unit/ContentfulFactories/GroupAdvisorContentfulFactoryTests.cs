namespace StockportContentApiTests.Unit.ContentfulFactories;

public class GroupAdvisorContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateAGroupAdvisor()
    {
        // Arrange
        var builder = new ContentfulGroupAdvisorBuilder();
        var contentfulGroupAdvisor = builder.Build();
        var factory = new GroupAdvisorContentfulFactory();

        // Act
        var result = factory.ToModel(contentfulGroupAdvisor);

        // Assert
        result.Name.Should().Be(contentfulGroupAdvisor.Name);
    }
}
