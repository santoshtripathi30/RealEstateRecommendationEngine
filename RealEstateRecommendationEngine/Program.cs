using Microsoft.OpenApi.Models;

using RealEstateRecommendationEngine.Infrastructure;
using RealEstateRecommendationEngine.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigureMiddleware(app);

app.Run();


// =======================
// Methods for Clean Separation
// =======================

void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Real Estate Recommendation API",
            Version = "v1"
        });
    });

    // DI Registrations
    services.AddSingleton<IAIPipelineServices, AIPipelineServices>();
    services.AddSingleton<IPropertyServices, PropertyServices>();
    services.AddSingleton<IRecommendationService, RecommendationService>();
    services.AddSingleton<IFileHelper, FileHelper>();
}

void ConfigureMiddleware(WebApplication app)
{
    var enableSwagger = Environment.GetEnvironmentVariable("ENABLE_SWAGGER") == "true";

   // if (app.Environment.IsDevelopment() || enableSwagger)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Real Estate API v1");
        });
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
}
