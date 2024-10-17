namespace StockportContentApiTests.Unit.Builders;

class ContentfulGroupAdvisorBuilder
{
    string _name = "name";
    string _email = "email";
    IEnumerable<ContentfulReference> _contentfulReference = new List<ContentfulReference> { new ContentfulReferenceBuilder().Build() };
    bool _globalAccess = false;

    public ContentfulGroupAdvisor Build()
        => new()
        {
            Name = _name,
            Email = _email,
            GlobalAccess = _globalAccess,
            Groups = _contentfulReference
        };

    public ContentfulGroupAdvisorBuilder Email(string value)
    {
        _email = value;
        return this;
    }

    public ContentfulGroupAdvisorBuilder Name(string value)
    {
        _name = value;
        return this;
    }

    public ContentfulGroupAdvisorBuilder GlobalAccess(bool value)
    {
        _globalAccess = value;
        return this;
    }

    public ContentfulGroupAdvisorBuilder ContentfulReferences(IEnumerable<ContentfulReference> value)
    {
        _contentfulReference = value;
        return this;
    }
}