using EuropeanStudentCard.Clients_iKnow;
using EuropeanStudentCard.Mocks_iKnow;
using EuropeanStudentCard.Validation_iKnow;
using EuropeanStudentCard.Services_IKnow.Eligibility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IKnowStudentValidator>();
builder.Services.AddSingleton<IEscEligibilityService, EscEligibilityService>();
if (builder.Configuration.GetValue<bool>("UseMocks"))
{
    builder.Services.AddSingleton<iKnowClient, MockIKnowClient>();
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
