namespace backend.Models;
public record Coin
{
    public string Name { get; init; }
    public string TagName { get; init; }
    public decimal Value { get; init; }
    public decimal Weight { get; init; }

    public Coin(string name, string tagName, decimal value, decimal weight)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        TagName = tagName ?? throw new ArgumentNullException(nameof(tagName));
        Value = value;
        Weight = weight;
    }
}
