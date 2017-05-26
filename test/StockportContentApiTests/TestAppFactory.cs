using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Fakes;

namespace StockportContentApiTests
{
    public class TestAppFactory
    {
        public static TestServer MakeFakeApp()
        {
            var hostBuilder = new WebHostBuilder()
             
             .UseStartup<FakeStartup>()
             .UseUrls("http://localhost:5001")
             .UseKestrel()
             .UseEnvironment("test")
             .UseContentRoot(Directory.GetCurrentDirectory());

            return new TestServer(hostBuilder);
        }

        public class FakeStartup : Startup
        {
            public FakeStartup(IHostingEnvironment env) : base(env) {}

            public override void ConfigureServices(IServiceCollection services)
            {
                base.ConfigureServices(services);
                services.AddTransient<IHttpClient>(p => GetHttpClient(p.GetService<ILoggerFactory>()));

                var dateTime = new Mock<ITimeProvider>();
                dateTime.Setup(o => o.Now()).Returns(FakeTimeProvider.DateTime);

                services.AddSingleton(dateTime.Object);

                var cache = new Mock<ICacheWrapper>();
                object emptyListReturnedFromCache = new List<string>();
                cache.Setup(o => o.TryGetValue("eventCategories", out emptyListReturnedFromCache)).Returns(false);
                services.AddSingleton(cache.Object);


                var contentfulClientManager = new Mock<IContentfulClientManager>();
                contentfulClientManager.Setup(o => o.GetClient(It.IsAny<ContentfulConfig>())).Returns(FakeContentfulClientFactory.Client.Object);
                services.AddSingleton(contentfulClientManager.Object);                
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

        public class FakeContentfulClientFactory
        {
            public static void MakeContentfulClientWithConfiguration(Action<Mock<Contentful.Core.IContentfulClient>> configureFakeContentfulClient)
            {
                Client = new Mock<Contentful.Core.IContentfulClient>();
                configureFakeContentfulClient(Client);
            }

            public static Mock<Contentful.Core.IContentfulClient> Client { get; private set; }
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
