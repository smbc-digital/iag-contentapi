using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Repositories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Services;
using StockportContentApi.Utils;

namespace StockportContentApi
{
    public class Startup
    {
        private readonly string _contentRootPath;

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).AddJsonFile("appsettings.json");
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            _contentRootPath = env.ContentRootPath;
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public virtual void ConfigureServices(IServiceCollection services)
        {
            Func<string, ContentfulConfig> createConfig = businessId =>
                new ContentfulConfig(businessId)
                    .Add("DELIVERY_URL", Configuration["Contentful:DeliveryUrl"])
                    .Add($"{ businessId.ToUpper()}_SPACE", 
                        Environment.GetEnvironmentVariable($"{ businessId.ToUpper()}_SPACE"))
                    .Add($"{ businessId.ToUpper()}_ACCESS_KEY", 
                        Environment.GetEnvironmentVariable($"{ businessId.ToUpper()}_ACCESS_KEY"))
                    .Build();

            var redirectBusinessIds = new List<string>();
            Configuration.GetSection("RedirectBusinessIds").Bind(redirectBusinessIds);

            services.AddSingleton(new RedirectBusinessIds(redirectBusinessIds));
            services.AddSingleton(new ButoConfig(Configuration["ButoBaseUrl"]));
            services.AddSingleton<IHttpClient>(
                p => new LoggingHttpClient(new HttpClient(new MsHttpClientWrapper(), p.GetService<ILogger<HttpClient>>()), p.GetService<ILogger<LoggingHttpClient>>()));
            services.AddTransient(_=>createConfig);
            services.AddTransient<IHealthcheckService>(p => new HealthcheckService($"{_contentRootPath}/version.txt", $"{_contentRootPath}/sha.txt", new FileWrapper()));
            services.AddTransient<ResponseHandler>();
            services.AddSingleton<ITimeProvider>(new TimeProvider());

            RegisterBuilders(services);
            RegisterRepos(services);

            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc();
        }

        private static void RegisterRepos(IServiceCollection services)
        {
            services.AddSingleton<Func<ContentfulConfig, ArticleRepository>>(
                p => { return x => new ArticleRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<Article>>(), p.GetService<IVideoRepository>(),p.GetService<ITimeProvider>()); });
            services.AddSingleton<Func<ContentfulConfig, ProfileRepository>>(
                p => { return x => new ProfileRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<Profile>>()); });
            services.AddSingleton<Func<ContentfulConfig, HomepageRepository>>(
                p => { return x => new HomepageRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<Homepage>>()); });
            services.AddSingleton<Func<ContentfulConfig, StartPageRepository>>(
                p => { return x => new StartPageRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<StartPage>>()); });
            services.AddSingleton<Func<ContentfulConfig, TopicRepository>>(
                p => { return x => new TopicRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<Topic>>()); });
            services.AddSingleton<Func<ContentfulConfig, FooterRepository>>(
                p => { return x => new FooterRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<Footer>>()); });
            services.AddSingleton<Func<ContentfulConfig, NewsRepository>>(
                p => { return x => new NewsRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<News>>(), p.GetService<IFactory<Newsroom>>(), p.GetService<ITimeProvider>(), p.GetService<IVideoRepository>()); });
            services.AddSingleton<Func<ContentfulConfig, AtoZRepository>>(
                p => { return x => new AtoZRepository(x, p.GetService<IHttpClient>(), p.GetService<IFactory<AtoZ>>()); });
            services.AddSingleton<IVideoRepository>(p => new VideoRepository(p.GetService<ButoConfig>(), p.GetService<IHttpClient>(), p.GetService<ILogger<VideoRepository>>()));
            services.AddSingleton<RedirectsRepository>();
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

            services.AddSingleton<IBuildContentTypesFromReferences<CarouselContent>, CarouselContentListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<SubItem>, SubItemListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Alert>, AlertListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Crumb>, BreadcrumbFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Topic>, TopicListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Section>, SectionListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Profile>, ProfileListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<Document>, DocumentListFactory>();
            services.AddSingleton<IBuildContentTypesFromReferences<SocialMediaLink>, SocialMediaLinkListFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddNLog();
            app.UseApplicationInsightsRequestTelemetry();
            app.UseApplicationInsightsExceptionTelemetry();
            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}