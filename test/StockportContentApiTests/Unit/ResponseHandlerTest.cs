namespace StockportContentApiTests.Unit;

public class ResponseHandlerTest
{
    private readonly FakeLogger<ResponseHandler> _fakeLogger = new FakeLogger<ResponseHandler>();

    [Fact]
    public void HandlesException()
    {
        var handler = new ResponseHandler(_fakeLogger);
        var result = AsyncTestHelper.Resolve(handler.Get(() =>
        {
            throw new Exception("error");
        }));
        Assert.Equal("An unexpected error occurred while performing the get operation",
                    _fakeLogger.ErrorMessage);
        Assert.Equal(500, (result as StatusCodeResult).StatusCode);
    }
}