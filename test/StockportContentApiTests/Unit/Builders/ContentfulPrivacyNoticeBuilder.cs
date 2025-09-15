namespace StockportContentApiTests.Unit.Builders;

public class ContentfulPrivacyNoticeBuilder
{
    public ContentfulPrivacyNotice Build() =>
        new()
        {
            Slug = "test-slug",
            Title = "test title",
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
}