using Contentful.Core.Configuration;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    public class ContactUsId
    {
        public string Name { get; }
        public string Slug { get; }
        public string EmailAddress { get; }      

        public ContactUsId(string name, string slug, string emailAddress)
        {
            Name = name;
            Slug = slug;
            EmailAddress = emailAddress;
        }
    }

    public class NullContactUsId : ContactUsId
    {
         public NullContactUsId() : base(string.Empty,string.Empty, string.Empty) { }
    }
}
