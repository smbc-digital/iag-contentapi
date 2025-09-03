namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PrivacyNoticeContentfulFactoryTests
{
    private readonly PrivacyNoticeContentfulFactory _privacyNoticeFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulPrivacyNotice, Topic>> _parentTopicFactory = new();

    public PrivacyNoticeContentfulFactoryTests() => _privacyNoticeFactory = new PrivacyNoticeContentfulFactory(_crumbFactory.Object, _parentTopicFactory.Object);

    [Fact]
    public void ToModel_ShouldReturnPrivacyNotice()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new();

        // Act
        PrivacyNotice privacyNotice = _privacyNoticeFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<PrivacyNotice>(privacyNotice);
    }

    [Fact]
    public void ToModel_ShouldConvertContentfulPrivacyNoticeToPrivacyNotice()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new ContentfulPrivacyNoticeBuilder().Build();

        // Act
        PrivacyNotice privacyNotice = _privacyNoticeFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.Equal(contentfulPrivacyNotice.Slug, privacyNotice.Slug);
        Assert.Equal(contentfulPrivacyNotice.Title, privacyNotice.Title);
        Assert.Equal(contentfulPrivacyNotice.Category, privacyNotice.Category);
        Assert.Equal(contentfulPrivacyNotice.Purpose, privacyNotice.Purpose);
        Assert.Equal(contentfulPrivacyNotice.TypeOfData, privacyNotice.TypeOfData);
        Assert.Equal(contentfulPrivacyNotice.Legislation, privacyNotice.Legislation);
        Assert.Equal(contentfulPrivacyNotice.Obtained, privacyNotice.Obtained);
        Assert.Equal(contentfulPrivacyNotice.ExternallyShared, privacyNotice.ExternallyShared);
        Assert.Equal(contentfulPrivacyNotice.RetentionPeriod, privacyNotice.RetentionPeriod);
        Assert.Equal(contentfulPrivacyNotice.OutsideEu, privacyNotice.OutsideEu);
        Assert.Equal(contentfulPrivacyNotice.AutomatedDecision, privacyNotice.AutomatedDecision);
    }

    [Fact]
    public void ToModel_ShouldNotAddLinks()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new ContentfulPrivacyNoticeBuilder().Build();
        contentfulPrivacyNotice.Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().Build() };
        contentfulPrivacyNotice.Image.SystemProperties.LinkType = "Link";
        contentfulPrivacyNotice.Breadcrumbs.First().Sys.LinkType = "Link";

        // Act
        PrivacyNotice privacyNotice = _privacyNoticeFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.Empty(privacyNotice.Breadcrumbs);
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
        _parentTopicFactory.Verify(parentTopicFactory => parentTopicFactory.ToModel(It.IsAny<ContentfulPrivacyNotice>()), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldReturnNull()
    {
        // Act & Assert
        Assert.Null(_privacyNoticeFactory.ToModel(null));
    }
}