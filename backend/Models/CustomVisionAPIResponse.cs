namespace backend.Models;

public record CustomVisionAPIResponse
{
    public string? id { get; set; }
    public string? project { get; set; }
    public string? iteration { get; set; }
    public DateTime created { get; set; }
    public Prediction[]? predictions { get; set; }
}