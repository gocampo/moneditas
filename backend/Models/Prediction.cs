namespace backend.Models;
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
