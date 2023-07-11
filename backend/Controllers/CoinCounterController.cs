namespace backend.Controllers;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;

[ApiController]
[Route("api")]
public class CoinCounterController : ControllerBase
{
    private const double PROBABILITY_THRESHOLD = 0.9;
    private readonly ICoinManager coinManager;
    private CalculationResult SumCoins(IEnumerable<Prediction> predictions)
    {
        var result = new CalculationResult();
        foreach (var prediction in predictions)
        {
            if (prediction.probability > PROBABILITY_THRESHOLD)
            {
                Coin coin = coinManager.GetCoinByTagName(prediction.tagName);
                result.CoinPredictions.Add(new Dtos.CoinPredictionDto(coin.Name,
                                                                         coin.Value,
                                                                         coin.Weight,
                                                                         prediction.probability,
                                                                         prediction.boundingBox));
                result.TotalAmount += coin.Value;
                result.TotalWeight += coin.Weight;
            }
        }
        return result;
    } 

    private byte[] LoadFromFile(){
        string filePath = "/var/tmp/coin.jpg";
        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
        return fileBytes;
    }

    [HttpGet]
    public ActionResult<string> Get()
    {
        Console.WriteLine("Get");
        return Ok("Hello world! Everything is working fine here.");
    }

    [HttpPost]
    public async Task<ActionResult<CalculationResult>> Count()
    {
        Console.WriteLine("Count");

        // byte[] imageBytes = LoadFromFile();
        
        byte[] imageBytes;
        using (var stream = new MemoryStream())
        {
            await Request.Body.CopyToAsync(stream);
            imageBytes = stream.ToArray();
        }

        HttpClient client = new HttpClient();
        string uri = "https://moneditas-prediction.cognitiveservices.azure.com/customvision/v3.0/Prediction/bd7a3841-89b3-4468-96d2-c051ab32af86/detect/iterations/Iteration8/image";
        client.DefaultRequestHeaders.Add("Prediction-Key", "db86459213904561a66b753453f41090");
        HttpContent content = new ByteArrayContent(imageBytes);
        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
        HttpResponseMessage response = await client.PostAsync(uri, content);
        response.EnsureSuccessStatusCode();
        string apiResponse = response.Content.ReadAsStringAsync().Result;
        if (apiResponse.Equals(string.Empty)){
            return NotFound();
        }
        Console.WriteLine(apiResponse);
        var customVisionAPIPredictions = JsonSerializer.Deserialize<CustomVisionAPIResponse>(apiResponse);
        if (customVisionAPIPredictions == null || customVisionAPIPredictions.predictions == null)
        {
            return NotFound();
        }
        CalculationResult result = SumCoins(customVisionAPIPredictions.predictions);
        Console.WriteLine(result);

        return result;
    }

    public CoinCounterController(ICoinManager coinManager)
    {
        this.coinManager = coinManager;

    }

}