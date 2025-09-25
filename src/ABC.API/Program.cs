using ABC.API.Common.Extensions;
using ABC.API.Common.Middleware;
using ABC.API.Endpoints;
using ABC.Application;
using ABC.Infrastructure;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, options =>
    {
        options.WithApiKeyValidation();
        options.WithPublisher();
    })
    .AddOpenApi()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddProblemDetails();

var app = builder.Build();

app.UseSerilog();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapFeedbackEndpoints();
app.MapSentimentTermEndpoints();
app.MapProductRatingEndpoints();
app.MapProductEndpoints();

app.UseHttpsRedirection();

Log.Information("ABC API started successfully");
app.Run();