﻿using System;
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
using Contentful.Core.Search;
using StockportContentApi.ContentfulModels;
using System.Threading.Tasks;
using StockportContentApiTests.Unit.Builders;

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
                var anEvent = new ContentfulEventBuilder().Slug("event1").EventDate(new DateTime(9999, 09, 09)).UpdatedAt(new DateTime(2016, 10, 05)).Build();
                var anotherEvent = new ContentfulEventBuilder().Slug("event2").EventDate(new DateTime(9999, 09, 09)).UpdatedAt(new DateTime(2016, 10, 05)).Build();
                var eventList = new List<ContentfulEvent> { anEvent, anotherEvent };               
                cache.Setup(o => o.GetFromCacheOrDirectly("event-all", It.IsAny<Func<Task<IList<ContentfulEvent>>>>())).ReturnsAsync(eventList);

                var categories = new List<string> { "Arts and Crafts","Business Events","Sports","Museums","Charity","Council","Christmas","Dance","Education","Chadkirk Chapel",
                                                "Community Group","Public Health","Fayre","Talk","Environment","Comedy","Family","Armed Forces","Antiques and Collectors","Excercise and Fitness",
                                                "Fair","Emergency Services","Bonfire","Remembrence Service" };
                cache.Setup(o => o.GetFromCacheOrDirectly("event-categories", It.IsAny<Func<Task<List<string>>>>())).ReturnsAsync(categories);

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
