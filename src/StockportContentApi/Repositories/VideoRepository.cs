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
        private const string StartTag = "{{VIDEO:";
        private const string EndTag = "}}";

        public VideoRepository(TwentyThreeConfig twentyThreeConfig, IHttpClient httpClient)
        {
            _twentyThreeConfig = twentyThreeConfig;
            _httpClient = httpClient;
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
            var videoData = videoId.Split(';');
            string url = $"{_twentyThreeConfig.BaseUrl}{videoData[0]}&token={videoData[1]}";
            
            var result = _httpClient.Get(url).Result;

            return result != null && result.StatusCode == HttpStatusCode.OK;
        }
    }
}