using StockportContentApi.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;

namespace StockportContentApi.Controllers
{
    public class PaymentController
    {
        
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, PaymentRepository> _createRepository;

        public PaymentController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, PaymentRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/payment/{slug}")]
        public async Task<IActionResult> GetPayment(string slug, string  businessId)
        {
            return await _handler.Get(() =>
            {
                var paymentRepository = _createRepository(_createConfig(businessId));
                return paymentRepository.GetPayment(slug);
            });
        }

        [HttpGet]
        [Route("api/{businessId}/payment")]
        public async Task<IActionResult> Index(string businessId)
        {
            return await _handler.Get(() =>
            {
                var paymentRepository = _createRepository(_createConfig(businessId));
                return paymentRepository.Get();
            });
        }
    }
}