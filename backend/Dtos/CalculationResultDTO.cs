namespace backend.Dtos;

public record CalculationResultDTO
{
    public List<CoinPredictionDto> CoinPredictions { get; init; }
    public double TotalAmount { get; set; }
    public double TotalWeight { get; set; }
    public int TotalCount { get; internal set; }

    public CalculationResultDTO()
    {
        CoinPredictions = new List<CoinPredictionDto>();
    }    
}
