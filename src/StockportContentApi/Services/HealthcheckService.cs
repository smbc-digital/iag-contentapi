namespace StockportContentApi.Services;

public interface IHealthcheckService
{
    Task<Healthcheck> Get();
}

public class HealthcheckService : IHealthcheckService
{
    private readonly string _appVersion;
    private readonly string _environment;
    private readonly IFileWrapper _fileWrapper;
    private readonly string _sha;

    public HealthcheckService(string appVersionPath, string shaPath, IFileWrapper fileWrapper, string environment)
    {
        _fileWrapper = fileWrapper;
        _appVersion = GetFirstFileLineOrDefault(appVersionPath, "dev");
        _sha = GetFirstFileLineOrDefault(shaPath, string.Empty);
        _environment = environment;
    }

    public async Task<Healthcheck> Get() =>
        await Task.FromResult(new Healthcheck(_appVersion, _sha, _environment, new()));

    private string GetFirstFileLineOrDefault(string filePath, string defaultValue)
    {
        if (_fileWrapper.Exists(filePath))
        {
            string firstLine = _fileWrapper.ReadAllLines(filePath).FirstOrDefault();

            if (!string.IsNullOrEmpty(firstLine))
                return firstLine.Trim();
        }

        return defaultValue.Trim();
    }
}