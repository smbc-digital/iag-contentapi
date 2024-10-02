namespace StockportContentApiTests.Unit.ContentfulFactories;

public class GroupAdvisorContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateAGroupAdvisor()
    {
        // Arrange
        ContentfulGroupAdvisorBuilder builder = new();
        ContentfulGroupAdvisor contentfulGroupAdvisor = builder.Build();
        GroupAdvisorContentfulFactory factory = new();

        // Act
        GroupAdvisor result = factory.ToModel(contentfulGroupAdvisor);

        // Assert
        result.Name.Should().Be(contentfulGroupAdvisor.Name);
    }
}
