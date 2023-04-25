namespace StockportContentApiTests.Unit.Builders;

class GroupAdvisorBuilder
{
    string _name = "name";
    string _email = "email";
    IEnumerable<string> _groups = new List<string> { "slug" };
    bool _globalAccess = false;

    public GroupAdvisor Build()
    {
        return new GroupAdvisor(_name, _email, _groups, _globalAccess);
    }

    public GroupAdvisorBuilder Email(string value)
    {
        _email = value;
        return this;
    }

    public GroupAdvisorBuilder Name(string value)
    {
        _name = value;
        return this;
    }

    public GroupAdvisorBuilder GlobalAccess(bool value)
    {
        _globalAccess = value;
        return this;
    }

    public GroupAdvisorBuilder Groups(IEnumerable<string> value)
    {
        _groups = value;
        return this;
    }
}
