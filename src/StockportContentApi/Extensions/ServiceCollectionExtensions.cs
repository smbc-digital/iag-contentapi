using StockportContentApi.Config;

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
        services.AddSingleton<IContentfulFactory<ContentfulOrganisation, Organisation>, OrganisationContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>, GroupSubCategoryContentfulFactory>();
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
        services.AddSingleton<IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>, GroupHomepageContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulEventBanner, EventBanner>, EventBannerContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulSpotlightOnBanner, SpotlightOnBanner>, SpotlightOnBannerContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>, SocialMediaLinkContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulSection, Section>, SectionContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulEvent, Event>, EventContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulInlineQuote, InlineQuote>, InlineQuoteContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulProfile, Profile>, ProfileContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulGroup, Group>, GroupContentfulFactory>();
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
        services.AddSingleton<IContentfulFactory<ContentfulGroupCategory, GroupCategory>, GroupCategoryContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulEventCategory, EventCategory>, EventCategoryContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulArticle, Article>, ArticleContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulDocumentPage, DocumentPage>, DocumentPageContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>, TopicSiteMapContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulTrivia, Trivia>, TriviaContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>, ArticleSiteMapContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulAtoZ, AtoZ>, AtoZContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulArticle, Topic>, ParentTopicContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulStartPage, StartPage>, StartPageContentfulFactory>();
        services.AddSingleton<IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>, GroupAdvisorContentfulFactory>();
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
        services.AddSingleton<Func<string, IAtoZRepository>>(serviceProvider => (businessId) =>
        {
            return new AtoZRepository(serviceProvider.GetService<Func<string, ContentfulConfig>>()(businessId),
                    serviceProvider.GetService<IContentfulClientManager>(),
                    serviceProvider.GetService<IContentfulFactory<ContentfulAtoZ, AtoZ>>(),
                    serviceProvider.GetService<ITimeProvider>(),
                    serviceProvider.GetService<ICache>(),
                    serviceProvider.GetService<IConfiguration>(),
                    serviceProvider.GetService<ILogger>());
        });

        services.AddSingleton<Func<string, IPrivacyNoticeRepository>>(serviceProvider => (businessId) =>
        {
            return new PrivacyNoticeRepository(serviceProvider.GetService<Func<string, ContentfulConfig>>()(businessId),
                serviceProvider.GetService<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>>(),
                serviceProvider.GetService<IContentfulClientManager>());
        });

        // services.AddSingleton<Func<string, IAssetRepository>>(serviceProvider => (businessId) =>
        // {
        //     return new AssetRepository(serviceProvider.GetService<Func<string, ContentfulConfig>>()(businessId),
        //         serviceProvider.GetService<IContentfulClientManager>(),
        //         serviceProvider.GetService<ILogger<AssetRepository>>());
        // });

        services.AddSingleton<Func<ContentfulConfig, IAssetRepository>>(p =>
        {
            return x => new AssetRepository(x,
                p.GetService<IContentfulClientManager>(),
                p.GetService<ILogger<AssetRepository>>());
        });
        
        services.AddSingleton<Func<string, IArticleRepository>>(serviceProvider => (businessId) =>
        {
            return new ArticleRepository(serviceProvider.GetService<Func<string, ContentfulConfig>>()(businessId),
                    serviceProvider.GetService<IContentfulClientManager>(),
                    serviceProvider.GetService<ITimeProvider>(),
                    serviceProvider.GetService<IContentfulFactory<ContentfulArticle, Article>>(),
                    serviceProvider.GetService<IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>>(),
                    serviceProvider.GetService<IVideoRepository>(),
                    serviceProvider.GetService<ICache>(),
                    serviceProvider.GetService<IOptions<RedisExpiryConfiguration>>());
        });

        services.AddSingleton<Func<string, IDocumentPageRepository>>(serviceProvider => (businessId) =>
        {
            return new DocumentPageRepository(serviceProvider.GetService<Func<string, ContentfulConfig>>()(businessId),
                    serviceProvider.GetService<IContentfulClientManager>(),
                    serviceProvider.GetService<IContentfulFactory<ContentfulDocumentPage, DocumentPage>>(),
                    serviceProvider.GetService<ICache>());
        });

        services.AddSingleton<IVideoRepository>(p =>
           new VideoRepository(p.GetService<TwentyThreeConfig>(), p.GetService<IHttpClient>()));

        services.AddSingleton<Func<ContentfulConfig, CacheKeyConfig, EventRepository>>(p =>
            (contentfulConfig, cacheKeyConfig) =>
                new(
                    contentfulConfig,
                    cacheKeyConfig,
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<ITimeProvider>(),
                    p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                    p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                    p.GetService<ICache>(),
                    p.GetService<IConfiguration>()));

        services.AddSingleton<Func<ContentfulConfig, DirectoryRepository>>(p =>
            config =>
                new(
                    config,
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulDirectory, Directory>>(),
                    p.GetService<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>>(),
                    p.GetService<ICache>(),
                    p.GetService<IOptions<RedisExpiryConfiguration>>(),
                    p.GetService<ILogger<DirectoryRepository>>()
                ));

        services.AddSingleton<Func<ContentfulConfig, ManagementRepository>>(p =>
            config =>
                new(
                    config,
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<ILogger<EventRepository>>()));

        services.AddSingleton<Func<ContentfulConfig, CacheKeyConfig, ShowcaseRepository>>(p =>
            (contentfulConfig, cacheKeyConfig) =>
                new(contentfulConfig, p.GetService<IContentfulFactory<ContentfulShowcase, Showcase>>(),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulNews, News>>(),
                    new(contentfulConfig,
                        cacheKeyConfig,
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                        p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                        p.GetService<ICache>(),
                        p.GetService<IConfiguration>()),
                    p.GetService<ILogger<ShowcaseRepository>>()
                )
            );

        services.AddSingleton<Func<ContentfulConfig, CacheKeyConfig, LandingPageRepository>>(p =>
            (contentfulConfig, cacheKeyConfig) =>
                new(contentfulConfig,
                    p.GetService<IContentfulFactory<ContentfulLandingPage, LandingPage>>(),
                    p.GetService<IContentfulClientManager>(),
                    new(contentfulConfig,
                        cacheKeyConfig,
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                        p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                        p.GetService<ICache>(),
                        p.GetService<IConfiguration>()),
                    new(contentfulConfig,
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<IContentfulFactory<ContentfulNews, News>>(),
                        p.GetService<IContentfulFactory<ContentfulNewsRoom, Newsroom>>(),
                        p.GetService<ICache>(),
                        p.GetService<IConfiguration>()),
                    p.GetService<IContentfulFactory<ContentfulProfile, Profile>>()
                )
            );

        services.AddSingleton<Func<ContentfulConfig, IProfileRepository>>(
            p =>
            {
                return x => new ProfileRepository(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulProfile, Profile>>());
            });
        services.AddSingleton<Func<ContentfulConfig, PaymentRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulPayment, Payment>>());
            });
        services.AddSingleton<Func<ContentfulConfig, ServicePayPaymentRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment>>());
            });

        services.AddSingleton<Func<ContentfulConfig, GroupCategoryRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(),
                    p.GetService<IContentfulClientManager>());
            });
            

        services.AddSingleton<Func<ContentfulConfig, CacheKeyConfig, EventCategoryRepository>>(p =>
            (contentfulConfig, cacheKeyConfig) =>
                new(contentfulConfig, cacheKeyConfig, p.GetService<IContentfulFactory<ContentfulEventCategory, EventCategory>>(),
                    p.GetService<IContentfulClientManager>(), p.GetService<ICache>(), p.GetService<IConfiguration>())
            );

        services.AddSingleton<Func<ContentfulConfig, HomepageRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulHomepage, Homepage>>());
            });
        services.AddSingleton<Func<ContentfulConfig, StartPageRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulStartPage, StartPage>>(), p.GetService<ITimeProvider>());
            });
        services.AddSingleton<Func<ContentfulConfig, FooterRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulFooter, Footer>>());
            });

        services.AddSingleton<Func<ContentfulConfig, SiteHeaderRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulSiteHeader, SiteHeader>>());
            });

        services.AddSingleton<Func<ContentfulConfig, NewsRepository>>(
            p =>
            {
                return x => new(x, p.GetService<ITimeProvider>(), p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulNews, News>>(),
                    p.GetService<IContentfulFactory<ContentfulNewsRoom, Newsroom>>(), p.GetService<ICache>(),
                    p.GetService<IConfiguration>());
            });

        services.AddSingleton<Func<ContentfulConfig, SectionRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulFactory<ContentfulSection, Section>>(),
                    p.GetService<IContentfulClientManager>());
            });
        services.AddSingleton<Func<ContentfulConfig, TopicRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulTopic, Topic>>(),
                    p.GetService<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>>());
            });
        services.AddSingleton<RedirectsRepository>();
        services.AddSingleton<IAuthenticationHelper>(p => new AuthenticationHelper());
        services.AddSingleton<Func<ContentfulConfig, CacheKeyConfig, IGroupRepository>>(p =>
                (contentfulConfig, cacheKeyConfig) =>
                    new GroupRepository(contentfulConfig, p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulGroup, Group>>(),
                        p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(),
                        p.GetService<IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>>(),
                        new(contentfulConfig,
                            cacheKeyConfig,
                            p.GetService<IContentfulClientManager>(),
                            p.GetService<ITimeProvider>(),
                            p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                            p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                            p.GetService<ICache>(),
                            p.GetService<IConfiguration>()),
                        p.GetService<ICache>(),
                        p.GetService<IConfiguration>())
        );

        services.AddSingleton<Func<ContentfulConfig, ContactUsIdRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulFactory<ContentfulContactUsId, ContactUsId>>(),
                    p.GetService<IContentfulClientManager>());
            });

        services.AddSingleton<Func<ContentfulConfig, OrganisationRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulFactory<ContentfulOrganisation, Organisation>>(),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<Func<ContentfulConfig, IGroupRepository>>().Invoke(x));
            });

        services.AddSingleton<Func<ContentfulConfig, IGroupAdvisorRepository>>(
            p =>
            {
                return x => new GroupAdvisorRepository(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>>());
            });

        services.AddSingleton<Func<ContentfulConfig, ContactUsAreaRepository>>(
            p =>
            {
                return x => new(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulContactUsArea, ContactUsArea>>());
            });

        services.AddSingleton<Func<ContentfulConfig, CommsRepository>>(
            p =>
            {
                return config =>
                    new(
                        config,
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>>()
                    );
            });

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
                p.GetService<Func<ContentfulConfig, IGroupAdvisorRepository>>(),
                p.GetService<Func<ContentfulConfig, IGroupRepository>>(),
                p.GetService<IContentfulFactory<Asset, Document>>(), p.GetService<IContentfulConfigBuilder>(),
                p.GetService<ILoggedInHelper>()));

        return services;
    }

    public static IServiceCollection AddBuilders(this IServiceCollection services)
    {
        services.AddSingleton<IContentfulConfigBuilder>(p =>
            new ContentfulConfigBuilder(p.GetService<IConfiguration>()));

        return services;
    }

    public static IServiceCollection AddGroupConfiguration(this IServiceCollection services,
        IConfiguration configuration, Serilog.ILogger logger)
    {
        if (!string.IsNullOrEmpty(configuration["group:authenticationKey"]))
        {
            GroupAuthenticationKeys groupKeys = new() { Key = configuration["group:authenticationKey"] };
            services.AddSingleton(groupKeys);

            services.AddSingleton<IJwtDecoder>(p =>
                new JwtDecoder(p.GetService<GroupAuthenticationKeys>(), p.GetService<ILogger<JwtDecoder>>()));
        }
        else
            logger.Information("Group authenticationKey not found.");

        return services;
    }

    public static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        services.AddTransient<ILoggedInHelper>(p => new LoggedInHelper(p.GetService<IHttpContextAccessor>(),
            p.GetService<IJwtDecoder>(), p.GetService<ILogger<LoggedInHelper>>()));

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