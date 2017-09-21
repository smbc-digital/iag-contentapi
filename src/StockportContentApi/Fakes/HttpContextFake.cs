using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
