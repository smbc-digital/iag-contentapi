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
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        HttpResponseMessage response = new();
        string[] resources = assembly.GetManifestResourceNames();
        string resourceName = resources.FirstOrDefault(f => f.Equals(file, StringComparison.OrdinalIgnoreCase));
        string json = string.Empty;
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new(stream))
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
        Assembly assembly = GetType().GetTypeInfo().Assembly;
        HttpResponseMessage response = new();
        string[] resources = assembly.GetManifestResourceNames();
        string resourceName = resources.FirstOrDefault(f => f.Equals(file, StringComparison.OrdinalIgnoreCase));
        string json = string.Empty;
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new(stream))
        {
            json = reader.ReadToEnd();
        }

        return json;
    }
}
