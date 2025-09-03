namespace StockportContentApiTests.Unit;

public class ResponseHandlerTests
{
    private readonly FakeLogger<ResponseHandler> _fakeLogger = new();

    [Fact]
    public async Task HandlesException()
    {
        // Arrange
        ResponseHandler handler = new(_fakeLogger);

        // Act
        IActionResult result = await handler.Get(() =>
        {
            throw new Exception("error");
        });

        // Assert
        Assert.Equal("An unexpected error occurred while performing the get operation", _fakeLogger.ErrorMessage);
        Assert.Equal(500, (result as StatusCodeResult).StatusCode);
    }
}