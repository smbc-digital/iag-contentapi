using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using StockportContentApi.Config;
using StockportContentApi.FeatureToggling;
using StockportContentApi.Http;

namespace StockportContentApi.Repositories
{
    public interface IVideoRepository
    {
        string Process(string content);
    }

    public class VideoRepository : IVideoRepository
    {
        private readonly ButoConfig _butoConfig;
        private readonly TwentyThreeConfig _twentyThreeConfig;
        private readonly IHttpClient _httpClient;
        private readonly ILogger<VideoRepository> _logger;
        private readonly FeatureToggles _featureToggles;
        private const string StartTag = "{{VIDEO:";
        private const string EndTag = "}}";

        public VideoRepository(ButoConfig butoConfig, TwentyThreeConfig twentyThreeConfig, IHttpClient httpClient, ILogger<VideoRepository> logger, FeatureToggles featureToggles)
        {
            _butoConfig = butoConfig;
            _twentyThreeConfig = twentyThreeConfig;
            _httpClient = httpClient;
            _logger = logger;
            _featureToggles = featureToggles;
        }

        public string Process(string content)
        {
            return ReplaceVideoTagsWithVideoContent(content);
        }

        private string ReplaceVideoTagsWithVideoContent(string body)
        {
            var videoTags = GetVideosTags(body);

            foreach (var videoTag in videoTags)
            {
                var videoId = videoTag
                        .Replace(StartTag, string.Empty)
                        .Replace(EndTag, string.Empty);

                if (!VideoExists(videoId)) body = body.Replace(videoTag, string.Empty);
            }

            return body;
        }

        private static IEnumerable<string> GetVideosTags(string body)
        {
            var matches = Regex.Matches(body, "{{VIDEO:([0-9aA-zZ]*;?[0-9aA-zZ]*)}}").OfType<Match>();
            return matches.Select(m => m.Value).ToList();
        }

        private bool VideoExists(string videoId)
        {
            string url;
            var videoData = videoId.Split(';');

            if (_featureToggles.TwentyThreeVideo && videoData.Length > 1)
            {
                url = $"{_twentyThreeConfig.BaseUrl}{videoData[0]}&token={videoData[1]}";
            }
            else
            {
                url = $"{_butoConfig.BaseUrl}video/{videoData[0]}";
            }
            
            var response = _httpClient.Get(url);
            var result = response.Result;

            if (result != null && result.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            // video doesn't exist, log and return false
            _logger.LogWarning("Buto video with id \"" + videoData[0] + "\" not found.");
            return false;
        }
    }
}