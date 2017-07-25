﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;
using StockportContentApi.Model;
using System.Collections.Generic;
using StockportContentApi.Http;

namespace StockportContentApi.Controllers
{
    public class EventController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, EventRepository> _eventRepository;
        private readonly Func<ContentfulConfig, EventCategoryRepository> _eventCategoryRepository;
        private readonly Func<ContentfulConfig, ManagementRepository> _managementRepository;

        public EventController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, EventRepository> eventRepository,
            Func<ContentfulConfig, EventCategoryRepository> eventCategoryRepository,
            Func<ContentfulConfig, ManagementRepository> managementRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _eventRepository = eventRepository;
            _managementRepository = managementRepository;
            _eventCategoryRepository = eventCategoryRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/eventCategory")]
        public async Task<IActionResult> GetEventCategories(string businessId)
        {
            return await _handler.Get(() =>
            {
                var eventRepository = _eventCategoryRepository(_createConfig(businessId));
                return eventRepository.GetEventCategories();
            });
        }

        [HttpGet]
        [Route("/api/{businessId}/eventhomepage")]
        public async Task<IActionResult> Homepage(string businessId)
        {
            var categoryRepository = _eventCategoryRepository(_createConfig(businessId));
            var categoriesresponse = await categoryRepository.GetEventCategories();
            var categories = categoriesresponse.Get<List<EventCategory>>();

            var repository = _eventRepository(_createConfig(businessId));
            var response = await repository.GetEventHomepage();
            var homepage = response.Get<EventHomepage>();

            homepage.Categories = categories;
            return Ok(homepage);
        }

        [HttpGet]
        [Route("/api/{businessId}/events/{slug}")]
        public async Task<IActionResult> Detail(string slug, string businessId, [FromQuery] DateTime? date)
        {
            return await _handler.Get(() =>
            {
                var repository = _eventRepository(_createConfig(businessId));
                return repository.GetEvent(slug, date);
            });
        }

        [HttpGet]
        [Route("/api/{businessId}/events")]
        [Route("/api/{businessId}/events/latest/{limit}")]
        public async Task<IActionResult> Index(
            string businessId, 
            int limit = 0,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] string category = null,
            [FromQuery] bool? featured = null,
            [FromQuery] string tag = null)
        {
            return await _handler.Get(() =>
            {
                var repository = _eventRepository(_createConfig(businessId));
                return repository.Get(dateFrom, dateTo, category, limit, featured, tag);
            });
        }

        [HttpDelete]
        [Route("api/{businessId}/events/{slug}")]
        public async Task<IActionResult> DeleteEvent(string slug, string businessId)
        {
            var repository = _eventRepository(_createConfig(businessId));
            var existingEvent = await repository.GetContentfulEvent(slug);

            return await _handler.Get(async () =>
            {
                var managementRepository = _managementRepository(_createConfig(businessId));
                var version = await managementRepository.GetVersion(existingEvent.Sys.Id);
                existingEvent.Sys.Version = version;
                return await managementRepository.Delete(existingEvent.Sys);
            });
        }
    }
}