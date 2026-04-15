using OperationalDataStorage.Presentation.DependencyInjection;
using OperationalDataStorage.Presentation.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddHealthChecks();
builder.Services.AddOperationalDataStorageServices(builder.Configuration);
builder.Services.AddControllers().AddJsonOptions(i => i.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureOpenApi();

var app = builder.Build();
app.MapHealthChecks("/healthz");
app.Services.AddLifetimeLogger();
app.Services.ValidateSettings();
app.Services.LogSettings();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthorization();
app.MapControllers();
app.Run();
