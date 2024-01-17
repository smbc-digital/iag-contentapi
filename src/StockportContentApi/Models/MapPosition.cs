namespace StockportContentApi.Model;

public class MapPosition
{
    public double Lon { get; set; }
    public double Lat { get; set; }
    public GeoCoordinate Coordinates { get => new GeoCoordinate(Lat, Lon); }

    public double Distance(MapPosition destination)
    {
        return Coordinates.GetDistanceTo(new GeoCoordinate(destination.Lat, Lon));
    }
}
