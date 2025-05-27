namespace StockportContentApi.Extensions;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Add all custom contentful factories
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddContentfulFactories(this IServiceCollection services)
    {
        services.AddSingleton<IContentfulFactory<ContentfulVideo, Video>>(p => new VideoContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulPrivacyNotice, Topic>, PrivacyNoticeParentTopicContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulProfile, Topic>, ProfileParentTopicContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>, PrivacyNoticeContentfulFactory>();
        services.AddSingleton<IContentfulFactory<Asset, Document>, DocumentContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulContactUsId, ContactUsId>, ContactUsIdContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulReference, Crumb>, CrumbContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulCarouselContent, CarouselContent>, CarouselContentContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulReference, SubItem>, SubItemContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulReference, ContentBlock>, ContentBlockContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulHomepage, Homepage>, HomepageContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulAlert, Alert>, AlertContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>, ContactUsCategoryContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>, RedirectContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulEventHomepage, EventHomepage>, EventHomepageContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulExternalLink, ExternalLink>, ExternalLinkContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulEventBanner, EventBanner>, EventBannerContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner>, SpotlightOnBannerContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>, SocialMediaLinkContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulSection, Section>, SectionContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulEvent, Event>, EventContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulInlineQuote, InlineQuote>, InlineQuoteContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulProfile, Profile>, ProfileContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulPayment, Payment>, PaymentContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment>, ServicePayPaymentContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulTopic, Topic>, TopicContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>, CallToActionBannerContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulDirectory, Directory>, DirectoryContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>, DirectoryEntryContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulShowcase, Showcase>, ShowcaseContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulLandingPage, LandingPage>, LandingPageContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulFooter, Footer>, FooterContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulSiteHeader, SiteHeader>, SiteHeaderContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulNews, News>, NewsContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulNewsRoom, Newsroom>, NewsRoomContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulEventCategory, EventCategory>, EventCategoryContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulArticle, Article>, ArticleContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulDocumentPage, DocumentPage>, DocumentPageContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>, TopicSiteMapContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulTrivia, Trivia>, TriviaContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>, ArticleSiteMapContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulAtoZ, AtoZ>, AtoZContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulArticle, Topic>, ParentTopicContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulStartPage, StartPage>, StartPageContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulGroupBranding, GroupBranding>, GroupBrandingContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulContactUsArea, ContactUsArea>, ContactUsAreaContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>, CommsContentfulFactory>();

        return services;
    }

    /// <summary>
    ///     Add cache services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="useRedisSession"></param>
    /// <returns></returns>
    public static IServiceCollection AddCache(this IServiceCollection services,
                                            bool useRedisSession,
                                            string _appEnvironment,
                                            IConfiguration configuration,
                                            Serilog.ILogger logger,
                                            bool useLocalCache = true)
    {
        logger.Information(
            $"CONTENTAPI : ServiceCollectionsExtensions : AddCache : Configure redis for session management - TokenStoreUrl: {configuration["TokenStoreUrl"]} Enabled: {useRedisSession}");

        if (useRedisSession)
        {
            string redisUrl = configuration["TokenStoreUrl"];
            logger.Information($"CONTENTAPI : ServiceCollectionsExtensions : AddCache : Using Redis URL {redisUrl}");

            string redisIp = redisUrl;
            if (!_appEnvironment.Equals("local"))
            {
                redisIp = GetHostEntryForUrl(redisUrl, logger);
                logger.Information($"CONTENTAPI : ServiceCollectionsExtensions : AddCache : Using Redis IP {redisIp}");
            }

            string name = Assembly.GetEntryAssembly()?.GetName().Name;

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = new()
                {
                    EndPoints =
                    {
                        redisIp
                    },
                    ClientName = name,
                    SyncTimeout = 30000,
                    AsyncTimeout = 30000,
                    SocketManager = SocketManager.ThreadPool
                };
            });

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisIp);
            logger.Information(
                $"CONTENTAPI : ServiceCollectionExtensions : Add Cache : Using Redis for session management - url {redisUrl}, ip {redisIp}, Name {name}");
            services.AddDataProtection().PersistKeysToStackExchangeRedis(redis, $"{name}DataProtection-Keys");
        }
        else
        {
            logger.Information(
                "CONTENTAPI : ServiceCollectionExtensions : Add Cache : Not using redis for session management, falling back to memory cache");
            services.AddDistributedMemoryCache();
        }

        if (useRedisSession || useLocalCache)
            services.AddScoped<IDistributedCacheWrapper>(p =>
                new DistributedCacheWrapper(p.GetService<IDistributedCache>()));

        services.AddSingleton<ICache>(p => new Cache(p.GetService<IDistributedCacheWrapper>(),
            p.GetService<ILogger<ICache>>(), useRedisSession, useLocalCache));

        return services;
    }

    /// <summary>
    ///     Add custom contentful configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddCacheKeyConfig(this IServiceCollection services, IConfiguration configuration)
    {
        Func<string, CacheKeyConfig> createCacheKeyConfig = businessId =>
            new CacheKeyConfig(businessId)
                .Add($"{businessId.ToUpper()}_EventsCacheKey", configuration[$"{businessId}:EventsCacheKey"])
                .Add($"{businessId.ToUpper()}_NewsCacheKey", configuration[$"{businessId}:NewsCacheKey"])
                .Build();
       
        services.AddTransient(_ => createCacheKeyConfig);

        return services;
    }

    /// <summary>
    ///     Add custom contentful configuration
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddContentfulConfig(this IServiceCollection services, IConfiguration configuration)
    {
        Func<string, ContentfulConfig> createConfig = businessId =>
            new ContentfulConfig(businessId)
                .Add("DELIVERY_URL", configuration["Contentful:DeliveryUrl"])
                .Add($"{businessId.ToUpper()}_SPACE", configuration[$"{businessId}:Space"])
                .Add($"{businessId.ToUpper()}_ACCESS_KEY", configuration[$"{businessId}:AccessKey"])
                .Add($"{businessId.ToUpper()}_MANAGEMENT_KEY", configuration[$"{businessId}:ManagementKey"])
                .Add($"{businessId.ToUpper()}_ENVIRONMENT", configuration[$"{businessId}:Environment"])
                .Build();

        services.AddTransient(_ => createConfig);

        return services;
    }

    /// <summary>
    ///     Add contentful clients
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddContentfulClients(this IServiceCollection services)
    {
        services.AddHttpClient<IContentfulClientManager, ContentfulClientManager>();

        return services;
    }

    /// <summary>
    ///     Add repositries
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<Func<string, IAtoZRepository>>(serviceProvider =>
            (contentfulConfig) =>
                new AtoZRepository(
                    serviceProvider.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    serviceProvider.GetService<IContentfulClientManager>(),
                    serviceProvider.GetService<IContentfulFactory<ContentfulAtoZ, AtoZ>>(),
                    serviceProvider.GetService<ITimeProvider>(),
                    serviceProvider.GetService<ICache>(),
                    serviceProvider.GetService<IConfiguration>(),
                    serviceProvider.GetService<ILogger>()));

        services.AddSingleton<Func<string, IPrivacyNoticeRepository>>(serviceProvider =>
            (contentfulConfig) =>
                new PrivacyNoticeRepository(
                    serviceProvider.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    serviceProvider.GetService<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>>(),
                    serviceProvider.GetService<IContentfulClientManager>()));

        services.AddSingleton<Func<ContentfulConfig, IAssetRepository>>(p =>
        {
            return x => new AssetRepository(x,
                p.GetService<IContentfulClientManager>(),
                p.GetService<ILogger<AssetRepository>>());
        });

        services.AddSingleton<Func<string, string, IArticleRepository>>(p =>
            (contentfulConfig, cacheKeyConfig) =>
                new ArticleRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<ITimeProvider>(),
                    p.GetService<IContentfulFactory<ContentfulArticle, Article>>(),
                    p.GetService<IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>>(),
                    p.GetService<IVideoRepository>(),
                    new(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                        p.GetService<Func<string, CacheKeyConfig>>()(cacheKeyConfig),
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                        p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                        p.GetService<ICache>(),
                        p.GetService<IConfiguration>()),
                    p.GetService<ICache>(),
                    p.GetService<IOptions<RedisExpiryConfiguration>>()));

        services.AddSingleton<Func<string, IDocumentPageRepository>>(serviceProvider =>
            (businessId) =>
                new DocumentPageRepository(serviceProvider.GetService<Func<string, ContentfulConfig>>()(businessId),
                    serviceProvider.GetService<IContentfulClientManager>(),
                    serviceProvider.GetService<IContentfulFactory<ContentfulDocumentPage, DocumentPage>>(),
                    serviceProvider.GetService<ICache>()));

        services.AddSingleton<IVideoRepository>(p =>
           new VideoRepository(p.GetService<TwentyThreeConfig>(), p.GetService<IHttpClient>()));

        services.AddSingleton<Func<string, string, IEventRepository>>(p =>
            (contentfulConfig, cacheKeyConfig) =>
                new EventRepository(
                    p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<Func<string, CacheKeyConfig>>()(cacheKeyConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<ITimeProvider>(),
                    p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                    p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                    p.GetService<ICache>(),
                    p.GetService<IConfiguration>()));

        services.AddSingleton<Func<string, IDirectoryRepository>>(p =>
            (contentfulConfig) =>
                new DirectoryRepository(
                    p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulDirectory, Directory>>(),
                    p.GetService<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>>(),
                    p.GetService<ICache>(),
                    p.GetService<IOptions<RedisExpiryConfiguration>>()
                ));

        services.AddSingleton<Func<string, IManagementRepository>>(p =>
            (contentfulConfig) =>
                new ManagementRepository(
                    p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<ILogger<EventRepository>>()));

        services.AddSingleton<Func<string, string, IShowcaseRepository>>(p =>
            (contentfulConfig, cacheKeyConfig) =>
                new ShowcaseRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulFactory<ContentfulShowcase, Showcase>>(),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulNews, News>>(),
                    new(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                        p.GetService<Func<string, CacheKeyConfig>>()(cacheKeyConfig),
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                        p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                        p.GetService<ICache>(),
                        p.GetService<IConfiguration>()),
                    p.GetService<ILogger<ShowcaseRepository>>()));

        services.AddSingleton<Func<string, string, ILandingPageRepository>>(p =>
            (contentfulConfig, cacheKeyConfig) =>
                new LandingPageRepository(
                    p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulFactory<ContentfulLandingPage, LandingPage>>(),
                    p.GetService<IContentfulClientManager>(),
                    new(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                        p.GetService<Func<string, CacheKeyConfig>>()(cacheKeyConfig),
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                        p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                        p.GetService<ICache>(),
                        p.GetService<IConfiguration>()),
                    new(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<IContentfulFactory<ContentfulNews, News>>(),
                        p.GetService<IContentfulFactory<ContentfulNewsRoom, Newsroom>>(),
                        p.GetService<ICache>(),
                        p.GetService<IConfiguration>(),
                        new(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                            p.GetService<Func<string, CacheKeyConfig>>()(cacheKeyConfig),
                            p.GetService<IContentfulClientManager>(),
                            p.GetService<ITimeProvider>(),
                            p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                            p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                            p.GetService<ICache>(),
                            p.GetService<IConfiguration>())),
                    p.GetService<IContentfulFactory<ContentfulProfile, Profile>>()));

        services.AddSingleton<Func<string, IProfileRepository>>(p =>
            (contentfulConfig) =>
                new ProfileRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulProfile, Profile>>()));

        services.AddSingleton<Func<string, IPaymentRepository>>( p =>
            (contentfulConfig) =>
                new PaymentRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulPayment, Payment>>()));

        services.AddSingleton<Func<string, IServicePayPaymentRepository>>(p =>
            (contentfulConfig) =>
                new ServicePayPaymentRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment>>()));

        services.AddSingleton<Func<string, string, IEventCategoryRepository>>(p =>
            (contentfulConfig, cacheKeyConfig) =>
                new EventCategoryRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<Func<string, CacheKeyConfig>>()(cacheKeyConfig),
                    p.GetService<IContentfulFactory<ContentfulEventCategory, EventCategory>>(),
                    p.GetService<IContentfulClientManager>(), p.GetService<ICache>(), p.GetService<IConfiguration>()));

        services.AddSingleton<Func<string, IHomepageRepository>>(p =>
            (contentfulConfig) =>
                new HomepageRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulHomepage, Homepage>>()));

        services.AddSingleton<Func<string, IStartPageRepository>>(p =>
            (contentfulConfig) =>
                new StartPageRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulStartPage, StartPage>>(),
                    p.GetService<ITimeProvider>()));
        
        services.AddSingleton<Func<string, IFooterRepository>>(p =>
            (contentfulConfig) =>
                new FooterRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulFooter, Footer>>()));

        services.AddSingleton<Func<string, ISiteHeaderRepository>>(p =>
            (contentfulConfig) =>
                new SiteHeaderRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulSiteHeader, SiteHeader>>()));

        services.AddSingleton<Func<string, INewsRepository>>(p =>
            (contentfulConfig) =>
                new NewsRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<ITimeProvider>(),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulNews, News>>(),
                    p.GetService<IContentfulFactory<ContentfulNewsRoom, Newsroom>>(),
                    p.GetService<ICache>(),
                    p.GetService<IConfiguration>(),
                    new(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                        p.GetService<Func<string, CacheKeyConfig>>()(contentfulConfig),
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                        p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                        p.GetService<ICache>(),
                        p.GetService<IConfiguration>())));

        services.AddSingleton<Func<string, ISectionRepository>>(p =>
            (contentfulConfig) =>
                new SectionRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulFactory<ContentfulSection, Section>>(),
                    p.GetService<IContentfulClientManager>()));
        
        services.AddSingleton<Func<string, ITopicRepository>>(p =>
            (contentfulConfig) =>
                new TopicRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulTopic, Topic>>(),
                    p.GetService<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>>()));
        
        services.AddSingleton<RedirectsRepository>();
        services.AddSingleton<IAuthenticationHelper>(p => new AuthenticationHelper());
        
        services.AddSingleton<Func<string, IContactUsIdRepository>>(p =>
            (contentfulConfig) =>
                new ContactUsIdRepository( p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulFactory<ContentfulContactUsId, ContactUsId>>(),
                    p.GetService<IContentfulClientManager>()));

        services.AddSingleton<Func<string, IContactUsAreaRepository>>( p =>
            (contentfulConfig) =>
                new ContactUsAreaRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulContactUsArea, ContactUsArea>>()));

        services.AddSingleton<Func<string, ICommsRepository>>(p =>
            (contentfulConfig) =>
                new CommsRepository(p.GetService<Func<string, ContentfulConfig>>()(contentfulConfig),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>>()));

        return services;
    }

    /// <summary>
    ///     Add auto mapper
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAutoMapper(this IServiceCollection services)
    {
        MapperConfiguration config = new(cfg => { cfg.AddProfile(new AutoMapperConfig()); });

        services.AddSingleton(p => config.CreateMapper());

        return services;
    }

    /// <summary>
    ///     Add redirects
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRedirects(this IServiceCollection services, IConfiguration configuration)
    {
        List<string> redirectBusinessIds = new();
        configuration.GetSection("RedirectBusinessIds").Bind(redirectBusinessIds);
        services.AddSingleton(new RedirectBusinessIds(redirectBusinessIds));

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IDocumentService>(p =>
            new DocumentsService(p.GetService<Func<ContentfulConfig, IAssetRepository>>(),
                p.GetService<IContentfulFactory<Asset, Document>>(),
                p.GetService<IContentfulConfigBuilder>()));

        return services;
    }

    public static IServiceCollection AddBuilders(this IServiceCollection services)
    {
        services.AddSingleton<IContentfulConfigBuilder>(p =>
            new ContentfulConfigBuilder(p.GetService<IConfiguration>()));

        return services;
    }

    private static string GetHostEntryForUrl(string host, Serilog.ILogger logger)
    {
        IPAddress[] addresses = Dns.GetHostEntryAsync(host).Result.AddressList;

        if (!addresses.Any())
        {
            logger.Error($"Could not resolve IP address for redis instance : {host}");
            throw new($"No redis instance could be found for host {host}");
        }

        if (addresses.Length > 1)
            logger.Warning($"Multple IP address for redis instance : {host} attempting to use first");

        return addresses.First().ToString();
    }
}