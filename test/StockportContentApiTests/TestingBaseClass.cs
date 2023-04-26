namespace StockportContentApiTests;

public abstract class TestingBaseClass
{
    /// <summary>
    /// Gets a http response from embeded file
    /// </summary>
    /// <param name="file">Resource path e.g. StockportConentApiTests.Unit.Test.json</param>
    /// <returns>HttpResponse with the content as a string</returns>
    protected HttpResponse GetResponseFromFile(string file)
    {
        var assembly = this.GetType().GetTypeInfo().Assembly;
        var response = new HttpResponseMessage();
        var resources = assembly.GetManifestResourceNames();
        var resourceName = resources.FirstOrDefault(f => f.Equals($"{file}", StringComparison.OrdinalIgnoreCase));
        var json = string.Empty;
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            json = reader.ReadToEnd();
        }

        return HttpResponse.Successful(json);
    }

    /// <summary>
    /// Gets the content of a embeded file as a string
    /// </summary>
    /// <param name="file">Resource path e.g. StockportConentApiTests.Unit.Test.json</param>
    /// <returns>String content of file</returns>
    protected string GetStringResponseFromFile(string file)
    {
        var assembly = this.GetType().GetTypeInfo().Assembly;
        var response = new HttpResponseMessage();
        var resources = assembly.GetManifestResourceNames();
        var resourceName = resources.FirstOrDefault(f => f.Equals($"{file}", StringComparison.OrdinalIgnoreCase));
        var json = string.Empty;
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            json = reader.ReadToEnd();
        }

        return json;
    }
}
