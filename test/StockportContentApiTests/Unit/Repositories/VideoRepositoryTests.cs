namespace StockportContentApiTests.Unit.Repositories;

public class VideoRepositoryTests : TestingBaseClass
{
    private readonly VideoRepository _videoRepository;
    private readonly Mock<IHttpClient> _fakeHttpClient = new();
    private const string MockTwentyThreeApiUrl = "https://y84kj.videomarketingplatform.co/v.ihtml/player.html?source=embed&photo%5fid=";

    public VideoRepositoryTests()
        => _videoRepository = new VideoRepository(new TwentyThreeConfig(MockTwentyThreeApiUrl), _fakeHttpClient.Object);

    [Fact]
    public void Process_ShouldReturnContentUnchanged_WhenNoVideoTagsPresent()
    {
        // Arrange
        string content = "This is a simple content without video tags.";

        // Act
        string result = _videoRepository.Process(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Process_ShouldProcessSingleVideoTag_WhenVideoTagIsPresent()
    {
        // Arrange
        string content = "This content contains a video tag {{VIDEO:12345}}.";

        // Act
        string result = _videoRepository.Process(content);

        // Assert
        Assert.Contains("{{VIDEO:12345}}", result);
    }

    [Fact]
    public void Process_ShouldProcessMultipleVideoTags_WhenMultipleVideoTagsArePresent()
    {
        // Arrange
        string content = "First video: {{VIDEO:12345}}, second video: {{VIDEO:67890}}.";

        // Act
        string result = _videoRepository.Process(content);

        // Assert
        Assert.Contains("{{VIDEO:12345}}", result);
        Assert.Contains("{{VIDEO:67890}}", result);
    }

    [Fact]
    public void Process_ShouldNotThrowException_WhenInvalidVideoTagIsPresent()
    {
        // Arrange
        string content = "This content contains an invalid video tag {{VIDEO:INVALID}}.";

        // Act
        var exception = Record.Exception(() => _videoRepository.Process(content));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void GetVideosTags_ShouldReturnEmptyList_WhenNoVideoTagsPresent()
    {
        // Arrange
        string content = "No video tags in this content.";

        // Act
        IEnumerable<string> result = InvokePrivateMethod<IEnumerable<string>>(_videoRepository, "GetVideosTags", content);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetVideosTags_ShouldReturnCorrectTags_WhenVideoTagsArePresent()
    {
        // Arrange
        string content = "Here is a video tag {{VIDEO:12345}} and another one {{VIDEO:67890}}.";

        // Act
        IEnumerable<string> result = InvokePrivateMethod<IEnumerable<string>>(_videoRepository, "GetVideosTags", content);

        // Assert
        Assert.Contains("{{VIDEO:12345}}", result);
        Assert.Contains("{{VIDEO:67890}}", result);
    }

    private static T InvokePrivateMethod<T>(object instance, string methodName, params object[] parameters)
    {
        MethodInfo methodInfo = instance.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
        
        return (T)methodInfo.Invoke(instance, parameters);
    }
}