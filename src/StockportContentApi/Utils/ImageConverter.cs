namespace StockportContentApi.Utils;

public static class ImageConverter
{
    private const string ImageHeight = "250";

    public static string ConvertToThumbnail(string imageUrl) =>
        string.IsNullOrEmpty(imageUrl) ? string.Empty : $"{imageUrl}?h={ImageHeight}";
}
