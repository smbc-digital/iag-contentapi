﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using System.Linq;
using Contentful.Core;
using Contentful.Core.Models;
using Microsoft.Extensions.Configuration;
using StockportContentApi.Client;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public interface IGroupRepository
    {
        Task<HttpResponse> Get();
        Task<HttpResponse> GetAdministratorsGroups(string email);
        Task<ContentfulGroup> GetContentfulGroup(string slug);
        Task<ContentfulCollection<ContentfulGroupCategory>> GetContentfulGroupCategories();
        Task<HttpResponse> GetGroup(string slug, bool onlyActive);
        Task<List<GroupCategory>> GetGroupCategories();
        Task<HttpResponse> GetGroupResults(string category, double latitude, double longitude, string order, string location, string slugs, string volunteering, string subCategories, string organisation);
        Task<List<Group>> GetLinkedGroupsByOrganisation(string slug);
    }

    public class GroupRepository : BaseRepository, IGroupRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<ContentfulGroup, Group> _groupFactory;
        private readonly IContentfulFactory<List<ContentfulGroup>, List<Group>> _groupListFactory;
        private readonly IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>> _groupCategoryListFactory;
        private readonly IContentfulFactory<ContentfulGroupHomepage, GroupHomepage> _groupHomepageContentfulFactory;
        private readonly EventRepository _eventRepository;
        private readonly ICache _cache;
        private IConfiguration _configuration;
        private readonly int _groupsTimeout;

        public GroupRepository(ContentfulConfig config, IContentfulClientManager clientManager,
                                 ITimeProvider timeProvider,
                                 IContentfulFactory<ContentfulGroup, Group> groupFactory,
                                 IContentfulFactory<List<ContentfulGroup>, List<Group>> groupListFactory,
                                 IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>> groupCategoryListFactory,
                                 IContentfulFactory<ContentfulGroupHomepage, GroupHomepage> groupHomepageContentfulFactory,
                                 EventRepository eventRepository,
                                 ICache cache,
                                 IConfiguration configuration
            )
        {
            _dateComparer = new DateComparer(timeProvider);
            _client = clientManager.GetClient(config);
            _groupFactory = groupFactory;
            _groupListFactory = groupListFactory;
            _groupCategoryListFactory = groupCategoryListFactory;
            _eventRepository = eventRepository;
            _cache = cache;
            _configuration = configuration;
            int.TryParse(_configuration["redisExpiryTimes:Groups"], out _groupsTimeout);
            _groupHomepageContentfulFactory = groupHomepageContentfulFactory;
        }

        public async Task<HttpResponse> Get()
        {
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);
            var entries = await GetAllEntriesAsync(_client, builder);
            var contentfulGroups = entries as IEnumerable<ContentfulGroup> ?? entries.ToList();

            contentfulGroups =
                contentfulGroups.Where(
                    group => _dateComparer.DateNowIsNotBetweenHiddenRange(group.DateHiddenFrom, group.DateHiddenTo));

            var groupList = _groupListFactory.ToModel(contentfulGroups.ToList());

            return entries == null || !groupList.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Groups found")
                : HttpResponse.Successful(groupList);
        }

        public async Task<ContentfulGroup> GetContentfulGroup(string slug)
        {
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            return entry;
        }

        public async Task<HttpResponse> GetGroupHomepage()
        {
            var builder = new QueryBuilder<ContentfulGroupHomepage>().ContentTypeIs("groupHomepage").Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.ToList().First();
            
            return entry == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No event homepage found")
                : HttpResponse.Successful(_groupHomepageContentfulFactory.ToModel(entry));
        }

        public async Task<HttpResponse> GetGroup(string slug, bool onlyActive)
        {
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);

            var entries = await _client.GetEntriesAsync(builder);

            var entry = onlyActive 
                ? entries.FirstOrDefault(g => _dateComparer.DateNowIsNotBetweenHiddenRange(g.DateHiddenFrom, g.DateHiddenTo)) 
                : entries.FirstOrDefault();

            if (entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No group found for '{slug}'");

            var group = _groupFactory.ToModel(entry);
            group.SetEvents(await _eventRepository.GetLinkedEvents<Group>(slug));           

            if(group.CategoriesReference != null && group.CategoriesReference != null && group.CategoriesReference.Any() && group.SubCategories.Any())
            {
                group.SetLinkedGroups(await GetLinkedGroups(group.CategoriesReference, group.SubCategories));
            }            

            return HttpResponse.Successful(group);
        }

        public async Task<List<Group>> GetLinkedGroups(List<GroupCategory> categoriesReference, List<GroupSubCategory> subCategories)
        {
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);
            var entries = await _client.GetEntriesAsync(builder); 
            var groups = _groupListFactory.ToModel(entries.ToList());

            var featuredGroup = groups.Where(g => g.CategoriesReference.Any(c => string.IsNullOrEmpty(categoriesReference[0].Slug) || c.Slug.ToLower() == categoriesReference[0].Slug.ToLower())
                                       && g.SubCategories.Any(c => string.IsNullOrEmpty(subCategories[0].Slug) || c.Slug.ToLower() == subCategories[0].Slug.ToLower()));
          
            return featuredGroup.ToList();
        }

        public async Task<HttpResponse> GetGroupResults(string category, double latitude, double longitude, string order, string location, string slugs, string volunteering, string subCategories, string organisation)
        {
            var groupResults = new GroupResults();
            

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);

            if (longitude != 0 && latitude != 0) builder = builder.FieldEquals("fields.mapPosition[near]", latitude + "," + longitude + (location.ToLower() == Defaults.Groups.Location ? ",10" : ",3.2"));

            var subCategoriesArray = subCategories.Split(',');
            var subCategoriesList = subCategoriesArray.Where(c => !string.IsNullOrWhiteSpace(c));

            if (!string.IsNullOrEmpty(slugs))
            {
                var slugsList = slugs.Split(',');
                builder = builder.FieldIncludes("fields.slug", slugsList);
            }

            var entries = await GetAllEntriesAsync(_client, builder);

            if (entries == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "No groups found");

            var groups = _groupListFactory.ToModel(entries.ToList())
                .Where(g => g.CategoriesReference.Any(c => string.IsNullOrEmpty(category) || c.Slug.ToLower() == category.ToLower()))
                .Where(g => _dateComparer.DateNowIsNotBetweenHiddenRange(g.DateHiddenFrom, g.DateHiddenTo))
                .Where(g => volunteering == string.Empty || (g.Volunteering && volunteering == "yes"))
                .Where(g => organisation == string.Empty || (g.Organisation != null && g.Organisation.Slug == organisation))
                .Where(g => !subCategoriesList.Any() || g.SubCategories.Any(c => subCategoriesList.Contains(c.Slug)))
                .ToList();

            switch (!string.IsNullOrEmpty(order) ? order.ToLower() : "name a-z")
            {
                case "name a-z":
                    groups = groups.OrderBy(g => g.Name).ToList();
                    break;
                case "name z-a":
                    groups = groups.OrderByDescending(g => g.Name).ToList();
                    break;
                case "nearest":
                    break;
                default:
                    groups = groups.OrderBy(g => g.Name).ToList();
                    break;
            }

            groupResults.Groups = groups;

            groupResults.AvailableSubCategories = groups.SelectMany(g => g.SubCategories ?? new List<GroupSubCategory>()).ToList();

            var groupCategoryResults = await GetGroupCategories();

            if (!string.IsNullOrEmpty(category) && groupCategoryResults.All(g => !string.Equals(g.Slug, category, StringComparison.CurrentCultureIgnoreCase)))
                return HttpResponse.Failure(HttpStatusCode.NotFound, "No categories found");

            groupResults.Categories = groupCategoryResults;

            return HttpResponse.Successful(groupResults);
        }

        public async Task<HttpResponse> GetAdministratorsGroups(string email)
        {
            var builder =
                new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldExists("fields.groupAdministrators")
                    .Include(1)
                    .Limit(ContentfulQueryValues.LIMIT_MAX);

            var contentfulGroups = await _client.GetEntriesAsync(builder);

            var groups = contentfulGroups.Where(g => g.GroupAdministrators.Items.Any(i => i.Email.ToUpper() == email.ToUpper()));

            var result = _groupListFactory.ToModel(groups.ToList());

            return HttpResponse.Successful(result);
        }

        public async Task<List<GroupCategory>> GetGroupCategories()
        {
            return await _cache.GetFromCacheOrDirectlyAsync("group-categories", GetGroupCategoriesDirect, _groupsTimeout);
        }

        public async Task<ContentfulCollection<ContentfulGroupCategory>> GetContentfulGroupCategories()
        {
            return await _cache.GetFromCacheOrDirectlyAsync("contentful-group-categories", GetContentfulGroupCategoriesDirect, _groupsTimeout);
        }

        public async Task<List<Group>> GetLinkedGroupsByOrganisation(string slug)
        {
            var response = Get();

            var groups = response.Result.Get<List<Group>>();

            groups = groups.Where(g => g.Organisation.Slug == slug)
                .OrderBy(g => g.Name)
                .ToList();

            return groups;
        }

        private async Task<List<GroupCategory>> GetGroupCategoriesDirect()
        {
            var groupCategoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);
            var groupCategoryEntries = await _client.GetEntriesAsync(groupCategoryBuilder);

            var groupCategoryList = _groupCategoryListFactory.ToModel(groupCategoryEntries.ToList())
                .OrderBy(c => c.Name).ToList();

            return !groupCategoryList.Any() ? null : groupCategoryList;
        }

        private async Task<ContentfulCollection<ContentfulGroupCategory>> GetContentfulGroupCategoriesDirect()
        {
            var groupCategoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);
            var result = await _client.GetEntriesAsync(groupCategoryBuilder);

            return !result.Any() ? null : result;
        }
    }
}
