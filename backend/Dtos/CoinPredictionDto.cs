using backend.Models;

namespace backend.Dtos;

public record CoinPredictionDto{
    public string Name { get; init;}
    public decimal Value { get; init;}
    public decimal Weight { get; init;}
    public float Probability {get; init;}
    public BoundingBox BoundingBox { get; init;} 
    public CoinPredictionDto(string name, decimal value, decimal weight, float probability, BoundingBox boundingBox)
    {
        Name = name;
        Value = value;
        Weight = weight;
        Probability = probability;
        BoundingBox = boundingBox with {};
    }
}

