using EuropeanStudentCard.DTO.iKnow;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EuropeanStudentCard.Clients.iKnowClients
{
    public class MockIKnowClient : iKnowClient
    {
        private readonly IReadOnlyList<IKnowStudentDto> _students;

        public MockIKnowClient(IWebHostEnvironment env)
        {
            var path = Path.Combine(env.ContentRootPath, "Data", "iKnowMockData", "students.json");
            var json = File.ReadAllText(path);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            _students = JsonSerializer.Deserialize<List<IKnowStudentDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<IKnowStudentDto>();
        }


        public Task<IReadOnlyList<IKnowStudentDto>> GetActiveStudentsAsync(CancellationToken ct = default)
        {
            var list = _students.Where(x => x.status == 1).ToList();
            return Task.FromResult((IReadOnlyList<IKnowStudentDto>)list);
        }

        public Task<IKnowStudentDto?> GetStudentByIndexAsync(string index, CancellationToken ct = default)
        {
            var s = _students.FirstOrDefault(x => x.index.Equals(index, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(s);
        }
    }
}
