using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using Newtonsoft.Json;
using StockportContentApi.ContentfulFactories.SmartAnswersFactories;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class SmartAnswerContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateASmartAnswersFromAContentfulReference()
        {
            var ContentfulReference =
                new ContentfulSmartAnswerBuilder().Build();                    
 
            var smartAnswers = new SmartAnswerContentfulFactory(HttpContextFake.GetHttpContextFake()).ToModel(ContentfulReference);

            var stringQuestionJson = JsonConvert.SerializeObject(ContentfulReference.QuestionJson);

            smartAnswers.Slug.Should().Be(ContentfulReference.Slug);
            smartAnswers.QuestionJson.Should().Be(stringQuestionJson);
        }      
    }
}
