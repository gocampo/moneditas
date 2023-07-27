namespace backend.Models;

public interface ICoinCatalog
{
    Coin GetCoinByTagName(string tagName);
}

public class CoinCatalog : ICoinCatalog
{
    private List<Coin> _coins { get; init; }
    public Coin GetCoinByTagName(string tagName)
    {
        Coin? coin = _coins.Find(c => c.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase));
        if (coin == null)
        {
            throw new Exception($"Tag name {tagName} not found");
        }
        return coin;
    }
    public CoinCatalog()
    {
        _coins = new List<Coin>{
        new Coin("$ 1", "ar_1p", 1, 6.35),
        new Coin("$ 1", "ar_1p_small", 1, 4.30),
        new Coin("$ 2", "ar_2p", 2, 7.20),
        new Coin("$ 5", "ar_5p", 5, 7.30),
        new Coin("$ 10", "ar_10p", 10, 9.00)
        };
    }
}