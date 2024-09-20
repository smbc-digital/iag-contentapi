using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace StockportContentApi.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add all custom contentful factories
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddContentfulFactories(this IServiceCollection services)
    {
        services.AddSingleton<IContentfulFactory<ContentfulVideo, Video>>(p => new VideoContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulPrivacyNotice, Topic>>(p => new PrivacyNoticeParentTopicContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(), p.GetService<ITimeProvider>()));
        services.AddSingleton<IContentfulFactory<ContentfulProfile, Topic>>(p => new ProfileParentTopicContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(), p.GetService<ITimeProvider>()));
        services.AddSingleton<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>>(p => new PrivacyNoticeContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(), p.GetService<IContentfulFactory<ContentfulPrivacyNotice, Topic>>()));
        services.AddSingleton<IContentfulFactory<ContentfulOrganisation, Organisation>>(p => new OrganisationContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>(p => new GroupSubCategoryContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulOrganisation, Organisation>>(p => new OrganisationContentfulFactory());
        services.AddSingleton<IContentfulFactory<Asset, Document>>(p => new DocumentContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulContactUsId, ContactUsId>>(p => new ContactUsIdContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulReference, Crumb>>(p => new CrumbContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulCarouselContent, CarouselContent>>(p => new CarouselContentContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulReference, SubItem>>(p => new SubItemContentfulFactory(p.GetService<ITimeProvider>()));
        services.AddSingleton<IContentfulFactory<ContentfulReference, ContentBlock>>(p => new ContentBlockContentfulFactory(p.GetService<ITimeProvider>()));
        services.AddSingleton<IContentfulFactory<ContentfulHomepage, Homepage>>(p => new HomepageContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(),
            p.GetService<IContentfulFactory<ContentfulGroup, Group>>(),
            p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
            p.GetService<IContentfulFactory<ContentfulCarouselContent, CarouselContent>>(),
            p.GetService<ITimeProvider>(),
            p.GetService<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>(),
            p.GetService<IContentfulFactory<IEnumerable<ContentfulSpotlightOnBanner>, IEnumerable<SpotlightOnBanner>>>()));
        services.AddSingleton<IContentfulFactory<ContentfulAlert, Alert>>(p => new AlertContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>>(p => new ContactUsCategoryContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulRedirect, BusinessIdToRedirects>>(p => new RedirectContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(p => new EventHomepageContentfulFactory(p.GetService<ITimeProvider>()));
        services.AddSingleton<IContentfulFactory<ContentfulExternalLink, ExternalLink>>(p => new ExternalLinkContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>>(p => new GroupHomepageContentfulFactory(p.GetService<IContentfulFactory<ContentfulGroup, Group>>(), p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(), p.GetService<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>(), p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<IContentfulFactory<ContentfulEventBanner, EventBanner>>()));
        services.AddSingleton<IContentfulFactory<ContentfulEventBanner, EventBanner>>(p => new EventBannerContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>>(p => new SpotlightBannerContentfulFactory());
        services.AddSingleton<IContentfulFactory<IEnumerable<ContentfulSpotlightOnBanner>, IEnumerable<SpotlightOnBanner>>>(p => new SpotlightOnBannerContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>(p => new SocialMediaLinkContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulSection, Section>>(p => new SectionContentfulFactory(p.GetService<IContentfulFactory<ContentfulProfile, Profile>>(),
            p.GetService<IContentfulFactory<Asset, Document>>(),
            p.GetService<IVideoRepository>(),
            p.GetService<ITimeProvider>(),
            p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
            p.GetService<IContentfulFactory<ContentfulGroupBranding, GroupBranding>>()));
        services.AddSingleton<IContentfulFactory<ContentfulEvent, Event>>(p => new EventContentfulFactory(p.GetService<IContentfulFactory<Asset, Document>>(),
            p.GetService<IContentfulFactory<ContentfulGroup, Group>>(),
            p.GetService<IContentfulFactory<ContentfulEventCategory, EventCategory>>(),
            p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
            p.GetService<ITimeProvider>()));
        services.AddSingleton<IContentfulFactory<ContentfulInlineQuote, InlineQuote>>(p => new InlineQuoteContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulProfile, Profile>>(p => new ProfileContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(), p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<IContentfulFactory<ContentfulTrivia, Trivia>>(), p.GetService<IContentfulFactory<ContentfulInlineQuote, InlineQuote>>(), p.GetService<IContentfulFactory<ContentfulEventBanner, EventBanner>>(), p.GetService<IContentfulFactory<ContentfulProfile, Topic>>()));
        services.AddSingleton<IContentfulFactory<ContentfulGroup, Group>>(p => new GroupContentfulFactory(p.GetService<IContentfulFactory<ContentfulOrganisation, Organisation>>(), p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(), p.GetService<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>(), p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<Asset, Document>>(), p.GetService<IContentfulFactory<ContentfulGroupBranding, GroupBranding>>(), p.GetService<IContentfulFactory<ContentfulAlert, Alert>>()));
        services.AddSingleton<IContentfulFactory<ContentfulPayment, Payment>>(p => new PaymentContentfulFactory(p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<ContentfulReference, Crumb>>()));
        services.AddSingleton<IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment>>(p => new ServicePayPaymentContentfulFactory(
            p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
            p.GetService<ITimeProvider>(),
            p.GetService<IContentfulFactory<ContentfulReference, Crumb>>()));
        services.AddSingleton<IContentfulFactory<ContentfulTopic, Topic>>(p => new TopicContentfulFactory(
            p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(),
            p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(),
            p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
            p.GetService<IContentfulFactory<ContentfulEventBanner, EventBanner>>(),
            p.GetService<IContentfulFactory<ContentfulCarouselContent, CarouselContent>>(),
            p.GetService<ITimeProvider>(),
            p.GetService<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>(),
            p.GetService<IContentfulFactory<ContentfulGroupBranding, GroupBranding>>()
            )
        );
        services.AddSingleton<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>(p => new CallToActionBannerContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulDirectory, Directory>>(p => new DirectoryContentfulFactory(
            p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(),
            p.GetService<IContentfulFactory<ContentfulExternalLink, ExternalLink>>(),
            p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), 
            p.GetService<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>(), 
            p.GetService<ITimeProvider>(),
            p.GetService<IContentfulFactory<ContentfulEventBanner, EventBanner>>(),
            p.GetService<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>>()));

        services.AddSingleton<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>>(p => new DirectoryEntryContentfulFactory(
            p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
            p.GetService<IContentfulFactory<ContentfulGroupBranding, GroupBranding>>(),
            p.GetService<ITimeProvider>()));
        
        services.AddSingleton<IContentfulFactory<ContentfulShowcase, Showcase>>
        (p => new ShowcaseContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(), p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(), p.GetService<ITimeProvider>(),
            p.GetService<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>(), p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<IContentfulFactory<ContentfulProfile, Profile>>(),
            p.GetService<IContentfulFactory<ContentfulTrivia, Trivia>>(),
            p.GetService<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>(),
            p.GetService<IContentfulFactory<ContentfulVideo, Video>>(),
            p.GetService<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>>()));

        services.AddSingleton<IContentfulFactory<ContentfulLandingPage, LandingPage>>
        (p => new LandingPageContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(), p.GetService<ITimeProvider>(),
            p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<IContentfulFactory<ContentfulReference, ContentBlock>>()));

        services.AddSingleton<IContentfulFactory<ContentfulFooter, Footer>>
            (p => new FooterContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(), p.GetService<IContentfulFactory<ContentfulSocialMediaLink,
            SocialMediaLink>>()));

        services.AddSingleton<IContentfulFactory<ContentfulNews, News>>(p => new NewsContentfulFactory(p.GetService<IVideoRepository>(),
            p.GetService<IContentfulFactory<Asset, Document>>(), p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<ITimeProvider>(),
            p.GetService<IContentfulFactory<ContentfulProfile, Profile>>()));

        services.AddSingleton<IContentfulFactory<ContentfulNewsRoom, Newsroom>>(p => new NewsRoomContentfulFactory(p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<ITimeProvider>()));
        services.AddSingleton<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(p => new GroupCategoryContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulEventCategory, EventCategory>>(p => new EventCategoryContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulArticle, Article>>
        (p => new ArticleContentfulFactory(p.GetService<IContentfulFactory<ContentfulSection, Section>>(),
            p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(),
            p.GetService<IContentfulFactory<ContentfulProfile, Profile>>(),
            p.GetService<IContentfulFactory<ContentfulArticle, Topic>>(),
            p.GetService<IContentfulFactory<Asset, Document>>(),
            p.GetService<IVideoRepository>(),
            p.GetService<ITimeProvider>(),
            p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(),
            p.GetService<IContentfulFactory<ContentfulGroupBranding, GroupBranding>>(),
            p.GetService<IContentfulFactory<ContentfulReference, SubItem>>()
            ));
        services.AddSingleton<IContentfulFactory<ContentfulDocumentPage, DocumentPage>>
        (p => new DocumentPageContentfulFactory(
            p.GetService<IContentfulFactory<Asset, Document>>(),
            p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(),
            p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(),
            p.GetService<ITimeProvider>()
            ));
        services.AddSingleton<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>>
            (p => new TopicSiteMapContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulTrivia, Trivia>>
            (p => new TriviaContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>>
            (p => new ArticleSiteMapContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulAtoZ, AtoZ>>
            (p => new AtoZContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulArticle, Topic>>(
            p => new ParentTopicContentfulFactory(
                p.GetService<IContentfulFactory<ContentfulReference, SubItem>>()
                , p.GetService<ITimeProvider>()));
        services.AddSingleton<IContentfulFactory<ContentfulStartPage, StartPage>>
            (p => new StartPageFactoryContentfulFactory(p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<IContentfulFactory<ContentfulReference, Crumb>>()));
        services.AddSingleton<IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>>
            (p => new GroupAdvisorContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulGroupBranding, GroupBranding>>
            (p => new GroupBrandingContentfulFactory());
        services.AddSingleton<IContentfulFactory<ContentfulContactUsArea, ContactUsArea>>
        (p => new ContactUsAreaContentfulFactory(p.GetService<IContentfulFactory<ContentfulReference, SubItem>>(), p.GetService<IContentfulFactory<ContentfulReference, Crumb>>(), p.GetService<ITimeProvider>(),
             p.GetService<IContentfulFactory<ContentfulAlert, Alert>>(), p.GetService<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>>()));
        services.AddSingleton<IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>>(_ => new CommsContentfulFactory(
            _.GetService<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>(),
            _.GetService<IContentfulFactory<ContentfulEvent, Event>>()
            ));

        return services;
    }

    /// <summary>
    /// Add cache services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="useRedisSession"></param>
    /// <returns></returns>
    public static IServiceCollection AddCache(this IServiceCollection services, bool useRedisSession, string _appEnvironment, IConfiguration configuration, Serilog.ILogger logger, bool useLocalCache = true)
    {
        logger.Information($"CONTENTAPI : ServiceCollectionsExtensions : AddCache : Configure redis for session management - TokenStoreUrl: {configuration["TokenStoreUrl"]} Enabled: {useRedisSession}");

        if (useRedisSession)
        {
            var redisUrl = configuration["TokenStoreUrl"];
            logger.Information($"CONTENTAPI : ServiceCollectionsExtensions : AddCache : Using Redis URL {redisUrl}");

            var redisIp = redisUrl;
            if (!_appEnvironment.Equals("local"))
            {
                redisIp = GetHostEntryForUrl(redisUrl, logger);
                logger.Information($"CONTENTAPI : ServiceCollectionsExtensions : AddCache : Using Redis IP {redisIp}");

            }

            var name = Assembly.GetEntryAssembly()?.GetName().Name;

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConfigurationOptions = new ConfigurationOptions
                {
                    EndPoints =
                        {
                            { redisIp }
                        },
                    ClientName = name,
                    SyncTimeout = 30000,
                    AsyncTimeout = 30000,
                    SocketManager = SocketManager.ThreadPool
                };
            });

            var redis = ConnectionMultiplexer.Connect(redisIp);
            logger.Information($"CONTENTAPI : ServiceCollectionExtensions : Add Cache : Using Redis for session management - url {redisUrl}, ip {redisIp}, Name {name}");
            services.AddDataProtection().PersistKeysToStackExchangeRedis(redis, $"{name}DataProtection-Keys");

        }
        else
        {
            logger.Information("CONTENTAPI : ServiceCollectionExtensions : Add Cache : Not using redis for session management, falling back to memory cache");
            services.AddDistributedMemoryCache();
        }

        if (useRedisSession || useLocalCache)
        { 
            services.AddScoped<IDistributedCacheWrapper>(p => new DistributedCacheWrapper(p.GetService<IDistributedCache>()));
        }

        services.AddSingleton<ICache>(p => new Cache(p.GetService<IDistributedCacheWrapper>(), p.GetService<ILogger<ICache>>(), useRedisSession, useLocalCache));

        return services;
    }

    /// <summary>
    /// Add custom contentful configuration
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
    /// Add contentful clients
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddContentfulClients(this IServiceCollection services)
    {
        services.AddHttpClient<IContentfulClientManager, ContentfulClientManager>();

        return services;
    }

    /// <summary>
    /// Add repositries
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddSingleton<Func<ContentfulConfig, AtoZRepository>>(p =>
            config =>
                new AtoZRepository(
                    config,
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulAtoZ, AtoZ>>(),
                    p.GetService<ITimeProvider>(),
                    p.GetService<ICache>(),
                    p.GetService<IConfiguration>(),
                    p.GetService<ILogger>()));

        services.AddSingleton<Func<ContentfulConfig, IPrivacyNoticeRepository>>(p => { return x => new PrivacyNoticeRepository(x, p.GetService<IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice>>(), p.GetService<IContentfulClientManager>()); });
        services.AddSingleton<Func<ContentfulConfig, IAssetRepository>>(p => { return x => new AssetRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<ILogger<AssetRepository>>()); });
        services.AddSingleton<Func<ContentfulConfig, ArticleRepository>>(p => { 
            return x => new ArticleRepository(x, p.GetService<IContentfulClientManager>(), 
            p.GetService<ITimeProvider>(), 
            p.GetService<IContentfulFactory<ContentfulArticle, Article>>(), 
            p.GetService<IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap>>(), 
            p.GetService<IVideoRepository>(), 
            p.GetService<ICache>(), 
            p.GetService<IOptions<RedisExpiryConfiguration>>()); 
        });

        services.AddSingleton<Func<ContentfulConfig, DocumentPageRepository>>(p => { return x => new DocumentPageRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulDocumentPage, DocumentPage>>(), p.GetService<ICache>()); });
        services.AddSingleton<IVideoRepository>(p => new VideoRepository(p.GetService<TwentyThreeConfig>(), p.GetService<IHttpClient>()));

        services.AddSingleton<Func<ContentfulConfig, EventRepository>>(p =>
            config =>
                new EventRepository(
                    config,
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<ITimeProvider>(),
                    p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                    p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                    p.GetService<ICache>(),
                    p.GetService<ILogger<EventRepository>>(),
                    p.GetService<IConfiguration>()));

        services.AddSingleton<Func<ContentfulConfig, DirectoryRepository>>(p =>
            config =>
                new DirectoryRepository(
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
                new ManagementRepository(
                    config,
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<ILogger<EventRepository>>()));

        services.AddSingleton<Func<ContentfulConfig, ShowcaseRepository>>(
            p => {
                return x => new ShowcaseRepository(x, p.GetService<IContentfulFactory<ContentfulShowcase, Showcase>>(),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulNews, News>>(),
                    new EventRepository(x,
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                        p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                        p.GetService<ICache>(),
                        p.GetService<ILogger<EventRepository>>(),
                        p.GetService<IConfiguration>()),
                    p.GetService<ILogger<ShowcaseRepository>>()
                );
            });

        services.AddSingleton<Func<ContentfulConfig, LandingPageRepository>>(
            p => {
                return x => new LandingPageRepository(x,
                    p.GetService<IContentfulFactory<ContentfulLandingPage, LandingPage>>(),
                    p.GetService<IContentfulClientManager>(),
                    p.GetService<IContentfulFactory<ContentfulNews, News>>(),
                    p.GetService<IContentfulFactory<ContentfulProfile, Profile>>()
                );
            });
        
        services.AddSingleton<Func<ContentfulConfig, IProfileRepository>>(
            p => { return x => new ProfileRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulProfile, Profile>>()); });
        services.AddSingleton<Func<ContentfulConfig, PaymentRepository>>(
            p => { return x => new PaymentRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulPayment, Payment>>()); });
        services.AddSingleton<Func<ContentfulConfig, ServicePayPaymentRepository>>(
            p => { return x => new ServicePayPaymentRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment>>()); });
        services.AddSingleton<Func<ContentfulConfig, GroupCategoryRepository>>(
            p => { return x => new GroupCategoryRepository(x, p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(), p.GetService<IContentfulClientManager>()); });
        services.AddSingleton<Func<ContentfulConfig, EventCategoryRepository>>(
            p => { return x => new EventCategoryRepository(x, p.GetService<IContentfulFactory<ContentfulEventCategory, EventCategory>>(), p.GetService<IContentfulClientManager>(), p.GetService<ICache>(), p.GetService<IConfiguration>()); });
        services.AddSingleton<Func<ContentfulConfig, HomepageRepository>>(
            p => { return x => new HomepageRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulHomepage, Homepage>>()); });
        services.AddSingleton<Func<ContentfulConfig, StartPageRepository>>(
            p => { return x => new StartPageRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulStartPage, StartPage>>(), p.GetService<ITimeProvider>()); });
        services.AddSingleton<Func<ContentfulConfig, FooterRepository>>(
            p => { return x => new FooterRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulFooter, Footer>>()); });
        services.AddSingleton<Func<ContentfulConfig, NewsRepository>>(
            p => { return x => new NewsRepository(x, p.GetService<ITimeProvider>(), p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulNews, News>>(), p.GetService<IContentfulFactory<ContentfulNewsRoom, Newsroom>>(), p.GetService<ICache>(), p.GetService<IConfiguration>()); });

        services.AddSingleton<Func<ContentfulConfig, SectionRepository>>(
            p => { return x => new SectionRepository(x, p.GetService<IContentfulFactory<ContentfulSection, Section>>(), p.GetService<IContentfulClientManager>(), p.GetService<ITimeProvider>()); });
        services.AddSingleton<Func<ContentfulConfig, TopicRepository>>(
            p => { return x => new TopicRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulTopic, Topic>>(), p.GetService<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>>()); });
        services.AddSingleton<RedirectsRepository>();
        services.AddSingleton<IAuthenticationHelper>(p => new AuthenticationHelper(p.GetService<ITimeProvider>()));
        services.AddSingleton<Func<ContentfulConfig, IGroupRepository>>(
            p => {
                return x => new GroupRepository(x, p.GetService<IContentfulClientManager>(),
                    p.GetService<ITimeProvider>(),
                    p.GetService<IContentfulFactory<ContentfulGroup, Group>>(),
                    p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(),
                    p.GetService<IContentfulFactory<ContentfulGroupHomepage, GroupHomepage>>(),
                    new EventRepository(x, p.GetService<IContentfulClientManager>(),
                        p.GetService<ITimeProvider>(),
                        p.GetService<IContentfulFactory<ContentfulEvent, Event>>(),
                        p.GetService<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>(),
                        p.GetService<ICache>(),
                        p.GetService<ILogger<EventRepository>>(),
                        p.GetService<IConfiguration>()),
                    p.GetService<ICache>(),
                    p.GetService<IConfiguration>());
            });
        
        services.AddSingleton<Func<ContentfulConfig, ContactUsIdRepository>>(
            p => { return x => new ContactUsIdRepository(x, p.GetService<IContentfulFactory<ContentfulContactUsId, ContactUsId>>(), p.GetService<IContentfulClientManager>()); });

        services.AddSingleton<Func<ContentfulConfig, OrganisationRepository>>(
            p => { return x => new OrganisationRepository(x, p.GetService<IContentfulFactory<ContentfulOrganisation, Organisation>>(), p.GetService<IContentfulClientManager>(), p.GetService<Func<ContentfulConfig, IGroupRepository>>().Invoke(x)); });

        services.AddSingleton<Func<ContentfulConfig, IGroupAdvisorRepository>>(
            p => {
                return x => new GroupAdvisorRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulGroupAdvisor, GroupAdvisor>>());
            });

        services.AddSingleton<Func<ContentfulConfig, ContactUsAreaRepository>>(
            p => { return x => new ContactUsAreaRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulContactUsArea, ContactUsArea>>()); });

        services.AddSingleton<Func<ContentfulConfig, CommsRepository>>(
            p => {
                return config =>
                    new CommsRepository(
                        config,
                        p.GetService<IContentfulClientManager>(),
                        p.GetService<IContentfulFactory<ContentfulCommsHomepage, CommsHomepage>>()
                    );
                });

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
    /// Add redirects
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRedirects(this IServiceCollection services, IConfiguration configuration)
    {
        var redirectBusinessIds = new List<string>();
        configuration.GetSection("RedirectBusinessIds").Bind(redirectBusinessIds);
        services.AddSingleton(new RedirectBusinessIds(redirectBusinessIds));

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IDocumentService>(p => new DocumentsService(p.GetService<Func<ContentfulConfig, IAssetRepository>>(), p.GetService<Func<ContentfulConfig, IGroupAdvisorRepository>>(), p.GetService<Func<ContentfulConfig, IGroupRepository>>(), p.GetService<IContentfulFactory<Asset, Document>>(), p.GetService<IContentfulConfigBuilder>(), p.GetService<ILoggedInHelper>()));

        return services;
    }

    public static IServiceCollection AddBuilders(this IServiceCollection services)
    {
        services.AddSingleton<IContentfulConfigBuilder>(p =>
            new ContentfulConfigBuilder(p.GetService<IConfiguration>()));

        return services;
    }

    public static IServiceCollection AddGroupConfiguration(this IServiceCollection services, IConfiguration configuration, Serilog.ILogger logger)
    {
        if (!string.IsNullOrEmpty(configuration["group:authenticationKey"]))
        {
            var groupKeys = new GroupAuthenticationKeys { Key = configuration["group:authenticationKey"] };
            services.AddSingleton(groupKeys);

            services.AddSingleton<IJwtDecoder>(p => new JwtDecoder(p.GetService<GroupAuthenticationKeys>(), p.GetService<ILogger<JwtDecoder>>()));
        }
        else
        {
            logger.Information("Group authenticationKey not found.");
        }

        return services;
    }

    public static IServiceCollection AddHelpers(this IServiceCollection services)
    {
        services.AddTransient<ILoggedInHelper>(p => new LoggedInHelper(p.GetService<IHttpContextAccessor>(), p.GetService<CurrentEnvironment>(), p.GetService<IJwtDecoder>(), p.GetService<ILogger<LoggedInHelper>>()));

        return services;
    }

    private static string GetHostEntryForUrl(string host, Serilog.ILogger logger)
    {
        var addresses = Dns.GetHostEntryAsync(host).Result.AddressList;

        if (!addresses.Any())
        {
            logger.Error($"Could not resolve IP address for redis instance : {host}");
            throw new Exception($"No redis instance could be found for host {host}");
        }

        if (addresses.Length > 1)
        {
            logger.Warning($"Multple IP address for redis instance : {host} attempting to use first");
        }

        return addresses.First().ToString();
    }
}
