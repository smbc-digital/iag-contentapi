using Microsoft.AspNetCore.Http;

namespace StockportContentApi.Fakes
{
    public static class HttpContextFake
    {
        public static IHttpContextAccessor GetHttpContextFake()
        {
            var _httpContextAccessor = new HttpContextAccessor();
            var _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["canViewSensitive"] = "true";
            _httpContextAccessor.HttpContext = _httpContext;
            return _httpContextAccessor;
        }
    }
}
