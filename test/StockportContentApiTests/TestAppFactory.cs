using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi;
using StockportContentApi.Http;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Fakes;

namespace StockportContentApiTests
{
    public class TestAppFactory
    {
        public static TestServer MakeFakeApp()
        {
            Environment.SetEnvironmentVariable("UNITTEST_SPACE", "XX");
            Environment.SetEnvironmentVariable("UNITTEST_ACCESS_KEY", "XX");

            var hostBuilder = new WebHostBuilder()
             
             .UseStartup<FakeStartup>()
             .UseUrls("http://localhost:5001")
             .UseKestrel()
             .UseEnvironment("Development");

            return new TestServer(hostBuilder);
        }

        public class FakeStartup : Startup
        {
            public FakeStartup(IHostingEnvironment env) : base(env)
            {
            }

            public override void ConfigureServices(IServiceCollection services)
            {
                base.ConfigureServices(services);
                services.AddTransient<IHttpClient>(p => GetHttpClient(p.GetService<ILoggerFactory>()));

                var dateTime = new Mock<ITimeProvider>();
                dateTime.Setup(o => o.Now()).Returns(FakeTimeProvider.DateTime);

                services.AddSingleton(dateTime.Object);
            }

            public LoggingHttpClient GetHttpClient(ILoggerFactory loggingFactory)
            {
                var fakeHttpClient = FakeHttpClientFactory.Client;
                
                return new LoggingHttpClient(fakeHttpClient, loggingFactory.CreateLogger<LoggingHttpClient>());
            }

        }

        public class FakeTimeProvider
        {
            public static void SetDateTime(DateTime dateTime)
            {
                DateTime = dateTime;
            }

            public static DateTime DateTime { get; private set; }
        }

        public class FakeHttpClientFactory
        {
            public static void MakeFakeHttpClientWithConfiguration(
                Action<FakeHttpClient> configureFakeHttpClient)
            {
                Client = new FakeHttpClient();
                configureFakeHttpClient(Client);
            }

            public static FakeHttpClient Client { get; private set; }
        }
    }
}
