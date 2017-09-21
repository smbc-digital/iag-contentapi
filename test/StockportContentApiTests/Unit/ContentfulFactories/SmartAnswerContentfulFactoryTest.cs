using System.Linq;
using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;
using Moq;
using StockportContentApi.Model;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
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
