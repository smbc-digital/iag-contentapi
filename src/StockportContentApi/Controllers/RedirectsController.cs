using StockportContentApi.Repositories;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StockportContentApi.Controllers
{
    public class RedirectsController : Controller
    {     
        private readonly ResponseHandler _handler;
        private readonly RedirectsRepository _repository;

        public RedirectsController(ResponseHandler handler,
            RedirectsRepository repository)
        {
            _handler = handler;
            _repository = repository;
        }

        [HttpGet]
        [Route("api/redirects")]
        public async Task<IActionResult> GetRedirects(string businessId)
        {
            return await _handler.Get(() => _repository.GetRedirects());
        }
    }
}