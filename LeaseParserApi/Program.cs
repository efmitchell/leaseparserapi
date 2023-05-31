using System.Threading.Tasks;
using Farmer;
using Farmer.Builders;
using LeaseParserApi;
using LeaseParserApi.Interfaces;
using LeaseParserApi.Models;
using LeaseParserApi.Parser;
using LeaseParserApi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Registering the HTTP client factory and the services
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Register the memory cache services
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ICacheProvider, InMemoryCacheProvider>();

builder.Services.AddTransient<IScheduleParserService, ScheduleParserService>();
builder.Services.AddTransient<IRawScheduleDataService, RawScheduleDataService>();

// Register the parsers
builder.Services.AddSingleton<IEntryTextParser, RegistrationDateParser>();
builder.Services.AddSingleton<IEntryTextParser, PropertyParser>();
builder.Services.AddSingleton<IEntryTextParser, LeaseDateParser>();
builder.Services.AddSingleton<IEntryTextParser, LesseeTitleParser>();

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "LeaseParserApi", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "LeaseParserApi v1"));

app.UseHttpsRedirection();

app.MapControllers();
app.MapHealthChecks("/health");

/*// Farmer deployment script, perhaps consider adding a deployment script for IaC to get the code into production once pipelines are setup?
var deployment = new Farmer.ArmBuilder()
    .Location(Farmer.Locations.WestEurope)
    .AddWebApp(w => w.WithName("LeaseParserApi"))
    .Build();

var deploymentResult = await deployment.DeployAsync();*/

app.Run();