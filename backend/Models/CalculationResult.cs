using backend.Dtos;

namespace backend.Models;

public record CalculationResult
{
    public List<CoinPredictionDto> CoinPredictions { get; init; }
    public double TotalAmount { get; set; }
    public double TotalWeight { get; set; }
    public CalculationResult()
    {
        CoinPredictions = new List<CoinPredictionDto>();
    }    
}
