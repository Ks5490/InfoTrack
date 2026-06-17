using SolSearch.Factories;
using SolSearch.Models.SettingTypes;
using SolSearch.Services;
using SolSearch.Services.API;
using SolSearch.Services.DataStratergy;
using SolSearch.Services.ListingWebsites;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddOptions<PotentialClientUrls>()
    .Bind(builder.Configuration.GetSection("PotentialClientUrls"));


builder.Services.AddScoped<IReportGeneratorService, ReportGeneratorService>();
builder.Services.AddScoped<IConveyancingListingSitesFactory, ConveyancingListingSitesFactory>();

builder.Services.AddScoped<IDataCollectionService, WebScrapperService>(); // explore keyed services vs factory

builder.Services.AddScoped<SolicitorsListingService>();


builder.Services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36"); // sim browser
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "SolSearchCors", policy =>
    {
        policy.WithOrigins(
            "http://UI:5173",
            "http://localhost:5173"  

        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.UseCors("SolSearchCors");

app.MapControllers();

app.Run();
