﻿namespace StockportContentApi.Client;

public interface IContentfulClientManager
{
    IContentfulClient GetClient(ContentfulConfig config);
    IContentfulManagementClient GetManagementClient(ContentfulConfig config);
}

public class ContentfulClientManager : IContentfulClientManager
{
    private readonly System.Net.Http.HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public ContentfulClientManager(
        System.Net.Http.HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public IContentfulClient GetClient(ContentfulConfig config)
    {
        bool.TryParse(_configuration["Contentful:UsePreviewAPI"], out var usePreviewApi);
        var client = new ContentfulClient(_httpClient, !usePreviewApi ? config.AccessKey : "", usePreviewApi ? config.AccessKey : "", config.SpaceKey, usePreviewApi)
        {
            ResolveEntriesSelectively = true
        };

        return client;
    }

    public IContentfulManagementClient GetManagementClient(ContentfulConfig config)
        => new ContentfulManagementClient(_httpClient, config.ManagementKey, config.SpaceKey);
}