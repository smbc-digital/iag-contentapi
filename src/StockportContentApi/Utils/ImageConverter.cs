namespace StockportContentApi.Utils;

public static class ImageConverter
{
    private const string ImageHeight = "250";

    public static string ConvertToThumbnail(string imageUrl)
    {
        return string.IsNullOrEmpty(imageUrl) ? "" : imageUrl + $"?h={ImageHeight}";
    }
}
