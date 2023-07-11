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

public interface ICoinManager
{
    Coin GetCoinByTagName(string tagName);
}

public class CoinManager : ICoinManager
{
    private List<Coin> Coins { get; init; }
    public Coin GetCoinByTagName(string tagName)
    {
        Coin? coin = Coins.Find(c => c.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        if (coin == null)
        {
            throw new Exception($"Tag name {tagName} not found");
        }
        return coin;
    }
    public CoinManager()
    {
        Coins = new List<Coin>();
        Coins.Add(new Coin("$ 1", "ar_1p", 1, 6.35));
        Coins.Add(new Coin("$ 1", "ar_1p_small", 1, 4.30));
        Coins.Add(new Coin("$ 2", "ar_2p", 2, 7.20));
        Coins.Add(new Coin("$ 5", "ar_5p", 5, 7.30));
        Coins.Add(new Coin("$ 10", "ar_10p", 10, 9.00));
    }
}