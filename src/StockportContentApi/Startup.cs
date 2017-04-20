using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StockportContentApi.Client;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Repositories;
using StockportContentApi.Model;
using StockportContentApi.Services;
using StockportContentApi.Utils;
using Swashbuckle.Swagger.Model;
using Contentful.Core.Models;

namespace StockportContentApi
{
    public class Startup
    {
        private readonly string _contentRootPath;
        private const string ConfigDir = "app-config";

        public Startup(IHostingEnvironment env)
        {
            var appConfig = Path.Combine(ConfigDir, "appsettings.json");
            var envConfig = Path.Combine(ConfigDir, $"appsettings.{env.EnvironmentName}.json");
            var secretConfig = Path.Combine(ConfigDir, "injected",
                $"appsettings.{env.EnvironmentName}.secrets.json");

            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile(appConfig)
                .AddJsonFile(envConfig)
                .AddJsonFile(secretConfig, true)
                .AddEnvironmentVariables()
                .Build();

            _contentRootPath = env.ContentRootPath;
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public virtual void ConfigureServices(IServiceCollection services)
        {
            Func<string, ContentfulConfig> createConfig = businessId =>
                    new ContentfulConfig(businessId)
                        .Add("DELIVERY_URL", Configuration["Contentful:DeliveryUrl"])
                        .Add($"{businessId.ToUpper()}_SPACE", Configuration[$"{businessId}:Space"])
                        .Add($"{businessId.ToUpper()}_ACCESS_KEY", Configuration[$"{businessId}:AccessKey"])
                        .Build();

            var redirectBusinessIds = new List<string>();
            Configuration.GetSection("RedirectBusinessIds").Bind(redirectBusinessIds);

            services.AddSingleton(new RedirectBusinessIds(redirectBusinessIds));
            services.AddSingleton(new ButoConfig(Configuration["ButoBaseUrl"]));
            services.AddSingleton<IHttpClient>(
                p => new LoggingHttpClient(new HttpClient(new MsHttpClientWrapper(), p.GetService<ILogger<HttpClient>>()), p.GetService<ILogger<LoggingHttpClient>>()));
            services.AddTransient(_ => createConfig);
            services.AddTransient<IHealthcheckService>(p => new HealthcheckService($"{_contentRootPath}/version.txt", $"{_contentRootPath}/sha.txt", new FileWrapper()));
            services.AddTransient<ResponseHandler>();
            services.AddSingleton<ITimeProvider>(new TimeProvider());

            RegisterBuilders(services);
            RegisterRepos(services);

            services.AddSwaggerGen(c =>
            {
                c.SingleApiVersion(new Info { Title = "Stockport Content API", Version = "v1" });
            });

            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();
        }

        private static void RegisterRepos(IServiceCollection services)
        {
            services.AddSingleton<IVideoRepository>(p => new VideoRepository(p.GetService<ButoConfig>(), p.GetService<IHttpClient>(), p.GetService<ILogger<VideoRepository>>()));
            services.AddSingleton<IContentfulFactory<Asset, Document>>(new DocumentContentfulFactory());
            services.AddSingleton<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>(p => new CrumbContentfulFactory());

            services.AddSingleton<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>(p => new SubItemContentfulFactory(p.GetService<ITimeProvider>()));

            services.AddSingleton<IContentfulFactory<List<ContentfulGroup>, List<Group>>>(p => new GroupListContentfulFactory(p.GetService<IContentfulFactory<ContentfulGroup, Group>>()));
            services.AddSingleton<IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>>>(p => new GroupCategoryListContentfulFactory(p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>()));

            services.AddSingleton<IContentfulFactory<Entry<ContentfulAlert>, Alert>>(p => new AlertContentfulFactory());
            services.AddSingleton<IContentfulFactory<ContentfulSection, Section>>(p => new SectionContentfulFactory(p.GetService<IContentfulFactory<ContentfulProfile, Profile>>(),
                                                                                                                    p.GetService<IContentfulFactory<Asset, Document>>(),
                                                                                                                    p.GetService<IVideoRepository>(),
                                                                                                                    p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<ContentfulEvent, Event>>(p => new EventContentfulFactory(p.GetService<IContentfulFactory<Asset, Document>>(),
                                                                                                                    p.GetService<IContentfulFactory<ContentfulGroup, Group>>(),
                                                                                                                    p.GetService<IContentfulFactory<Entry<ContentfulAlert>, Alert>>(),
                                                                                                                    p.GetService<ITimeProvider>()));
            services.AddSingleton<IContentfulFactory<ContentfulProfile, Profile>>(p => new ProfileContentfulFactory(p.GetService<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>()));
            services.AddSingleton<IContentfulFactory<ContentfulGroup, Group>>(p => new GroupContentfulFactory(p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>()));
            services.AddSingleton<IContentfulFactory<ContentfulPayment, Payment>>
                (p => new PaymentContentfulFactory(p.GetService<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>()));

            services.AddSingleton<IContentfulFactory<ContentfulTopic, Topic>>(p => new TopicContentfulFactory(p.GetService<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>(),
                                                                                                              p.GetService<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>(),
                                                                                                              p.GetService<IContentfulFactory<Entry<ContentfulAlert>, Alert>>(),
                                                                                                              p.GetService<ITimeProvider>()));

            services.AddSingleton<IContentfulFactory<ContentfulShowcase, Showcase>>
                (p => new ShowcaseContentfulFactory(p.GetService<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>(), p.GetService<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>(), p.GetService<ITimeProvider>()));

            

            services.AddSingleton<IContentfulFactory<ContentfulNews, News>>(p => new NewsContentfulFactory(p.GetService<IVideoRepository>(),
                                                                                                           p.GetService<IContentfulFactory<Asset, Document>>()));

            services.AddSingleton<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(p => new GroupCategoryContentfulFactory());

            services.AddSingleton<IContentfulFactory<Entry<ContentfulArticle>, Article>>
                (p => new ArticleContentfulFactory(p.GetService<IContentfulFactory<ContentfulSection, Section>>(),
                                                    p.GetService<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>(),
                                                    p.GetService<IContentfulFactory<ContentfulProfile, Profile>>(),
                                                    p.GetService<IContentfulFactory<Entry<ContentfulArticle>, Topic>>(),
                                                    p.GetService<IContentfulFactory<Asset, Document>>(),
                                                    p.GetService<IVideoRepository>(),
                                                    p.GetService<ITimeProvider>()));

            services.AddSingleton<IContentfulFactory<Entry<ContentfulArticle>, Topic>>(
                p => new ParentTopicContentfulFactory(
                    p.GetService<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>()
                    ,p.GetService<ITimeProvider>())
                    );

            services.AddSingleton<IContentfulClientManager>(new ContentfulClientManager(new System.Net.Http.HttpClient()));
            services.AddSingleton<Func<ContentfulConfig, ArticleRepository>>(
                p => { return x => new ArticleRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<Entry<ContentfulArticle>, Article>>(), p.GetService<IVideoRepository>()); });

            services.AddSingleton<Func<ContentfulConfig, ShowcaseRepository>>(
               p => { return x => new ShowcaseRepository(x, p.GetService<IContentfulFactory<ContentfulShowcase, Showcase>>(), p.GetService<IContentfulClientManager>()); });

            services.AddSingleton<Func<ContentfulConfig, ProfileRepository>>(
                p => { return x => new ProfileRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulProfile, Profile>>()); });
            services.AddSingleton<Func<ContentfulConfig, GroupRepository>>(
                p => { return x => new GroupRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulGroup, Group>>(), p.GetService<IContentfulFactory<List<ContentfulGroup>, List<Group>>>(), p.GetService<IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>>>()); });

            services.AddSingleton<Func<ContentfulConfig, PaymentRepository>>(
                p => { return x => new PaymentRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulPayment, Payment>>()); });

