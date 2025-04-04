﻿namespace StockportContentApiTests.Unit.Builders;

public class ContentfulParentTopicBuilder
{
    private readonly string _name = "name";
    private readonly List<ContentfulReference> _subItems = new() { new ContentfulReferenceBuilder().Slug("sub-slug").Build() };
    private readonly List<ContentfulReference> _secondaryItems = new() { new ContentfulReferenceBuilder().Slug("secondary-slug").Build() };
    private string _systemId = "id";
    private string _contentTypeSystemId = "id";

    public ContentfulTopic Build()
        => new()
        {
            Name = _name,
            SubItems = _subItems,
            SecondaryItems = _secondaryItems,
            Sys = new SystemProperties
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                Id = _systemId
            }
        };

    public ContentfulParentTopicBuilder SystemId(string id)
    {
        _systemId = id;
        return this;
    }

    public ContentfulParentTopicBuilder SystemContentTypeId(string id)
    {
        _contentTypeSystemId = id;
        return this;
    }
}