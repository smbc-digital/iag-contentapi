using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StockportContentApi.Http;

namespace StockportContentApiTests.Unit.Fakes
{
    public class FakeHttpClient : IHttpClient
    {
        private readonly Dictionary<string, object> _responses = new Dictionary<string, object>();
        private string _url;

        public FakeHttpClient For(string url)
        {
            _url = url;
            return this;
        }

        public void Return(HttpResponse response)
        {
            _responses.Add(_url, response);
        }

        public void Throw(Exception exception)
        {
            _responses.Add(_url, exception);
        }

        public Task<HttpResponse> Get(string url)
        {
            object response = _responses[url];

            var exception = response as Exception;
            if (exception != null)
                throw exception;

            return Task.FromResult((HttpResponse) _responses[url]);
        }
    }
}