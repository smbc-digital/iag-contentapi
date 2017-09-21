using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Fakes;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ContactUsIdContentfulFactoryTest
    {
        private readonly ContentfulContactUsId _contentfulContactUsId;
        private readonly ContactUsIdContentfulFactory _contactUsIdContentfulFactory;

        public ContactUsIdContentfulFactoryTest()
        {
            _contentfulContactUsId = new ContentfulContactUsId() {EmailAddress = "test@stockport.gov.uk", Name = "Test email", Slug = "test-email"};
         
            _contactUsIdContentfulFactory = new ContactUsIdContentfulFactory(HttpContextFake.GetHttpContextFake());
        }

        [Fact]
        public void ShouldCreateAContactUsIdFromAContentfulContactUsId()
        {
            var contactUsId = _contactUsIdContentfulFactory.ToModel(_contentfulContactUsId);
            contactUsId.ShouldBeEquivalentTo(_contentfulContactUsId);
        }
    }
}
