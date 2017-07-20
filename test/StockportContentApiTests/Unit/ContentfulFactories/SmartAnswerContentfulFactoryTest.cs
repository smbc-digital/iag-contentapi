using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;
using Moq;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class SmartAnswerContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateASmartAnswersFromAContentfulReference()
        {
            var ContentfulReference =
                new ContentfulSmartAnswerBuilder().Build();                    
 
            var smartAnswers = new SmartAnswerContentfulFactory().ToModel(ContentfulReference);

            smartAnswers.Slug.Should().Be(ContentfulReference.Slug);
            smartAnswers.QuestionJson.Should().Be(ContentfulReference.QuestionJson);
        }      
    }
}
