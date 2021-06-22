using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using StockportContentApi.Config;
using StockportContentApi.Http;

namespace StockportContentApi.Repositories
{
    public interface IVideoRepository
    {
        string Process(string content);
    }

    public class VideoRepository : IVideoRepository
    {
        private readonly TwentyThreeConfig _twentyThreeConfig;
        private readonly IHttpClient _httpClient;
        private readonly ILogger<VideoRepository> _logger;
        private const string StartTag = "{{VIDEO:";
        private const string EndTag = "}}";

        public VideoRepository(TwentyThreeConfig twentyThreeConfig, IHttpClient httpClient, ILogger<VideoRepository> logger)
        {
            _twentyThreeConfig = twentyThreeConfig;
            _httpClient = httpClient;
            _logger = logger;
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

                if (!VideoExists(videoId, body)) body = body.Replace(videoTag, string.Empty);
            }

            return body;
        }

        private static IEnumerable<string> GetVideosTags(string body)
        {
            var matches = Regex.Matches(body, "{{VIDEO:([0-9aA-zZ]*;?[0-9aA-zZ]*)}}").OfType<Match>();
            return matches.Select(m => m.Value).ToList();
        }

        private bool VideoExists(string videoId, string body)
        {
            var videoData = videoId.Split(';');
            string url = $"{_twentyThreeConfig.BaseUrl}{videoData[0]}&token={videoData[1]}";
            
            var response = _httpClient.Get(url);
            var result = response.Result;

            if (result != null && result.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            // video doesn't exist, log and return false
            _logger.LogWarning($"Twenty three video with id '{videoData[0]}' not found. Video URL: {url}. Body {body}");
            return false;
        }
    }
}