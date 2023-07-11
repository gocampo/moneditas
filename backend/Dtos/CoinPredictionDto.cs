using backend.Models;

namespace backend.Dtos;

public record CoinPredictionDto{
    public string Name { get; init;}
    public double Value { get; init;}
    public double Weight { get; init;}
    public double Probability {get; init;}
    public BoundingBox BoundingBox { get; init;} 
    public CoinPredictionDto(string name, double value, double weight, double probability, BoundingBox boundingBox)
    {
        Name = name;
        Value = value;
        Weight = weight;
        Probability = probability;
        BoundingBox = boundingBox with {};
    }
}

