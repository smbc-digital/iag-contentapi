using StockportContentApi.ContentfulModels;
using System.Collections.Generic;

namespace StockportContentApiTests.Unit.Builders
{
    class ContentfulGroupAdvisorBuilder
    {

        string _name = "name";
        string _email = "email";
        IEnumerable<ContentfulReference> _contentfulReference = new List<ContentfulReference> { new ContentfulReferenceBuilder().Build() };
        bool _globalAccess = false;

        public ContentfulGroupAdvisor Build()
        {
            return new ContentfulGroupAdvisor
            {
                Name = _name,
                EmailAddress = _email,
                HasGlobalAccess = _globalAccess,
                Groups = _contentfulReference
            };
        }

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
}
