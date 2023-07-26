namespace backend.Models;
public record Coin
{
    public string Name { get; init; }
    public string TagName { get; init; }
    public double Value { get; init; }
    public double Weight { get; init; }  

    public Coin(string name, string tagName,  double value, double weight)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        TagName = tagName ?? throw new ArgumentNullException(nameof(tagName));
        Value = value;
        Weight = weight;
    }    
}
