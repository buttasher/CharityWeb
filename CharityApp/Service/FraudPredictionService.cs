using CharityApp.Models;
using System.Text.Json;
using System.Text;

public class FraudPredictionService
{
    private readonly HttpClient _httpClient;

    public FraudPredictionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(bool fraud, string method, List<string> flags, double confidence)> PredictFraudAsync(Donation donation)
    {
        var requestData = new
        {
            amount = donation.Amount,
            vpn_used = (donation.VpnUsed ?? false) ? 1 : 0,
            failed_attempts = donation.FailedAttempts,
            country = donation.Country,
            email = donation.Email
        };

        var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("http://localhost:5000/predict", content);
        var result = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(result);
        var root = doc.RootElement;

        bool isFraud = root.GetProperty("fraud").GetBoolean();
        string method = root.GetProperty("method").GetString();
        double confidence = root.TryGetProperty("confidence", out var c) ? c.GetDouble() : 0.0;

        List<string> flags = new();
        if (root.TryGetProperty("flags", out var fList))
        {
            foreach (var item in fList.EnumerateArray())
                flags.Add(item.GetString());
        }

        return (isFraud, method, flags, confidence);
    }
}
