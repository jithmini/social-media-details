using Microsoft.OpenApi.Models;
using SocialMediaDetailAppBackend.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SocialMediaDetailAppBackend", Version = "v1" });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-API-KEY",
        Type = SecuritySchemeType.ApiKey,
        Description = "API Key needed to access the endpoints"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                }
            },
            new string[] {}
        }
    });
});

// Read CORS settings from configuration

var corsSettings = builder.Configuration.GetSection("CorsSettings");

var allowedOrigin = corsSettings.GetValue<string>("AllowedOrigin");



// Add CORS policy

builder.Services.AddCors(options =>

{

    options.AddPolicy("AllowReactApp",

        policy =>

        {

            policy.WithOrigins(allowedOrigin)

                  .AllowAnyHeader()

                  .AllowAnyMethod();

        });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SocialMediaDetailAppBackend v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
