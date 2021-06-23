using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

        public string Process(string content) =>
            ReplaceVideoTagsWithVideoContent(content);

        private string ReplaceVideoTagsWithVideoContent(string body)
        {
            var videoTags = GetVideosTags(body);

            foreach (var videoTag in videoTags)
            {
                var videoId = videoTag
                    .Replace(StartTag, string.Empty)
                    .Replace(EndTag, string.Empty);

                if (!VideoExists(videoId))
                    body = body.Replace(videoTag, string.Empty);
            }

            return body;
        }

        private static IEnumerable<string> GetVideosTags(string body) =>
            Regex
                .Matches(body, "{{VIDEO:([0-9aA-zZ]*;?[0-9aA-zZ]*)}}")
                .OfType<Match>()
                .Select(match => match.Value)
                .ToList();

        private bool VideoExists(string videoId)
        {
            var videoData = videoId.Split(';');
            string url = $"{_twentyThreeConfig.BaseUrl}{videoData[0]}&token={videoData[1]}";
            
            var result = _httpClient.Get(url).Result;

            if (result != null && result.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            _logger.LogWarning($"Twenty three video with id '{videoData[0]}' not found. Video URL: {url}. Response {JsonConvert.SerializeObject(result)}");
            return false;
        }
    }
}