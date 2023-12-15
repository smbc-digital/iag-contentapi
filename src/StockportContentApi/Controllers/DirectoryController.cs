namespace StockportContentApi.Controllers
{
    public class DirectoryController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, DirectoryRepository> _createDirectoryRepository;

        public DirectoryController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, DirectoryRepository> createDirectoryRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createDirectoryRepository = createDirectoryRepository;
        }

        [HttpGet]
        [Route("v2/{businessId}/directories")]
        public async Task<IActionResult> GetDirectories(string businessId)
        {
            return await _handler.Get(() =>
            {
                var directoryRepository = _createDirectoryRepository(_createConfig(businessId));
                return directoryRepository.GetAllDirectories();
            });
        }

        [HttpGet]
        [Route("{businessId}/directory/{slug}")]
        [Route("v2/{businessId}/directory/{slug}")]
        public async Task<IActionResult> GetDirectory(string slug, string businessId)
        {
            return await _handler.Get(() =>
            {
                var directoryRepository = _createDirectoryRepository(_createConfig(businessId));
                return directoryRepository.Get(slug);
            });
        }

        [HttpGet]
        [Route("v2/{businessId}/directory/{directorySlug}/directory-entry/{directoryEntrySlug}")]
        public async Task<IActionResult> GetDirectoryEntry(string directorySlug, string directoryEntrySlug, string businessId)
        {
            return await _handler.Get(() =>
            {
                var directoryRepository = _createDirectoryRepository(_createConfig(businessId));
                return directoryRepository.GetEntry(directoryEntrySlug);
            });
        }
    }
}
