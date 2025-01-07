namespace StockportContentApi.Repositories;

public interface IVideoRepository
{
    string Process(string content);
}

public class VideoRepository(TwentyThreeConfig twentyThreeConfig,
                            IHttpClient httpClient) : IVideoRepository
{
    private readonly TwentyThreeConfig _twentyThreeConfig = twentyThreeConfig;
    private readonly IHttpClient _httpClient = httpClient;
    private const string StartTag = "{{VIDEO:";
    private const string EndTag = "}}";

    public string Process(string content) =>
        ReplaceVideoTagsWithVideoContent(content);

    private static string ReplaceVideoTagsWithVideoContent(string body)
    {
        IEnumerable<string> videoTags = GetVideosTags(body);

        foreach (string videoTag in videoTags)
        {
            string videoId = videoTag
                .Replace(StartTag, string.Empty)
                .Replace(EndTag, string.Empty);
        }

        return body;
    }

    private static IEnumerable<string> GetVideosTags(string body)
    {
        IEnumerable<Match> matches = Regex.Matches(body, "{{VIDEO:([0-9aA-zZ]*;?[0-9aA-zZ]*)}}").OfType<Match>();
        
        return matches.Select(m => m.Value).ToList();
    }
}