            services.AddSingleton<Func<ContentfulConfig, GroupCategoryRepository>>(
                p => { return x => new GroupCategoryRepository(x, p.GetService<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>(), p.GetService<IContentfulClientManager>()); });

            services.AddSingleton<Func<ContentfulConfig, HomepageRepository>>(
                p => { return x => new HomepageRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<Homepage>>()); });
            services.AddSingleton<Func<ContentfulConfig, StartPageRepository>>(
                p => { return x => new StartPageRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<StartPage>>()); });
            services.AddSingleton<Func<ContentfulConfig, TopicRepository>>(
                p => { return x => new TopicRepository(x, p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulTopic, Topic>>()); });
            services.AddSingleton<Func<ContentfulConfig, FooterRepository>>(
                p => { return x => new FooterRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<Footer>>()); });
            services.AddSingleton<Func<ContentfulConfig, NewsRepository>>(
                p => { return x => new NewsRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<News>>(), p.GetService<IFactory<Newsroom>>(), p.GetService<INewsCategoriesFactory>(), p.GetService<ITimeProvider>(), p.GetService<IContentfulClientManager>(), p.GetService<IContentfulFactory<ContentfulNews, News>>()); });
            services.AddSingleton<Func<ContentfulConfig, AtoZRepository>>(
                p => { return x => new AtoZRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<AtoZ>>(), p.GetService<ITimeProvider>()); });
            services.AddSingleton<RedirectsRepository>();
            services.AddSingleton<Func<ContentfulConfig, EventRepository>>(
                p => { return x => new EventRepository(x, p.GetService<IHttpClient>(), p.GetService<IContentfulClientManager>(), p.GetService<ITimeProvider>(), p.GetService<IContentfulFactory<ContentfulEvent, Event>>(), p.GetService<IEventCategoriesFactory>()); });
        }

        private static void RegisterBuilders(IServiceCollection services)
        {
            services.AddSingleton<IFactory<Article>, ArticleFactory>();
            services.AddSingleton<IFactory<Alert>, AlertFactory>();
            services.AddSingleton<IFactory<CarouselContent>, CarouselContentFactory>();
            services.AddSingleton<IFactory<Homepage>, HomepageFactory>();
            services.AddSingleton<IFactory<Topic>, TopicFactory>();
            services.AddSingleton<IFactory<Profile>, ProfileFactory>();
            services.AddSingleton<IFactory<News>, NewsFactory>();
            services.AddSingleton<IFactory<Newsroom>, NewsroomFactory>();
            services.AddSingleton<IFactory<AtoZ>, AtoZFactory>();
            services.AddSingleton<IFactory<StartPage>, StartPageFactory>();
            services.AddSingleton<IFactory<SubItem>, SubItemFactory>();
            services.AddSingleton<IFactory<Footer>, FooterFactory>();
            services.AddSingleton<IFactory<SocialMediaLink>, SocialMediaLinkFactory>();
            services.AddSingleton<IFactory<BusinessIdToRedirects>, RedirectsFactory>();
            services.AddSingleton<IFactory<LiveChat>, LiveChatFactory>();
            services.AddSingleton<INewsCategoriesFactory, NewsCategoriesFactory>();
            services.AddSingleton<IEventCategoriesFactory, EventCategoriesFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<CarouselContent>, CarouselContentListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<SubItem>, SubItemListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Alert>, AlertListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Crumb>, BreadcrumbFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Topic>, TopicListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Section>, SectionListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Profile>, ProfileListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Document>, DocumentListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<SocialMediaLink>, SocialMediaLinkListFactory>();
            services.AddSingleton<IBuildContentTypeFromReference<LiveChat>, LiveChatListFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSwagger();
            app.UseSwaggerUi();

            loggerFactory.AddNLog();
            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();
            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}