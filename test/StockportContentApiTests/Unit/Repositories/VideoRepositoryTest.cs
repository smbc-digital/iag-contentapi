using System.Net;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Repositories;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class VideoRepositoryTest : TestingBaseClass
    {
        private readonly VideoRepository _videoRepository;
        private readonly Mock<ILogger<VideoRepository>> _videoLogger;
        private readonly Mock<IHttpClient> _fakeHttpClient;
        private const string MockTwentyThreeApiUrl = "https://y84kj.videomarketingplatform.co/v.ihtml/player.html?source=embed&photo%5fid=";

        public VideoRepositoryTest()
        {
            _videoLogger = new Mock<ILogger<VideoRepository>>();
            _fakeHttpClient = new Mock<IHttpClient>();
            _videoRepository = new VideoRepository(new TwentyThreeConfig(MockTwentyThreeApiUrl), _fakeHttpClient.Object, _videoLogger.Object);
        }

        /// <summary>
        /// Nothing should be replaced as both videos exist
        /// </summary>
        [Fact]
        public void KeepsVideoTagsForMultipleTwentyThreeVideoTagsInContent()
        {
            _fakeHttpClient.Setup(o => o.Get($"{MockTwentyThreeApiUrl}VideoId1&token=VideoToken1"))
                .ReturnsAsync(HttpResponse.Successful(
                    GetStringResponseFromFile("StockportContentApiTests.Unit.MockVideoResponses.VideoExists.json")));

            _fakeHttpClient.Setup(o => o.Get($"{MockTwentyThreeApiUrl}VideoId2&token=VideoToken2"))
                .ReturnsAsync(HttpResponse.Successful(GetStringResponseFromFile("StockportContentApiTests.Unit.MockVideoResponses.VideoExists.json")));

            var content = "Some text {{VIDEO:VideoId1;VideoToken1}}, {{VIDEO:VideoId2;VideoToken2}} Some more text";
            var response = _videoRepository.Process(content);

            response.Should().Be(content);
        }

        /// <summary>
        /// Test to check if multiple video tags will be removed if they don't exist and keep one existing video tag in the content
        /// </summary>
        [Fact]
        public void RemovesVideoTagTwentyThreeVideoDoesNotExistAndWillKeepOneTag()
        {
            _fakeHttpClient.Setup(o => o.Get($"{MockTwentyThreeApiUrl}VideoId1&token=VideoToken1"))
                .ReturnsAsync(HttpResponse.Failure(HttpStatusCode.NotFound, "No video found"));

            _fakeHttpClient.Setup(o => o.Get($"{MockTwentyThreeApiUrl}VideoId2&token=VideoToken2"))
                .ReturnsAsync(HttpResponse.Failure(HttpStatusCode.NotFound, "No video found"));

            _fakeHttpClient.Setup(o => o.Get($"{MockTwentyThreeApiUrl}VideoId3&token=VideoToken3"))
                .ReturnsAsync(HttpResponse.Successful("video exists"));

            var content = "Some text {{VIDEO:VideoId1;VideoToken1}}, {{VIDEO:VideoId2;VideoToken2}} Some more text. {{VIDEO:VideoId3;VideoToken3}}";
            var result = _videoRepository.Process(content);

            LogTesting.Assert(_videoLogger, LogLevel.Warning, "Twenty three video with id 'VideoId1' not found.");
            LogTesting.Assert(_videoLogger, LogLevel.Warning, "Twenty three video with id 'VideoId2' not found.");
            result.Should().NotContain("{{VIDEO:VideoId1;VideoToken1}}");
            result.Should().NotContain("{{VIDEO:VideoId2;VideoToken2}}");
            result.Should().Contain("{{VIDEO:VideoId3;VideoToken3}}");
        }

        /// <summary>
        /// Test to check video tag gets removed if video provider is down or the url is wrong (service unavailable)
        /// </summary>
        [Fact]
        public void RemovesOneVideoTagIfTwentyThreeReturnsServiceUnavailable()
        {
            _fakeHttpClient.Setup(o => o.Get($"{MockTwentyThreeApiUrl}VideoId1&token=VideoToken1"))
                .ReturnsAsync(HttpResponse.Successful("video exists"));

            _fakeHttpClient.Setup(o => o.Get($"{MockTwentyThreeApiUrl}VideoId2&token=VideoToken2"))
                .ReturnsAsync(HttpResponse.Failure(HttpStatusCode.ServiceUnavailable, "Service unavailable"));

            var content = "Some text {{VIDEO:VideoId1;VideoToken1}}, {{VIDEO:VideoId2;VideoToken2}} Some more text";

            var response = _videoRepository.Process(content);

            response.Should().Be("Some text {{VIDEO:VideoId1;VideoToken1}},  Some more text");
        }
    }
}