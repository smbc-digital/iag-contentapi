using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AutoMapper;
using Contentful.Core.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Profile = StockportContentApi.Model.Profile;
using Microsoft.Extensions.Configuration;
using NLog.Extensions.Logging;
using StockportWebapp.DataProtection;

namespace StockportContentApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add all custom contentful factories
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddContentfulFactories(this IServiceCollection services)
        {
            services.AddSingleton<IContentfulFactory<Asset, Document>>(new DocumentContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulContactUsId, ContactUsId>>(new ContactUsIdContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulReference, Crumb>>(p => new CrumbContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulCarouselContent, CarouselContent>>(new CarouselContentContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulReference, SubItem>>(p => new SubItemContentfulFactory(p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<ContentfulHomepage, Homepage>>(p => new HomepageContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(),
                p.GetService<IContentfulFactory<ContentfulGroup, Group>>(),
                p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
                p.GetService<IContentfulFactory<ContentfulCarouselContent, CarouselContent>>(),
                p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox>>(p => new ExpandingLinkBoxContentfulfactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(), p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<List<ContentfulGroup>, List<Group>>>(p => new GroupListContentfulFactory(p.GetService<IContentfulFactory<ContentfulGroup, Group>>()));
            services.AddSingleton<IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>>>(p => new GroupCategoryListContentfulFactory(p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>()));
            services.AddSingleton<IContentfulFactory<List<ContentfulEventCategory>, List<EventCategory>>>(p => new EventCategoryListContentfulFactory(p.GetService<IContentfulFactory<ContentfulEventCategory, EventCategory>>()));
            services.AddSingleton<IContentfulFactory<ContentfulAlert, Alert>>(p => new AlertContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>>(p => new RedirectContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(p => new EventHomepageContentfulFactory(p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<ContentfulEventBanner, EventBanner>>(p => new EventBannerContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulConsultation, Consultation>>(p => new ConsultationContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>(p => new SocialMediaLinkContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulSection, Section>>(p => new SectionContentfulFactory(p.GetService<IContentfulFactory<ContentfulProfile, Profile>>(),
                p.GetService<IContentfulFactory<Asset, Document>>(),
                p.GetService<IVideoRepository>(),
                p.GetService<ITimeProvider>(),
                p.GetService<IContentfulFactory<ContentfulAlert, Alert>>()));
            services.AddSingleton<IContentfulFactory<ContentfulEvent, Event>>(p => new EventContentfulFactory(p.GetService<IContentfulFactory<Asset, Document>>(),
                p.GetService<IContentfulFactory<ContentfulGroup, Group>>(),
                p.GetService<IContentfulFactory<List<ContentfulEventCategory>, List<EventCategory>>>(),
                p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
                p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<ContentfulProfile, Profile>>(p => new ProfileContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, Crumb>>()));
            services.AddSingleton<IContentfulFactory<List<ContentfulEvent>, List<Event>>>(p => new EventListContentfulFactory(p.GetService<IContentfulFactory<ContentfulEvent, Event>>()));
            services.AddSingleton<IContentfulFactory<ContentfulGroup, Group>>(p => new GroupContentfulFactory(p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(), p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<ContentfulPayment, Payment>>(p => new PaymentContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, Crumb>>()));
            services.AddSingleton<IContentfulFactory<ContentfulTopic, Topic>>(p => new TopicContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(),
                p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(),
                p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
                p.GetService<IContentfulFactory<ContentfulEventBanner, EventBanner>>(),
                p.GetService<IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox>>(),
                p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<ContentfulShowcase, Showcase>>
            (p => new ShowcaseContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(), p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(), p.GetService<ITimeProvider>(),
                p.GetService<IContentfulFactory<ContentfulConsultation, Consultation>>(),
                p.GetService<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>()));
            services.AddSingleton<IContentfulFactory<ContentfulFooter, Footer>>
                (p => new FooterContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(), p.GetService<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>()));
            services.AddSingleton<IContentfulFactory<ContentfulNews, News>>(p => new NewsContentfulFactory(p.GetService<IVideoRepository>(),
                p.GetService<IContentfulFactory<Asset, Document>>()));
            services.AddSingleton<IContentfulFactory<ContentfulNewsRoom, Newsroom>>(p => new NewsRoomContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(p => new GroupCategoryContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulEventCategory, EventCategory>>(p => new EventCategoryContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulArticle, Article>>
            (p => new ArticleContentfulFactory(p.GetService<IContentfulFactory<ContentfulSection, Section>>(),
                p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(),
                p.GetService<IContentfulFactory<ContentfulProfile, Profile>>(),
                p.GetService<IContentfulFactory<ContentfulArticle, Topic>>(),
                p.GetService<IContentfulFactory<ContentfulLiveChat, LiveChat>>(),
                p.GetService<IContentfulFactory<Asset, Document>>(),
                p.GetService<IVideoRepository>(),
                p.GetService<ITimeProvider>(),
                p.GetService<IContentfulFactory<ContentfulAlert, Alert>>()));
            services.AddSingleton<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>>
                (p => new TopicSiteMapContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>>
                (p => new ArticleSiteMapContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulLiveChat, LiveChat>>
                (p => new LiveChatContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulSmartAnswers, SmartAnswer>>
                (p => new SmartAnswerContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulAtoZ, AtoZ>>
                (p => new AtoZContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulArticle, Topic>>(
                p => new ParentTopicContentfulFactory(
                    p.GetService<IContentfulFactory<ContentfulReference, SubItem>>()
                    , p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<ContentfulStartPage, StartPage>>
                (p => new StartPageFactoryContentfulFactory(p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<IContentfulFactory<ContentfulReference, Crumb>>()));

            return services;
        }

        /// <summary>
        /// Add cache services
        /// </summary>
        /// <param name="services"></param>
        /// <param name="useRedisSession"></param>
        /// <returns></returns>
        public static IServiceCollection AddCache(this IServiceCollection services, bool useRedisSession)
        {
            services.AddSingleton<IDistributedCacheWrapper>(
                p => new DistributedCacheWrapper(p.GetService<IDistributedCache>()));
            services.AddSingleton<ICache>(p => new Utils.Cache(p.GetService<IDistributedCacheWrapper>(), p.GetService<ILogger<ICache>>(), useRedisSession));

            return services;
        }

        /// <summary>
        /// Add contentful clients
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddContentfulClients(this IServiceCollection services)
        {
            services.AddSingleton<IContentfulClientManager>(new ContentfulClientManager(new System.Net.Http.HttpClient()));

            return services;
        }

        /// <summary>
        /// Add repositries
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddSingleton<IVideoRepository>(p => new VideoRepository(p.GetService<ButoConfig>(), p.GetService<IHttpClient>(), p.GetService<ILogger<VideoRepository>>()));
            services.AddSingleton<Func<ContentfulConfig, ArticleRepository>>(p => { return x => new ArticleRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<ContentfulArticle, Article>>(), p.GetService<IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>>(), p.GetService<IVideoRepository>(), p.GetService<ICache>() ); });
            services.AddSingleton<Func<ContentfulConfig, EventRepository>>(p => { return x => new EventRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<ContentfulEvent, Event>>(), p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(), p.GetService<ICache>(), p.GetService<ILogger<EventRepository>>()); });
            services.AddSingleton<Func<ContentfulConfig, EventRepository>>(p => { return x => new EventRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<ContentfulEvent, Event>>(), p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(), p.GetService<ICache>(), p.GetService<ILogger<EventRepository>>()); });
            services.AddSingleton<Func<ContentfulConfig, ShowcaseRepository>>(
                p => {
                    return x => new ShowcaseRepository(x, p.GetService<IContentfulFactory<ContentfulShowcase, Showcase>>(),
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<IContentfulFactory<List<ContentfulEvent>, List<Event>>>(),
                        p.GetService<IContentfulFactory<ContentfulNews, News>>(),
                        p.GetService<ITimeProvider>(),
                        new EventRepository(x,
                            p.GetService<IContentfulClientManager>(),
                            p.GetService<ITimeProvider>(),
                            p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                            p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                            p.GetService<ICache>(),
                            p.GetService<ILogger<EventRepository>>())
                    );
                });
            services.AddSingleton<Func<ContentfulConfig, ProfileRepository>>(
                p => { return x => new ProfileRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulProfile, Profile>>()); });
            services.AddSingleton<Func<ContentfulConfig, PaymentRepository>>(
                p => { return x => new PaymentRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulPayment, Payment>>()); });
            services.AddSingleton<Func<ContentfulConfig, GroupCategoryRepository>>(
                p => { return x => new GroupCategoryRepository(x, p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(), p.GetService<IContentfulClientManager>()); });
            services.AddSingleton<Func<ContentfulConfig, EventCategoryRepository>>(
                p => { return x => new EventCategoryRepository(x, p.GetService<IContentfulFactory<ContentfulEventCategory, EventCategory>>(), p.GetService<IContentfulClientManager>(), p.GetService<ICache>()); });
            services.AddSingleton<Func<ContentfulConfig, HomepageRepository>>(
                p => { return x => new HomepageRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulHomepage, Homepage>>()); });
            services.AddSingleton<Func<ContentfulConfig, StartPageRepository>>(
                p => { return x => new StartPageRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulStartPage, StartPage>>()); });
            services.AddSingleton<Func<ContentfulConfig, FooterRepository>>(
                p => { return x => new FooterRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulFooter, Footer>>()); });
            services.AddSingleton<Func<ContentfulConfig, NewsRepository>>(
                p => { return x => new NewsRepository(x, p.GetService<ITimeProvider>(), p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulNews, News>>(), p.GetService<IContentfulFactory<ContentfulNewsRoom, Newsroom>>()); });
            services.AddSingleton<Func<ContentfulConfig, AtoZRepository>>(
                p => { return x => new AtoZRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulAtoZ, AtoZ>>(), p.GetService<ITimeProvider>()); });
            services.AddSingleton<Func<ContentfulConfig, SectionRepository>>(
                p => { return x => new SectionRepository(x, p.GetService<IContentfulFactory<ContentfulSection, Section>>(), p.GetService<IContentfulClientManager>(), p.GetService<ITimeProvider>()); });
            services.AddSingleton<Func<ContentfulConfig, TopicRepository>>(
                p => { return x => new TopicRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulTopic, Topic>>(), p.GetService<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>>()); });
            services.AddSingleton<RedirectsRepository>();
            services.AddSingleton<Func<ContentfulConfig, GroupRepository>>(
                p => {
                    return x => new GroupRepository(x, p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulGroup, Group>>(),
                        p.GetService<IContentfulFactory<List<ContentfulGroup>, List<Group>>>(),
                        p.GetService<IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>>>(),
                        new EventRepository(x, p.GetService<IContentfulClientManager>(),
                            p.GetService<ITimeProvider>(),
                            p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                            p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                            p.GetService<ICache>(),
                            p.GetService<ILogger<EventRepository>>()),
                        p.GetService<ICache>());
                });
            services.AddSingleton<Func<ContentfulConfig, ContactUsIdRepository>>(
                p => { return x => new ContactUsIdRepository(x, p.GetService<IContentfulFactory<ContentfulContactUsId, ContactUsId>>(), p.GetService<IContentfulClientManager>()); });
            services.AddSingleton<Func<ContentfulConfig, SmartAnswersRepository>>(
                p => { return x => new SmartAnswersRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulSmartAnswers, SmartAnswer>>(), p.GetService<ICache>(), p.GetService<ILogger<SmartAnswersRepository>>()); });

            services.AddSingleton<Func<ContentfulConfig, ManagementRepository>>(
                p => { return x => new ManagementRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<ILogger<HttpClient>>()); });

            return services;
        }

        /// <summary>
        /// Add auto mapper
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperConfig());
            });

            services.AddSingleton<IMapper>(p => config.CreateMapper());

            return services;
        }

        /// <summary>
        /// Add custom contentful configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddContentfulConfig(this IServiceCollection services, IConfigurationRoot configuration)
        {
            Func<string, ContentfulConfig> createConfig = businessId =>
                new ContentfulConfig(businessId)
                    .Add("DELIVERY_URL", configuration["Contentful:DeliveryUrl"])
                    .Add($"{businessId.ToUpper()}_SPACE", configuration[$"{businessId}:Space"])
                    .Add($"{businessId.ToUpper()}_ACCESS_KEY", configuration[$"{businessId}:AccessKey"])
                    .Add($"{businessId.ToUpper()}_MANAGEMENT_KEY", configuration[$"{businessId}:ManagementKey"])
                    .Build();

            services.AddTransient(_ => createConfig);

            return services;
        }

        /// <summary>
        /// Add redirects
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedirects(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var redirectBusinessIds = new List<string>();
            configuration.GetSection("RedirectBusinessIds").Bind(redirectBusinessIds);
            services.AddSingleton(new RedirectBusinessIds(redirectBusinessIds));

            return services;
        }

        /// <summary>
        /// Add redis for distributed cache
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="useRedisSession"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfigurationRoot configuration, bool useRedisSession)
        {
            var loggerFactory = new LoggerFactory().AddNLog();
            ILogger logger = loggerFactory.CreateLogger<Startup>();

            if (useRedisSession)
            {
                var redisUrl = configuration["TokenStoreUrl"];
                var redisIp = GetHostEntryForUrl(redisUrl, logger);
                logger.LogInformation($"Using redis for session management - url {redisUrl}, ip {redisIp}");
                services.AddDataProtection().PersistKeysToRedis(redisIp);

                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = redisIp;
                    options.InstanceName = "master";
                });
            }
            else
            {
                logger.LogInformation("Not using redis for session management!");
            }

            return services;
        }

        private static string GetHostEntryForUrl(string host, ILogger logger)
        {

            var addresses = Dns.GetHostEntryAsync(host).Result.AddressList;

            if (!addresses.Any())
            {
                logger.LogError($"Could not resolve IP address for redis instance : {host}");
                throw new Exception($"No redis instance could be found for host {host}");
            }

            if (addresses.Length > 1)
            {
                logger.LogWarning($"Multple IP address for redis instance : {host} attempting to use first");
            }

            return addresses.First().ToString();
        }
    }
}
