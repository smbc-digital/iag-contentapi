using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Repositories;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StockportContentApi.Controllers
{
    public class PrivacyNoticeController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<ContentfulConfig, IPrivacyNoticeRepository> _privacyNoticeRepository;
        private readonly Func<string, ContentfulConfig> _createConfig;

        public PrivacyNoticeController(ResponseHandler handler, Func<ContentfulConfig, IPrivacyNoticeRepository> privacyNoticeRepository, Func<string, ContentfulConfig> createConfig)
        {
            _handler = handler;
            _createConfig = createConfig;
            _privacyNoticeRepository = privacyNoticeRepository;
        }

        [HttpGet]
        [Route("{businessId}/privacy-notices/{slug}")]
        [Route("v1/{businessId}/privacy-notices/{slug}")]
        public async Task<IActionResult> GetPrivacyNotice(string slug, string businessId)
        {
            return await _handler.Get(async () =>
            {
                var repository = _privacyNoticeRepository(_createConfig(businessId));
                var privacyNotice = await repository.GetPrivacyNotice(slug);

                if (privacyNotice is null)
                    return HttpResponse.Failure(System.Net.HttpStatusCode.NotFound, "Privacy notice not found");

                return HttpResponse.Successful(privacyNotice);
            });
        }

        [HttpGet]
        [Route("{businessId}/privacy-notices")]
        [Route("v1/{businessId}/privacy-notices")]
        public async Task<IActionResult> GetAllPrivacyNotices([FromRoute]string businessId, [FromQuery]string title)
        {
            return await _handler.Get(async () =>
            {
                List<PrivacyNotice> privacyNotices;
                var repository = _privacyNoticeRepository(_createConfig(businessId));

                if (!string.IsNullOrEmpty(title))
                {
                    privacyNotices = await repository.GetPrivacyNoticesByTitle(title);
                }
                else
                {
                    privacyNotices = await repository.GetAllPrivacyNotices();
                }

                if (privacyNotices is null)
                {
                    return HttpResponse.Failure(System.Net.HttpStatusCode.NotFound, "Privacy notices not found");
                }
                else
                {
                    return HttpResponse.Successful(privacyNotices);
                }
            });
        }
    }
}
