using System.Net;
using System.Text;
using System.Text.Json;
using EuropeanStudentCard.Configuration;
using EuropeanStudentCard.DTO.EscRouter;
using EuropeanStudentCard.Exceptions;
using EuropeanStudentCard.Interfaces;
using Microsoft.Extensions.Options;

namespace EuropeanStudentCard.Services;


public class EscRouterClient : IEscRouterClient
{
    private readonly HttpClient _httpClient;
    private readonly EscRouterSettings _settings;
    private readonly ILogger<EscRouterClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public EscRouterClient(
        HttpClient httpClient,
        IOptions<EscRouterSettings> settings,
        ILogger<EscRouterClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };
    }

    public async Task<string> GenerateEscnAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = $"/cards/generate-escn?prefix={_settings.EscnPrefix}&pic={_settings.Pic}";
            _logger.LogInformation("Generating ESCN with prefix: {Prefix}, PIC: {Pic}", _settings.EscnPrefix, _settings.Pic);

            var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogWarning("ESCN generation failed with status {StatusCode}. Falling back to local GUID. Error: {Error}", 
                    response.StatusCode, errorBody);
                
                // Fallback for development when not registered yet
                return Guid.NewGuid().ToString();
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<GenerateEscnResponse>(json, _jsonOptions);

            if (result?.CardNumber != null)
            {
                _logger.LogInformation("Successfully generated ESCN: {CardNumber}", result.CardNumber);
                return result.CardNumber;
            }

            _logger.LogWarning("ESCN generation returned null. Falling back to local GUID.");
            return Guid.NewGuid().ToString();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating ESCN. Falling back to local GUID.");
            return Guid.NewGuid().ToString();
        }
    }

    public async Task<EscCardResponse?> CreateCardAsync(EscCardRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = "/cards";
            _logger.LogInformation("Creating card with ESCN: {CardNumber} for ESI: {Esi}", 
                request.CardNumber, request.PersonIdentifier);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to create card. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to create card: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<EscCardResponse>(responseJson, _jsonOptions);
            
            _logger.LogInformation("Successfully created card: {CardNumber}", result?.CardNumber);
            return result;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating card");
            throw new EscRouterException("Error creating card", ex);
        }
    }

    public async Task<List<EscCardResponse>> GetAllCardsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = "/cards";
            _logger.LogDebug("Fetching all cards");

            var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to fetch cards. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to fetch cards: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<List<EscCardResponse>>(json, _jsonOptions) ?? new List<EscCardResponse>();
            
            _logger.LogInformation("Successfully fetched {Count} cards", result.Count);
            return result;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching cards");
            throw new EscRouterException("Error fetching cards", ex);
        }
    }

    public async Task<EscCardResponse?> GetCardAsync(string escn, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = $"/cards/{escn}";
            _logger.LogDebug("Fetching card: {Escn}", escn);

            var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Card not found: {Escn}", escn);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to fetch card {Escn}. Status: {StatusCode}, Error: {Error}", 
                    escn, response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to fetch card: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<EscCardResponse>(json, _jsonOptions);
            
            _logger.LogInformation("Successfully fetched card: {Escn}", escn);
            return result;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching card {Escn}", escn);
            throw new EscRouterException($"Error fetching card {escn}", ex);
        }
    }

    public async Task<EscCardResponse?> UpdateCardAsync(string escn, EscCardRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = $"/cards/{escn}";
            _logger.LogInformation("Updating card: {Escn}", escn);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to update card {Escn}. Status: {StatusCode}, Error: {Error}", 
                    escn, response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to update card: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<EscCardResponse>(responseJson, _jsonOptions);
            
            _logger.LogInformation("Successfully updated card: {Escn}", escn);
            return result;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating card {Escn}", escn);
            throw new EscRouterException($"Error updating card {escn}", ex);
        }
    }

    public async Task<bool> DeleteCardAsync(string escn, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = $"/cards/{escn}";
            _logger.LogInformation("Deleting card: {Escn}", escn);

            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Card not found for deletion: {Escn}", escn);
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to delete card {Escn}. Status: {StatusCode}, Error: {Error}", 
                    escn, response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to delete card: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            _logger.LogInformation("Successfully deleted card: {Escn}", escn);
            return true;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting card {Escn}", escn);
            throw new EscRouterException($"Error deleting card {escn}", ex);
        }
    }

    public async Task<byte[]?> GetQrCodeAsync(string escn, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = $"/cards/{escn}/qr";
            _logger.LogDebug("Fetching QR code for card: {Escn}", escn);

            // Create request with Accept header for SVG format (default from ESC Router API)
            var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("image/svg+xml"));

            var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("QR code not found for card: {Escn}", escn);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to fetch QR code for {Escn}. Status: {StatusCode}, Error: {Error}", 
                    escn, response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to fetch QR code: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var qrBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogInformation("Successfully fetched QR code for card: {Escn}, Size: {Size} bytes", escn, qrBytes.Length);
            return qrBytes;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching QR code for card {Escn}", escn);
            throw new EscRouterException($"Error fetching QR code for card {escn}", ex);
        }
    }

    public async Task<EscCardStatusResponse?> GetCardStatusAsync(string escn, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = $"/cards/{escn}/status";
            _logger.LogDebug("Checking status for card: {Escn}", escn);

            var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Card status not found: {Escn}", escn);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to check card status {Escn}. Status: {StatusCode}, Error: {Error}", 
                    escn, response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to check card status: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<EscCardStatusResponse>(json, _jsonOptions);
            
            _logger.LogInformation("Card {Escn} status: {Status}, Valid: {Valid}", 
                escn, result?.CardStatusType, result?.Valid);
            return result;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking card status {Escn}", escn);
            throw new EscRouterException($"Error checking card status {escn}", ex);
        }
    }

    // ==================== Person Endpoints ====================

    public async Task<List<EscPersonResponse>> GetAllPersonsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = "/persons";
            _logger.LogDebug("Fetching all persons");

            var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to fetch persons. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to fetch persons: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<List<EscPersonResponse>>(json, _jsonOptions) ?? new List<EscPersonResponse>();
            
            _logger.LogInformation("Successfully fetched {Count} persons", result.Count);
            return result;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching persons");
            throw new EscRouterException("Error fetching persons", ex);
        }
    }

    public async Task<EscPersonResponse?> CreatePersonAsync(EscPersonRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = "/persons";
            _logger.LogInformation("Creating person with ESI: {Esi}", request.Identifier);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to create person. Status: {StatusCode}, Error: {Error}", 
                    response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to create person: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<EscPersonResponse>(responseJson, _jsonOptions);
            
            _logger.LogInformation("Successfully created person: {Esi}", result?.Identifier);
            return result;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person");
            throw new EscRouterException("Error creating person", ex);
        }
    }

    public async Task<EscPersonResponse?> GetPersonAsync(string esi, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = $"/persons/{Uri.EscapeDataString(esi)}";
            _logger.LogDebug("Fetching person: {Esi}", esi);

            var response = await _httpClient.GetAsync(endpoint, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Person not found: {Esi}", esi);
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to fetch person {Esi}. Status: {StatusCode}, Error: {Error}", 
                    esi, response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to fetch person: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<EscPersonResponse>(json, _jsonOptions);
            
            _logger.LogInformation("Successfully fetched person: {Esi}", esi);
            return result;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching person {Esi}", esi);
            throw new EscRouterException($"Error fetching person {esi}", ex);
        }
    }

    public async Task<EscPersonResponse?> UpdatePersonAsync(string esi, EscPersonRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = $"/persons/{Uri.EscapeDataString(esi)}";
            _logger.LogInformation("Updating person: {Esi}", esi);

            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to update person {Esi}. Status: {StatusCode}, Error: {Error}", 
                    esi, response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to update person: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            var result = JsonSerializer.Deserialize<EscPersonResponse>(responseJson, _jsonOptions);
            
            _logger.LogInformation("Successfully updated person: {Esi}", esi);
            return result;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person {Esi}", esi);
            throw new EscRouterException($"Error updating person {esi}", ex);
        }
    }

    public async Task<bool> DeletePersonAsync(string esi, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = $"/persons/{Uri.EscapeDataString(esi)}";
            _logger.LogInformation("Deleting person: {Esi}", esi);

            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken).ConfigureAwait(false);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Person not found for deletion: {Esi}", esi);
                return false;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogError("Failed to delete person {Esi}. Status: {StatusCode}, Error: {Error}", 
                    esi, response.StatusCode, errorBody);
                
                throw new EscRouterException(
                    $"Failed to delete person: {response.StatusCode}",
                    response.StatusCode,
                    errorBody,
                    endpoint);
            }

            _logger.LogInformation("Successfully deleted person: {Esi}", esi);
            return true;
        }
        catch (EscRouterException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting person {Esi}", esi);
            throw new EscRouterException($"Error deleting person {esi}", ex);
        }
    }
}