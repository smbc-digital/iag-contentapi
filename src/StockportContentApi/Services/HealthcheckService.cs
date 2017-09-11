using System.Collections.Generic;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Services
{
    public interface IHealthcheckService
    {
        Task<Healthcheck> Get();
    }

    public class HealthcheckService : IHealthcheckService
    {
        private readonly string _appVersion;
        private readonly string _sha;
        private readonly IFileWrapper _fileWrapper;
        private readonly string _environment;
        private readonly ICache _cacheWrapper;

        public HealthcheckService(string appVersionPath, string shaPath, IFileWrapper fileWrapper, string environment, ICache cacheWrapper)
        {
            _fileWrapper = fileWrapper;
            _appVersion = GetFirstFileLineOrDefault(appVersionPath, "dev");
            _sha = GetFirstFileLineOrDefault(shaPath, string.Empty);
            _environment = environment;
            _cacheWrapper = cacheWrapper;
        }

        private string GetFirstFileLineOrDefault(string filePath, string defaultValue)
        {
            if (_fileWrapper.Exists(filePath))
            {
                var firstLine = _fileWrapper.ReadAllLines(filePath).FirstOrDefault();
                if (!string.IsNullOrEmpty(firstLine))
                    return firstLine;
            }
            return defaultValue;
        }

        public async Task<Healthcheck> Get()
        {
            // Commented out because it was breaking prod.
            //var keys = await _cacheWrapper.GetKeys();
            return new Healthcheck(_appVersion, _sha, _environment, new List<RedisValueData>());
        }
    }
}