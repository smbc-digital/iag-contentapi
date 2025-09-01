namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class MapPosition
{
    public double Lon { get; set; }
    public double Lat { get; set; }

    [JsonIgnore]
    public GeoCoordinate Coordinates { get => new(Lat, Lon); }

    public double Distance(MapPosition destination) =>
        Coordinates.GetDistanceTo(new(destination.Lat, destination.Lon));
}