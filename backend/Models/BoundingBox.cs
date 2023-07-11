namespace backend.Models;
public record BoundingBox
{
    public float left { get; init; }
    public float top { get; init; }
    public float width { get; init; }
    public float height { get; init; }
}
