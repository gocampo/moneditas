namespace backend.Controllers;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
using backend.Dtos;
using backend.Services;

[ApiController]
[Route("api")]
public class CoinCounterController : ControllerBase
{
    private const double PROBABILITY_THRESHOLD = 0.9;
    private readonly ICoinCatalog _coinCatalog;
    private readonly IConfiguration _configuration;
    private readonly IAuditService _auditService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CoinCounterController> _logger;

    private CalculationResultDTO SumCoins(IEnumerable<Prediction> predictions)
    {
        var result = new CalculationResultDTO();
        foreach (var prediction in predictions)
        {
            if (prediction.probability > PROBABILITY_THRESHOLD)
            {
                Coin coin = _coinCatalog.GetCoinByTagName(prediction.tagName);
                result.CoinPredictions.Add(new Dtos.CoinPredictionDto(coin.Name,
                                                                         coin.Value,
                                                                         coin.Weight,
                                                                         prediction.probability,
                                                                         prediction.boundingBox));
                result.TotalAmount += coin.Value;
                result.TotalWeight += coin.Weight;
                result.TotalCount++;
            }
        }
        return result;
    }

    private byte[] LoadFromFile()
    {
        string filePath = "/var/tmp/coin.jpg";
        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        return fileBytes;
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
        _logger.LogInformation("Get");
        return Ok("Hello world! Everything is working fine here.");
    }

    [HttpPost]
    public async Task<ActionResult<CalculationResultDTO>> Count()
    {
        _logger.LogInformation("Count");

        byte[] imageBytes;
        using (var stream = new MemoryStream())
        {
            await Request.Body.CopyToAsync(stream);
            imageBytes = stream.ToArray();
        }

        var httpClient = _httpClientFactory.CreateClient();
        string uri = _configuration["PredictionAPI:URI"];
        string predictionKey = _configuration["PredictionAPI:Key"];
        httpClient.DefaultRequestHeaders.Add("Prediction-Key", predictionKey);
        HttpContent content = new ByteArrayContent(imageBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

        HttpResponseMessage response = await httpClient.PostAsync(uri, content);

        response.EnsureSuccessStatusCode();
        string apiResponse = await response.Content.ReadAsStringAsync();
        if (apiResponse.Equals(string.Empty))
        {
            return NotFound();
        }
        var customVisionAPIPredictions = JsonSerializer.Deserialize<CustomVisionAPIResponse>(apiResponse);
        if (customVisionAPIPredictions == null || customVisionAPIPredictions.predictions == null)
        {
            return NotFound();
        }
        CalculationResultDTO result = SumCoins(customVisionAPIPredictions.predictions);
        await _auditService.CreateAsync(result);
        return result;
    }

    public CoinCounterController(IConfiguration configuration,
        ICoinCatalog coinCatalog,
        IAuditService auditService,
        IHttpClientFactory httpClientFactory,
        ILogger<CoinCounterController> logger)
    {
        _coinCatalog = coinCatalog;
        _configuration = configuration;
        _auditService = auditService;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

}