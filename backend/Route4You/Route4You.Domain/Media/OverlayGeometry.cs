namespace Route4You.Domain.Media;

public class OverlayGeometry
{
    public List<Polyline> Polylines { get; set; } = new();
    public static OverlayGeometry Empty => new();
}