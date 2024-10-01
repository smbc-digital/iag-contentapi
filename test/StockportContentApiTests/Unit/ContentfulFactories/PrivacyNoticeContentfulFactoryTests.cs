namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PrivacyNoticeContentfulFactoryTests
{
    private readonly PrivacyNoticeContentfulFactory _privacyNoticeContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _mockCrumbFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulPrivacyNotice, Topic>> _parentTopicFactory = new();

    public PrivacyNoticeContentfulFactoryTests() => _privacyNoticeContentfulFactory = new PrivacyNoticeContentfulFactory(_mockCrumbFactory.Object, _parentTopicFactory.Object);

    [Fact]
    public void ToModel_ShouldReturnPrivacyNotice()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new();

        // Act
        PrivacyNotice privacyNotice = _privacyNoticeContentfulFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<PrivacyNotice>(privacyNotice);
    }

    [Fact]
    public void ToModel_ShouldConvertContentfulPrivacyNoticeToPrivacyNotice()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Slug = "test-slug",
            Title = "test-title",
            Category = "test-category",
            Purpose = "test-purpose",
            TypeOfData = "test-type-of-data",
            Legislation = "test-legislation",
            Obtained = "test-obtained",
            ExternallyShared = "test-externally-shared",
            RetentionPeriod = "test-retention-period",
            OutsideEu = false,
            AutomatedDecision = false,
            Breadcrumbs = new List<ContentfulReference>()
        };

        // Act
        PrivacyNotice privacyNotice = _privacyNoticeContentfulFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        privacyNotice
            .Should()
            .BeEquivalentTo(contentfulPrivacyNotice, p => p
            .ExcludingMissingMembers());
    }

    [Fact]
    public void ToModel_ShouldNotAddLinks()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Slug = "test-slug",
            Title = "test-title",
            Category = "test-category",
            Purpose = "test-purpose",
            TypeOfData = "test-type-of-data",
            Legislation = "test-legislation",
            Obtained = "test-obtained",
            ExternallyShared = "test-externally-shared",
            RetentionPeriod = "test-retention-period",
            OutsideEu = false,
            AutomatedDecision = false,
            Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().Build() },
        };

        contentfulPrivacyNotice.Image.SystemProperties.LinkType = "Link";
        contentfulPrivacyNotice.Breadcrumbs.First().Sys.LinkType = "Link";

        // Act
        PrivacyNotice privacyNotice = _privacyNoticeContentfulFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.Empty(privacyNotice.Breadcrumbs);
        _mockCrumbFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
        _parentTopicFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulPrivacyNotice>()), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldReturnNull()
    {
        // Act
        PrivacyNotice privacyNotice = _privacyNoticeContentfulFactory.ToModel(null);

        // Assert
        Assert.Null(privacyNotice);
    }
}