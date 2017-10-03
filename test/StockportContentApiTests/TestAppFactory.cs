using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Fakes;
using StockportContentApi.ContentfulModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using StockportContentApiTests.Unit.Builders;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.PlatformAbstractions;
using NLog.Extensions.Logging;
using StockportContentApi.Model;

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
                .UseContentRoot(Path.GetFullPath(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
                    "..", "..", "..")));

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

                var cache = new Mock<ICache>();

                cache.Setup(_ => _.GetKeys()).ReturnsAsync(new List<RedisValueData>()
                {
                    new RedisValueData()
                    {
                        Expiry = "expiry",
                        Key = "Key",
                        NumberOfItems = 4
                    }
                });

                var categories = new List<string> { "Arts and Crafts","Business Events","Sports","Museums","Charity","Council","Christmas","Dance","Education","Chadkirk Chapel",
                                                "Community Group","Public Health","Fayre","Talk","Environment","Comedy","Family","Armed Forces","Antiques and Collectors","Excercise and Fitness",
                                                "Fair","Emergency Services","Bonfire","Remembrence Service" };
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync("event-categories", It.IsAny<Func<Task<List<string>>>>(), It.IsAny<int>())).ReturnsAsync(categories);

                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "group-categories"), It.IsAny<Func<Task<List<GroupCategory>>>>(), It.IsAny<int>())).ReturnsAsync(new List<GroupCategory> { new GroupCategory("name", "slug", "icon", "image-url.jpg") });
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.IsAny<int>())).ReturnsAsync(new List<ContentfulEvent> {
                                    new ContentfulEventBuilder().Slug("event1").UpdatedAt(new DateTime(2016,10,5)).Build(),
                                    new ContentfulEventBuilder().Slug("event2").UpdatedAt(new DateTime(2016,10,5)).Build()
                                });

                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-all"), It.IsAny<Func<Task<IList<ContentfulNews>>>>(), It.IsAny<int>())).ReturnsAsync(new List<ContentfulNews> {
                                    new ContentfulNewsBuilder().Slug("news_item").SunriseDate(new DateTime(2016, 6, 20)).SunsetDate(new DateTime(9999, 9, 9)).Document().Build()
                                });

                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "newsroom"), It.IsAny<Func<Task<ContentfulNewsRoom>>>(), It.IsAny<int>()))
                    .ReturnsAsync(new ContentfulNewsRoomBuilder().Build());

                var newsCategories = new List<string> { "Benefits","Business","Council leader","Crime prevention and safety","Children and families","Environment","Elections","Health and social care","Housing","Jobs","Leisure and culture","Libraries","Licensing","Partner organisations","Planning and building","Roads and travel","Schools and education","Waste and recycling","Test Category" };
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "news-categories"), It.IsAny<Func<Task<List<string>>>>(), It.IsAny<int>())).ReturnsAsync(newsCategories);

                var nullAToZ = new List<AtoZ>();             
                var aToZArticle = new List<AtoZ>
                {
                    new AtoZ("Vintage Village turns 6 years old", "vintage-village-turns-6-years-old", "The vintage village turned 6 with a great reception", "article", new List<string>()),                    
                };
                var aToZTopic = new List<AtoZ>
                {
                    new AtoZ("Benefits & Support", "benefits-and-support", "Benefits & Support", "topic", new List<string>()),
                     new AtoZ("Bins & Recycling", "bins-and-recycling", "Collection days, bulky items", "topic", new List<string>()),
                };

                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "atoz-article-v"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>())).ReturnsAsync(aToZArticle);
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "atoz-topic-v"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>())).ReturnsAsync(nullAToZ);
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "atoz-showcase-v"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>())).ReturnsAsync(nullAToZ);
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "atoz-article-b"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>())).ReturnsAsync(nullAToZ);
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "atoz-topic-b"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>())).ReturnsAsync(aToZTopic);
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "atoz-showcase-b"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>())).ReturnsAsync(nullAToZ);
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "atoz-article-c"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>())).ReturnsAsync(aToZArticle);
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "atoz-topic-c"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>())).ReturnsAsync(aToZTopic);
                cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "atoz-showcase-c"), It.IsAny<Func<Task<List<AtoZ>>>>(), It.IsAny<int>())).ReturnsAsync(nullAToZ);

                services.AddSingleton(cache.Object);

                var contentfulClientManager = new Mock<IContentfulClientManager>();
                contentfulClientManager.Setup(o => o.GetClient(It.IsAny<ContentfulConfig>())).Returns(FakeContentfulClientFactory.Client.Object);
                services.AddSingleton(contentfulClientManager.Object);                
            }

            // used for removing middleware authentication
            public override void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IDistributedCache cache, IApplicationLifetime appLifetime)
            {
                app.UseApplicationInsightsRequestTelemetry();
                app.UseApplicationInsightsExceptionTelemetry();
                app.UseStaticFiles();
                app.UseMvc();

                loggerFactory.AddNLog();
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
