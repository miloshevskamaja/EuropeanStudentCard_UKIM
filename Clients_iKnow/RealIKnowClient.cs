using EuropeanStudentCard.Models_iKnow;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace EuropeanStudentCard.Clients_iKnow
{
    // this will be used when we will have access token to the real iKnow API
    public class RealIKnowClient : iKnowClient

    {
        private readonly HttpClient _http;

        public RealIKnowClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<IKnowStudentDto?> GetStudentByIndexAsync(string index, CancellationToken ct)
        {
            // example:
            // GET /api/students/{index}

            var response = await _http.GetAsync($"/api/students/{Uri.EscapeDataString(index)}", ct);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            return JsonSerializer.Deserialize<IKnowStudentDto>(json, options);
        }

        public Task<IReadOnlyList<IKnowStudentDto>> GetActiveStudentsAsync(CancellationToken ct)
        {
            // TODO:  endpoint for list of regular students
            throw new NotImplementedException("Active students endpoint not implemented yet.");
        }

        // Helper: if iKnow needs Bearer token
        public void SetBearerToken(string token)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
