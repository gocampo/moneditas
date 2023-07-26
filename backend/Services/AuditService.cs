namespace backend.Services;

using backend.Dtos;
using MongoDB.Driver;

public interface IAuditService
{
    Task CreateAsync(CalculationResultDTO calculationResultDTO);
}

public class AuditService : IAuditService
{
    private readonly IConfiguration configuration;
    private readonly IMongoCollection<CalculationResultDTO> _AuditCollection;

    public AuditService(IConfiguration configuration)
    {
        this.configuration = configuration;

        _AuditCollection = new MongoClient(
            configuration["DBSettings:ConnectionString"]).GetDatabase(
            configuration["DBSettings:Database"]).GetCollection<CalculationResultDTO>(
            configuration["DBSettings:Collection"]);
    }

    public async Task CreateAsync(CalculationResultDTO calculationResultDTO) =>
        await _AuditCollection.InsertOneAsync(calculationResultDTO);

}