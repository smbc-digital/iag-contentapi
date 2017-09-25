using StockportContentApi.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using AutoMapper;
using StockportContentApi.ManagementModels;
using StockportContentApi.Http;

namespace StockportContentApi.Controllers
{
    public class GroupController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, GroupRepository> _groupRepository;
        private readonly Func<ContentfulConfig, EventRepository> _eventRepository;
        private readonly Func<ContentfulConfig, GroupCategoryRepository> _groupCategoryRepository;
        private readonly Func<ContentfulConfig, ManagementRepository> _managementRepository;        
        private readonly IMapper _mapper;

        public GroupController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, GroupRepository> groupRepository,
            Func<ContentfulConfig, EventRepository> eventRepository,
            Func<ContentfulConfig, GroupCategoryRepository> groupCategoryRepository,
            Func<ContentfulConfig, ManagementRepository> managementRepository,
            IMapper mapper
            )
        {
            _handler = handler;
            _createConfig = createConfig;
            _groupRepository = groupRepository;
            _eventRepository = eventRepository;
            _groupCategoryRepository = groupCategoryRepository;
            _managementRepository = managementRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("api/{businessId}/groups")]
        [Route("api/v1/{businessId}/groups")]
        public async Task<IActionResult> GetGroups(string businessId)
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _groupRepository(_createConfig(businessId));
                return groupRepository.Get();
            });
        }

        [HttpGet]
        [Route("/api/{businessId}/grouphomepage")]
        [Route("/api/v1/{businessId}/grouphomepage")]
        public async Task<IActionResult> Homepage(string businessId)
        {         
            var repository = _groupRepository(_createConfig(businessId));
            var response = await repository.GetGroupHomepage();
            var homepage = response.Get<GroupHomepage>();          
            return Ok(homepage);
        }

        [HttpGet]
        [Route("api/{businessId}/groups/{groupSlug}")]
        [Route("api/v1/{businessId}/groups/{groupSlug}")]
        public async Task<IActionResult> GetGroup(string groupSlug, string businessId, [FromQuery] bool onlyActive = true)
        { 
            return await _handler.Get(() =>
            {
                var groupRepository = _groupRepository(_createConfig(businessId));
                return groupRepository.GetGroup(groupSlug, onlyActive);
            });
        }

        [HttpGet]
        [Route("api/{businessId}/group-categories")]
        [Route("api/v1/{businessId}/group-categories")]
        public async Task<IActionResult> GetGroupCategories(string businessId)
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _groupCategoryRepository(_createConfig(businessId));
                return groupRepository.GetGroupCategories();
            });
        }

        [HttpGet]
        [Route("api/{businessId}/group-results")]
        [Route("api/v1/{businessId}/group-results")]
        public async Task<IActionResult> GetGroupResults(string businessId, GroupSearch groupSearch, [FromQuery] string slugs = "")
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _groupRepository(_createConfig(businessId));
                return groupRepository.GetGroupResults(groupSearch.Category, groupSearch.Latitude, groupSearch.Longitude, groupSearch.Order, groupSearch.Location, slugs, groupSearch.GetInvolved, groupSearch.SubCategories, groupSearch.Organisation);
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet]
        [Route("api/{businessId}/groups/administrators/{email}")]
        [Route("api/v1/{businessId}/groups/administrators/{email}")]
        public async Task<IActionResult> GetAdministratorsGroups(string businessId, string email)
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _groupRepository(_createConfig(businessId));
                return groupRepository.GetAdministratorsGroups(email);
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        [Route("api/{businessId}/groups/{slug}")]
        [Route("api/v1/{businessId}/groups/{slug}")]
        public async Task<IActionResult> UpdateGroup([FromBody] Group group, string businessId)
        {
            var repository = _groupRepository(_createConfig(businessId));
            var existingGroup = await repository.GetContentfulGroup(group.Slug);

            var existingCategories = await repository.GetContentfulGroupCategories();
            var referencedCategories = existingCategories.Where(c => group.CategoriesReference.Select(cr => cr.Slug).Contains(c.Slug)).ToList();

            var managementGroup = ConvertToManagementGroup(group, referencedCategories, existingGroup);

            return await _handler.Get(async () =>
            {
                var managementRepository = _managementRepository(_createConfig(businessId));
                var version = await managementRepository.GetVersion(existingGroup.Sys.Id);
                existingGroup.Sys.Version = version;
                return await managementRepository.CreateOrUpdate(managementGroup, existingGroup.Sys);
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete]
        [Route("api/{businessId}/groups/{slug}")]
        [Route("api/v1/{businessId}/groups/{slug}")]
        public async Task<IActionResult> DeleteGroup(string slug, string businessId)
        {
            var repository = _groupRepository(_createConfig(businessId));
            var eventRepository = _eventRepository(_createConfig(businessId));
            var existingGroup = await repository.GetContentfulGroup(slug);
            var groupEvents = await eventRepository.GetAllEventsForAGroup(slug);

            return await _handler.Get(async () =>
            {
                var managementRepository = _managementRepository(_createConfig(businessId));

                foreach (var groupEvent in groupEvents)
                {
                    var eventVersion = await managementRepository.GetVersion(groupEvent.Sys.Id);
                    groupEvent.Sys.Version = eventVersion;
                    var result = await managementRepository.Delete(groupEvent.Sys);
                    if (result.StatusCode != System.Net.HttpStatusCode.OK) {
                        return result;
                    }
                }

                var version = await managementRepository.GetVersion(existingGroup.Sys.Id);
                existingGroup.Sys.Version = version;
                return await managementRepository.Delete(existingGroup.Sys);
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete]
        [Route("api/{businessId}/groups/{slug}/administrators/{emailAddress}")]
        [Route("api/v1/{businessId}/groups/{slug}/administrators/{emailAddress}")]
        public async Task<IActionResult> RemoveAdministrator(string slug, string emailAddress, string businessId)
        {
            var repository = _groupRepository(_createConfig(businessId));

            var existingGroup = await repository.GetContentfulGroup(slug);

            existingGroup.GroupAdministrators.Items = existingGroup.GroupAdministrators.Items.Where(a => a.Email != emailAddress).ToList();

            var managementGroup = new ManagementGroup();
            _mapper.Map(existingGroup, managementGroup);

            return await _handler.Get(async () =>
            {
                var managementRepository = _managementRepository(_createConfig(businessId));
                var version = await managementRepository.GetVersion(existingGroup.Sys.Id);
                existingGroup.Sys.Version = version;
                var response = await managementRepository.CreateOrUpdate(managementGroup, existingGroup.Sys);
                
                return response.StatusCode == System.Net.HttpStatusCode.OK ? HttpResponse.Successful($"Successfully deleted administrator") : HttpResponse.Failure(response.StatusCode, "An error has occurred");
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        [Route("api/{businessId}/groups/{slug}/administrators/{emailAddress}")]
        [Route("api/v1/{businessId}/groups/{slug}/administrators/{emailAddress}")]
        public async Task<IActionResult> UpdateAdministrator([FromBody] GroupAdministratorItems user, string slug, string emailAddress, string businessId)
        {
            return await AddOrUpdateAdministrator(user, slug, emailAddress, businessId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("api/{businessId}/groups/{slug}/administrators/{emailAddress}")]
        [Route("api/v1/{businessId}/groups/{slug}/administrators/{emailAddress}")]
        public async Task<IActionResult> AddAdministrator([FromBody] GroupAdministratorItems user, string slug, string emailAddress, string businessId)
        {
            return await AddOrUpdateAdministrator(user, slug, emailAddress, businessId);
        }

        private async Task<IActionResult> AddOrUpdateAdministrator(GroupAdministratorItems user, string slug, string emailAddress, string businessId)
        {
            var repository = _groupRepository(_createConfig(businessId));

            var existingGroup = await repository.GetContentfulGroup(slug);

            existingGroup.GroupAdministrators.Items = existingGroup.GroupAdministrators.Items.Where(a => a.Email != emailAddress).ToList();

            if (user != null)
            {
                existingGroup.GroupAdministrators.Items.Add(user);

                ManagementGroup managementGroup = new ManagementGroup();
                _mapper.Map(existingGroup, managementGroup);

                return await _handler.Get(async () =>
                {
                    var managementRepository = _managementRepository(_createConfig(businessId));
                    var version = await managementRepository.GetVersion(existingGroup.Sys.Id);
                    existingGroup.Sys.Version = version;
                    var response = await managementRepository.CreateOrUpdate(managementGroup, existingGroup.Sys);
                    return response.StatusCode == System.Net.HttpStatusCode.OK ? HttpResponse.Successful($"Successfully updated the group {existingGroup.Name}") : HttpResponse.Failure(response.StatusCode, "An error has occurred");
                });
            }
            else
            {
                throw new Exception("Invalid data received");
            }
        }

        private ManagementGroup ConvertToManagementGroup(Group group, List<ContentfulGroupCategory> referencedCategories, ContentfulGroup existingGroup)
        {
            var contentfulGroup = _mapper.Map<ContentfulGroup>(group);
            contentfulGroup.CategoriesReference = referencedCategories;
            contentfulGroup.Image = existingGroup.Image;
            contentfulGroup.SubCategories = existingGroup.SubCategories;
            contentfulGroup.Organisation = existingGroup.Organisation;
            contentfulGroup.Donations = existingGroup.Donations;
            var managementGroup = new ManagementGroup();
            _mapper.Map(contentfulGroup, managementGroup);
            return managementGroup;
        }
    }
}
