using Microsoft.Extensions.Http.Resilience;
using Yarp.ReverseProxy.Forwarder;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("yarp-resilient")
    .AddStandardResilienceHandler(options =>
    {
        options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(2);
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);

        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.MinimumThroughput = 20;
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);
    });

    builder.Services.AddSingleton<IForwarderHttpClientFactory, ResilienceForwarderHttpClientFactory>();

builder.Services.AddReverseProxy()
.LoadFromConfig(builder.Configuration
.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

app.MapGet("/", () => "API Gateway is running!");

app.Run();
