namespace backend.Dtos;

public record CalculationResultDTO
{
    public List<CoinPredictionDto> CoinPredictions { get; init; }
    public decimal TotalAmount { get; set; }
    public decimal TotalWeight { get; set; }
    public int TotalCount { get; internal set; }

    public CalculationResultDTO()
    {
        CoinPredictions = new List<CoinPredictionDto>();
    }    
}
