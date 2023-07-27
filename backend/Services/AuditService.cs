namespace backend.Services;

using backend.Dtos;
using MongoDB.Driver;

public interface IAuditService
{
    Task CreateAsync(CalculationResultDTO calculationResultDTO);
}

public class AuditService : IAuditService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuditService> _logger;
    private readonly IMongoCollection<CalculationResultDTO> _auditCollection;

    public AuditService(IConfiguration configuration, ILogger<AuditService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        _logger.LogInformation("Connecting to MongoDB...");
        _logger.LogInformation("Connection string: " + configuration["DBSettings:ConnectionString"]);
        _logger.LogInformation("Database: " + configuration["DBSettings:Database"]);
        _logger.LogInformation("Collection: " + configuration["DBSettings:Collection"]);

        _auditCollection = new MongoClient(
            configuration["DBSettings:ConnectionString"]).GetDatabase(
            configuration["DBSettings:Database"]).GetCollection<CalculationResultDTO>(
            configuration["DBSettings:Collection"]);
    }

    public async Task CreateAsync(CalculationResultDTO calculationResultDTO) =>
        await _auditCollection.InsertOneAsync(calculationResultDTO);

}