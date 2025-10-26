using OurCity.Api.Middlewares;
using Scalar.AspNetCore;
using Serilog;

/*
 * Lots of code setup from ChatGPT, asking for various things at various points (e.g. how to setup Serilog)
 */

var builder = WebApplication.CreateBuilder(args);

//Logging
builder.Host.UseSerilog((ctx, config) => config.ReadFrom.Configuration(builder.Configuration));

//CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

//OpenAPI
builder.Services.AddOpenApi();

//HTTP request/response
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = (context) =>
    {
        context.ProblemDetails.Extensions.Remove("traceId");
    };
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();
app.UseCorrelationId();
app.UseCors();
app.UseSecurityHeaders();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    //Multiple API documentation tools
    app.MapScalarApiReference();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.Run();
