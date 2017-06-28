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

namespace StockportContentApi.Controllers
{
    public class GroupController
    {

        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, GroupRepository> _groupRepository;
        private readonly Func<ContentfulConfig, GroupCategoryRepository> _groupCategoryRepository;
        private readonly Func<ContentfulConfig, ManagementRepository> _managementRepository;
        private readonly IMapper _mapper;

        public GroupController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, GroupRepository> groupRepository,
            Func<ContentfulConfig, GroupCategoryRepository> groupCategoryRepository,
            Func<ContentfulConfig, ManagementRepository> managementRepository,
            IMapper mapper
            )
        {
            _handler = handler;
            _createConfig = createConfig;
            _groupRepository = groupRepository;
            _groupCategoryRepository = groupCategoryRepository;
            _managementRepository = managementRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("api/{businessId}/group/{groupSlug}")]
        public async Task<IActionResult> GetGroup(string groupSlug, string businessId, [FromQuery] bool onlyActive = true)
        { 
            return await _handler.Get(() =>
            {
                var groupRepository = _groupRepository(_createConfig(businessId));
                return groupRepository.GetGroup(groupSlug, onlyActive);
            });
        }

        [HttpGet]
        [Route("api/{businessId}/groupCategory")]
        public async Task<IActionResult> GetGroupCategories(string businessId)
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _groupCategoryRepository(_createConfig(businessId));
                return groupRepository.GetGroupCategories();
            });
        }

        [HttpGet]
        [Route("api/{businessId}/groupResults")]
        public async Task<IActionResult> GetGroupResults(string businessId, [FromQuery] string category = "", [FromQuery] double latitude = 0, [FromQuery] double longitude = 0, [FromQuery] string order = "", [FromQuery] string location = "")
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _groupRepository(_createConfig(businessId));
                return groupRepository.GetGroupResults(category, latitude, longitude, order, location);
            });
        }

        [HttpGet]
        [Route("api/{businessId}/groups/administrators/{email}")]
        public async Task<IActionResult> GetAdministratorsGroups(string businessId, string email)
        {
            return await _handler.Get(() =>
            {
                var groupRepository = _groupRepository(_createConfig(businessId));
                return groupRepository.GetAdministratorsGroups(email);
            });
        }
        [HttpPost]
        [Route("api/{businessId}/groups/add")]
        public async Task<IActionResult> AddGroup(string businessId, [FromBody] Group group)
        {
            var contentfulGroup = _mapper.Map<ContentfulGroup>(group);

            return await _handler.Get(() =>
            {
                var managementRepository = _managementRepository(_createConfig(businessId));
                return managementRepository.CreateOrUpdate(contentfulGroup);
            });
        }

        [HttpPut]
        [Route("api/{businessId}/groups/update")]
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

        [HttpDelete]
        [Route("api/{businessId}/groups/{slug}/administrators/delete/{emailAddress}")]
        public async Task<IActionResult> RemoveAdministrator(string slug, string emailAddress, string businessId)
        {
            var repository = _groupRepository(_createConfig(businessId));

            var existingGroup = await repository.GetContentfulGroup(slug);

            existingGroup.GroupAdministrators.Items = existingGroup.GroupAdministrators.Items.Where(a => a.Email != emailAddress).ToList();

            ManagementGroup managementGroup = new ManagementGroup();
            _mapper.Map(existingGroup, managementGroup);

            return await _handler.Get(async () =>
            {
                var managementRepository = _managementRepository(_createConfig(businessId));
                var version = await managementRepository.GetVersion(existingGroup.Sys.Id);
                existingGroup.Sys.Version = version;
                return await managementRepository.CreateOrUpdate(managementGroup, existingGroup.Sys);
            });
        }

        [HttpPut]
        [Route("api/{businessId}/groups/{slug}/administrators/update/{emailAddress}")]
        public async Task<IActionResult> UpdateAdministrator([FromBody] string permission, string slug, string emailAddress, string businessId)
        {
            return await AddOrUpdateAdministrator(permission, slug, emailAddress, businessId);
        }

        [HttpPost]
        [Route("api/{businessId}/groups/{slug}/administrators/add/{emailAddress}")]
        public async Task<IActionResult> AddAdministrator([FromBody] string permission, string slug, string emailAddress, string businessId)
        {
            return await AddOrUpdateAdministrator(permission, slug, emailAddress, businessId);
        }

        private async Task<IActionResult> AddOrUpdateAdministrator(string permission, string slug, string emailAddress, string businessId)
        {
            var repository = _groupRepository(_createConfig(businessId));

            var existingGroup = await repository.GetContentfulGroup(slug);

            existingGroup.GroupAdministrators.Items = existingGroup.GroupAdministrators.Items.Where(a => a.Email != emailAddress).ToList();
            existingGroup.GroupAdministrators.Items.Add(new GroupAdministratorItems { Email = emailAddress, Permission = permission });

            ManagementGroup managementGroup = new ManagementGroup();
            _mapper.Map(existingGroup, managementGroup);

            return await _handler.Get(async () =>
            {
                var managementRepository = _managementRepository(_createConfig(businessId));
                var version = await managementRepository.GetVersion(existingGroup.Sys.Id);
                existingGroup.Sys.Version = version;
                return await managementRepository.CreateOrUpdate(managementGroup, existingGroup.Sys);
            });
        }

        private ManagementGroup ConvertToManagementGroup(Group group, List<ContentfulGroupCategory> referencedCategories, ContentfulGroup existingGroup)
        {
            var contentfulGroup = _mapper.Map<ContentfulGroup>(group);
            contentfulGroup.CategoriesReference = referencedCategories;
            contentfulGroup.Image = existingGroup.Image;
            var managementGroup = new ManagementGroup();
            _mapper.Map(contentfulGroup, managementGroup);
            return managementGroup;
        }
    }
}
