using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Moq;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApiTests.Unit.Helpers
{
    public class AuthenticationHelperTests
    {
        private Mock<ITimeProvider> _timeProvider;
        private Mock<IApiKeyRepository> _apiRepository;

        public AuthenticationHelperTests()
        {
            _timeProvider = new Mock<ITimeProvider>();
            _apiRepository = new Mock<IApiKeyRepository>();
        }

        public void ExtractAuthenticationDataFromContext_ShouldReturnAuthenticationDataWithCorrectValues()
        {
            var context = new DefaultHttpContext();
            context.Request.Path = "/stockportgov/test";
            context.Request.Headers.Add("Authorization", "test");


        }
    }
}
