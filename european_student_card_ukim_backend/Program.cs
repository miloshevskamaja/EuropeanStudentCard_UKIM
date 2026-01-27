using EuropeanStudentCard.Clients_iKnow;
using EuropeanStudentCard.Mocks_iKnow;
using EuropeanStudentCard.Validation_iKnow;
using EuropeanStudentCard.Services_IKnow.Eligibility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System.Text.Json.Serialization;

using EuropeanStudentCard.Data;
using EuropeanStudentCard.Configuration;
using EuropeanStudentCard.Interfaces;
using EuropeanStudentCard.Services;

var builder = WebApplication.CreateBuilder(args);

var sqliteConnection = new SqliteConnection("DataSource=:memory:");
sqliteConnection.Open();
// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Handle circular references between Student and StudentCard
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddSingleton<IKnowStudentValidator>();
builder.Services.AddSingleton<IEscEligibilityService, EscEligibilityService>();
if (builder.Configuration.GetValue<bool>("UseMocks"))
{
    builder.Services.AddSingleton<iKnowClient, MockIKnowClient>();
}


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated(); // creates Students table
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
