namespace backend.Models;

public record BoundingBox
{
    public float left { get; init; }
    public float top { get; init; }
    public float width { get; init; }
    public float height { get; init; }
}

public record Prediction
{
    public float probability { get; init; }
    public string tagId { get; init; }
    public string tagName { get; init; }
    public BoundingBox boundingBox { get; init; }

    public Prediction(string tagid,
                      string tagname,
                      float probability,
                      BoundingBox boundingBox)
    {
        this.probability = probability;
        tagId = tagid;
        tagName = tagname;
        this.boundingBox = boundingBox;
    }
}

public record CustomVisionAPIResponse
{
    public string? id { get; set; }
    public string? project { get; set; }
    public string? iteration { get; set; }
    public DateTime created { get; set; }
    public Prediction[]? predictions { get; set; }
}