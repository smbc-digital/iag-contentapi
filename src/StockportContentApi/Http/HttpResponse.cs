namespace StockportContentApi.Http;

public class HttpResponse
{
    public readonly HttpStatusCode StatusCode;
    private readonly object _content;
    public readonly string Error;

    private HttpResponse(HttpStatusCode statusCode, object content, string error)
    {
        StatusCode = statusCode;
        _content = content;
        Error = error;
    }

    public static HttpResponse Successful(object content)
    {
        return new HttpResponse(HttpStatusCode.OK, content, string.Empty);
    }

    public static HttpResponse Failure(HttpStatusCode statusCode, string error)
    {
        return new HttpResponse(statusCode, null, error);
    }

    public IActionResult CreateResult()
    {
        switch (StatusCode)
        {
            case HttpStatusCode.OK:
                return new ObjectResult(_content);
            case HttpStatusCode.BadRequest:
                return new BadRequestObjectResult(Error);
            case HttpStatusCode.NotFound:
                return new NotFoundObjectResult(Error);
            case HttpStatusCode.InternalServerError:
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            case HttpStatusCode.BadGateway:
                return new StatusCodeResult((int)HttpStatusCode.BadGateway);
            default:
                return new EmptyResult();
        }

    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public T Get<T>()
    {
        return (T)_content;
    }
}