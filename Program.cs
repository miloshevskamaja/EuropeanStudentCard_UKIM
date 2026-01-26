using System.Text.Json.Serialization;
using EuropeanStudentCard.Configuration;
using EuropeanStudentCard.Data;
using EuropeanStudentCard.Interfaces;
using EuropeanStudentCard.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


var sqliteConnection = new SqliteConnection("DataSource=:memory:");
sqliteConnection.Open();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references between Student and StudentCard
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
    
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(sqliteConnection));

builder.Services.Configure<EscRouterSettings>(
    builder.Configuration.GetSection("EscRouter"));

builder.Services.AddHttpClient<IEscRouterClient, EscRouterClient>((serviceProvider, client) =>
{
    var settings = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<EscRouterSettings>>().Value;
    client.BaseAddress = new Uri(settings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
    
    // Add Bearer token if configured
    if (settings.HasToken)
    {
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", settings.Token);
    }
});

builder.Services.AddScoped<ICardService, CardService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // ✅ creates Students table
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
