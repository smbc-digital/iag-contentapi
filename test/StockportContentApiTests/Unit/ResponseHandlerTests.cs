namespace StockportContentApiTests.Unit;

public class ResponseHandlerTests
{
    private readonly FakeLogger<ResponseHandler> _fakeLogger = new();

    [Fact]
    public void HandlesException()
    {
        ResponseHandler handler = new(_fakeLogger);
        IActionResult result = AsyncTestHelper.Resolve(handler.Get(() =>
        {
            throw new Exception("error");
        }));
        Assert.Equal("An unexpected error occurred while performing the get operation",
                    _fakeLogger.ErrorMessage);
        Assert.Equal(500, (result as StatusCodeResult).StatusCode);
    }
}