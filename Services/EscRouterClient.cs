using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using EuropeanStudentCard.Interfaces;
using EuropeanStudentCard.Models;

namespace EuropeanStudentCard.Services
{
    /// <summary>
    /// Real implementation that talks to the ESC Router V2 API.
    /// It expects an API bearer token (provided via configuration or env var).
    /// </summary>
    public class EscRouterClient : IEscRouterClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://router.europeanstudentcard.eu/esc-rest/v3/api/v2"; // V2 endpoint base
        private readonly string _bearerToken;

        public EscRouterClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _bearerToken = Environment.GetEnvironmentVariable("ESC_ROUTER_TOKEN") ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(_bearerToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _bearerToken);
            }
        }
        // ------------------------------------------------------------
        // 1. Generate ESCN (card number) – GET /cards/generate-escn
        // ------------------------------------------------------------
        public async Task<string> GenerateEscnAsync()
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/cards/generate-escn?prefix='ADD_PREFIX'&pic='ADD_PIC'");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[EscRouterClient] GenerateEscn failed: {response.StatusCode}");
                // Fallback to local GUID generation so the app still works in mock mode
                return Guid.NewGuid().ToString();
            }
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(json);
                // Expected shape: { "cardNumber": "<uuid>" } or just a plain string
                if (doc.RootElement.TryGetProperty("cardNumber", out var prop))
                {
                    return prop.GetString() ?? Guid.NewGuid().ToString();
                }
                // If the API returns the raw string directly
                return doc.RootElement.GetString() ?? Guid.NewGuid().ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EscRouterClient] GenerateEscn JSON error: {ex.Message}");
                return Guid.NewGuid().ToString();
            }
        }
        public async Task<bool> IssueCardAsync(StudentCard card)
        {
            var payload = new
            {
                cardNumber = card.CardNumber,
                personIdentifier = card.PersonIdentifier,
                issuerIdentifier = card.IssuerIdentifier,
                cardStatusType = card.CardStatusType,
                issuedAt = card.IssuedAt.ToString("yyyy-MM-dd"),
                expiresAt = card.ExpiresAt?.ToString("yyyy-MM-dd"),
                cardType = card.CardType,
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            // ADD BEARER TOKEN AS AUTHORIZATION HEADER
            var response = await _httpClient.PostAsync($"{_baseUrl}/cards", content);
            if (!response.IsSuccessStatusCode)
            {   
                Console.WriteLine($"[EscRouterClient] IssueCard failed: {response.StatusCode}");
                return false;
            }
            return true;
        }

        public async Task<string> CheckCardStatusAsync(string cardNumber)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/cards/{cardNumber}/status");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[EscRouterClient] CheckCardStatus failed: {response.StatusCode}");
                return "UNKNOWN";
            }
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("cardStatusType", out var statusProp))
                {
                    if (statusProp.ValueKind == JsonValueKind.Object && statusProp.TryGetProperty("key", out var keyProp))
                        return keyProp.GetString() ?? "UNKNOWN";
                    return statusProp.GetString() ?? "UNKNOWN";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EscRouterClient] JSON parse error: {ex.Message}");
            }
            return "UNKNOWN";
        }

        public Task<byte[]> GetQrCodeAsync(string cardNumber)
        {
            throw new NotImplementedException();
        }

        // Update an existing card (PUT)
        public async Task<bool> UpdateCardAsync(StudentCard card)
        {
            var payload = new
            {
                cardNumber = card.CardNumber,
                personIdentifier = card.PersonIdentifier,
                issuerIdentifier = card.IssuerIdentifier,
                cardStatusType = card.CardStatusType,
                issuedAt = card.IssuedAt.ToString("yyyy-MM-dd"),
                expiresAt = card.ExpiresAt?.ToString("yyyy-MM-dd"),
                cardType = card.CardType,
            };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{_baseUrl}/cards/{card.CardNumber}", content);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[EscRouterClient] UpdateCard failed: {response.StatusCode}");
                return false;
            }
            return true;
        }

        // Revoke a card (DELETE)
        public async Task<bool> RevokeCardAsync(string cardNumber)
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/cards/{cardNumber}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[EscRouterClient] RevokeCard failed: {response.StatusCode}");
                return false;
            }
            return true;
        }

        // Retrieve full card details (GET)
        public async Task<StudentCard?> GetCardAsync(string cardNumber)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/cards/{cardNumber}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[EscRouterClient] GetCard failed: {response.StatusCode}");
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var card = JsonSerializer.Deserialize<StudentCard>(json, options);
                return card;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EscRouterClient] Deserialization error: {ex.Message}");
                return null;
            }
        }
    }
}
